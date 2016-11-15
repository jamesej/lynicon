using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Lynicon.Utility;

namespace Lynicon.Routing
{
    /// <summary>
    /// A mock class for HttpContextBase which can easily be created
    /// populating just the fields needed for testing routing in Lynicon
    /// </summary>
    public class MockHttpContextBase : HttpContextBase
    {
        private readonly HttpRequestBase mockHttpRequestBase;
        private Dictionary<string, object> items;

        /// <summary>
        /// Create a mock HttpContextBase with just the url of the request
        /// </summary>
        /// <param name="appRelativeUrl">The url of the request</param>
        public MockHttpContextBase(string appRelativeUrl)
        {
            if (!appRelativeUrl.StartsWith("~"))
                appRelativeUrl = "~" + appRelativeUrl;
            this.mockHttpRequestBase = new MockHttpRequestBase(appRelativeUrl);
            this.items = new Dictionary<string, object>();
        }

        public override System.Collections.IDictionary Items
        {
            get
            {
                return items;
            }
        }

        public override IHttpHandler PreviousHandler
        {
            get
            {
                return new MvcHandler(new RequestContext());
            }
        }
        public override HttpRequestBase Request
        {
            get
            {
                return mockHttpRequestBase;
            }
        }

        public override HttpResponseBase Response
        {
            get { return new MockResponse(); }
        }

        private class MockHttpRequestBase : HttpRequestBase
        {
            private readonly string appRelativeUrl;
            private readonly NameValueCollection query;
            private readonly string queryString;

            public MockHttpRequestBase(string appRelativeUrl)
            {
                this.query = new NameValueCollection();
                this.queryString = appRelativeUrl.After("?");
                if (!string.IsNullOrEmpty(queryString))
                    queryString.Split('&').Do(pt => this.query.Add(pt.UpTo("="), pt.After("=")));

                this.appRelativeUrl = appRelativeUrl.UpTo("?");
            }

            public override string AppRelativeCurrentExecutionFilePath
            {
                get { return appRelativeUrl; }
            }

            public override string PathInfo
            {
                get { return ""; }
            }

            public override string ApplicationPath
            {
                get { return "/"; }
            }

            public override NameValueCollection QueryString
            {
                get
                {
                    return query;
                }
            }

            public override Uri Url
            {
                get
                {
                    return new Uri("http://localhost/"
                        + appRelativeUrl.Replace("~/", "")
                        + (string.IsNullOrEmpty(queryString) ? "" : "?" + queryString));
                }
            }

        }

        private class MockResponse : HttpResponseBase
        {
            public override string ApplyAppPathModifier(string virtualPath)
            {
                return virtualPath;
            }
        }
    }
}
