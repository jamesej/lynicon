using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Threading;
using Lynicon.Collation;
using Lynicon.Extensibility;
using Lynicon.Repositories;
using Lynicon.Base.Modules;
using Lynicon.Base.Models;

namespace Lynicon.Test.Models
{
    public class VersioningTest
    {
        public Random rand = new Random();
        public int Option { get; set; }
        public void Run()
        {
            Option = rand.Next(4);

            switch (Option)
            {
                case 0:
                    try
                    {
                        var res = Collator.Instance.Get<HeaderContent>(new Address(typeof(HeaderContent), "peaches"));
                        if (res == null)
                            Debug.WriteLine("--Option 0: Thread: {0}, No result", Thread.CurrentThread.ManagedThreadId);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("--Option 0: Thread: {0}, {1} \n {2}", Thread.CurrentThread.ManagedThreadId, ex.Message, ex.StackTrace);
                    }
                    break;

                case 1:
                    VersionManager.Instance.PushState(VersioningMode.All);
                    try
                    {
                        var res = Collator.Instance.Get<HeaderContent, ContentItem>(iq => iq.Where(ci => ci.Path == "y"));
                        int cnt = res == null ? 0 : res.Count();
                        if (cnt < 2)
                            Debug.WriteLine("--Option 1: Thread: {0}, {1} (< 2) results", Thread.CurrentThread.ManagedThreadId, cnt);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("--Option 1: Thread: {0}, {1} \n {2}", Thread.CurrentThread.ManagedThreadId, ex.Message, ex.StackTrace);
                    }
                    finally
                    {
                        VersionManager.Instance.PopState();
                    }
                    break;

                case 2:
                    VersionManager.Instance.PushState(VersioningMode.Specific, Publishing.PublishedVersion);
                    try
                    {
                        var res = Repository.Instance.Get<ContentItem>(typeof(HeaderContent), iq => iq.Where(ci => ci.Path == "peaches"));
                        int cnt = res == null ? 0 : res.Count();
                        var res0 = res.FirstOrDefault();
                        bool isPub = res0 == null ? false : ((IPublishable)res0).IsPubVersion.Value;
                        if (cnt != 1 || !isPub)
                            Debug.WriteLine("--Option 2: Thread: {0}, {1} (!= 1) results, isPub = {2}", Thread.CurrentThread.ManagedThreadId, cnt, isPub);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("--Option 2: Thread: {0}, {1} \n {2}", Thread.CurrentThread.ManagedThreadId, ex.Message, ex.StackTrace);
                    }
                    finally
                    {
                        VersionManager.Instance.PopState();
                    }
                    break;

                case 3:
                    VersionManager.Instance.PushState(VersioningMode.All);
                    try
                    {
                        Run();
                        Debug.WriteLine(">> [{0}]", Thread.CurrentThread.ManagedThreadId);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("--Option 1: Thread: {0}, {1} \n {2}", Thread.CurrentThread.ManagedThreadId, ex.Message, ex.StackTrace);
                    }
                    finally
                    {
                        VersionManager.Instance.PopState();
                    }
                    break;

                default:
                    Debug.WriteLine("Unknown test action");
                    break;
            }
        }
    }
}