using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Utility;

namespace Lynicon.Models
{
    // Experimental code not yet in use

    /// <summary>
    /// Node to insert in an HTML document
    /// </summary>
    public class InsertNode
    {
        public int Start { get; set; }
        public int End { get; set; }
        public string TagName { get; set; }
        public string Id { get; set; }
        public string AttributeValue { get; set;}
    }

    /// <summary>
    /// [Experimental code] Html processor which can insert nodes into an HTML document
    /// </summary>
    public class DocumentNodeInserter
    {
        public List<InsertNode> ExistingNodes { get; set; }

        public string TagName { get; set; }
        public string TagAttribute { get; set; }

        int pos = 0;

        string html;
        public DocumentNodeInserter(string html, string tagName, string tagAttribute)
        {
            this.html = html;
            this.TagName = tagName;
            this.TagAttribute = tagAttribute;
        }

        public void Process()
        {
            Document(); 
        }

        public void Document()
        {
            Comments();
            Doctype();
            Comments();
            Tag();
        }

        public void Comments()
        {
            Whitespace();
            if (Match("<!--"))
            {
                pos += 4;
                Scan("-->");
                pos += 3;
                Comments();
            }
        }

        public void Doctype()
        {
            Whitespace();
            if (MatchCI("<!doctype"))
            {
                pos += 9;
                Scan(">");
                pos += 1;
            }
        }

        public void Tag()
        {
            Whitespace();
        }

        public void Whitespace()
        {
            while (pos < html.Length && char.IsWhiteSpace(html[pos]))
                pos++;
        }

        public bool Match(string s)
        {
            int p2 = pos;
            for (int i = 0; i < s.Length; i++)
                if (s[i] != html[p2++])
                    return false;
            return true;
        }

        public bool MatchCI(string s)
        {
            int p2 = pos;
            for (int i = 0; i < s.Length; i++)
                if (char.ToLower(s[i]) != char.ToLower(html[p2++]))
                    return false;
            return true;
        }

        public string Scan(string end)
        {
            return Scan(new string[] { end });
        }
        public string Scan(string[] ends)
        {
            while (pos < html.Length)
            {
                for (int i = 0; i < ends.Length; i++)
                    if (MatchCI(ends[i]))
                        return ends[i];
                pos++;
            }
            return null;
        }
    }
}
