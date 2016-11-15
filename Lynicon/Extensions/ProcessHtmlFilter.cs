using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.IO;
using System.Diagnostics;
using Lynicon.Utility;
using Lynicon.Extensibility;
using System.Web.Mvc;
using System.Web;
using Lynicon.Modules;

namespace Lynicon.Extensions
{
    /// <summary>
    /// Filter for post processing the output HTML
    /// </summary>
    public class ProcessHtmlFilter : MemoryStream
    {
        //public static OrderedProcess<HtmlDocument> GetProcesses()
        //{
        //    var op = HttpContext.Current.Items["Lynicon_htmlprocesses"] as OrderedProcess<HtmlDocument>;
        //    if (op == null)
        //    {
        //        op = new OrderedProcess<HtmlDocument>();
        //        HttpContext.Current.Items["Lynicon_htmlprocesses"] = op;
        //    }
        //    return op;
        //}

        StreamWriter sw;
        Controller controller;

        /// <summary>
        /// Create a filter for post processing HTML from FilterContext properties
        /// </summary>
        /// <param name="s">Output stream</param>
        /// <param name="controller">The controller which output the stream</param>
        public ProcessHtmlFilter(Stream s, Controller controller)
        {
            sw = new StreamWriter(s);
            this.controller = controller;
        }

        /// <summary>
        /// Process the output HTML as a string
        /// </summary>
        /// <param name="markup">Output HTML</param>
        /// <returns>Processed HTML</returns>
        public virtual string ProcessHtml(string markup)
        {
            DateTime st = DateTime.Now;

            HtmlDocument doc = new HtmlDocument();
            HtmlNode.ElementsFlags["link"] = HtmlElementFlag.Empty;
            doc.Load(new StringReader(markup));
            doc.OptionWriteEmptyNodes = true;

            // Apply registered processes
            //var processes = GetProcesses();
            //doc = processes.Process(doc);
            doc = (HtmlDocument)EventHub.Instance.ProcessEvent("PostProcess.Html", this, doc).Data;

            // redirect link targets to top so we don't get recursing editors
            if ((controller.HttpContext.Request.QueryString["$mode"] ?? "").ToLower() == "view")
            {
                HtmlNode head = doc.DocumentNode.SelectSingleNode("/html/head");
                HtmlNode baseTag = doc.CreateElement("base");
                baseTag.SetAttributeValue("target", "_top");
                if (!HttpContext.Current.Request.IsLocal)
                {
                    string canonical = UrlHelperX.CanonicalGetter();
                    if (canonical != null && canonical.Contains("://"))
                    {
                        baseTag.SetAttributeValue("href", canonical.UpTo("://") + "://" + canonical.After("://").UpTo("/"));
                    }
                }

                head.AppendChild(baseTag);
            }

            Debug.WriteLine("process html " + (DateTime.Now - st).TotalMilliseconds + "ms");

            return doc.DocumentNode.OuterHtml;
        }

        /// <summary>
        /// Close the stream, processing and writing the output
        /// </summary>
        public override void Close()
        {
            byte[] allBytes = this.ToArray();
            string s = System.Text.Encoding.UTF8.GetString(allBytes);

            if (!s.Contains("</html>"))
            {
                s = string.Format("<html><head></head><body>{0}</body></html>", s);
            }

            s = ProcessHtml(s);

            sw.Write(s);
            sw.Flush();
            sw.Close();
            base.Close();
        }

    }
}
