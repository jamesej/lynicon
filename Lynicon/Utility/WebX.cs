using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Web.UI;
using System.Web;
using System.Collections;
using System.Net;
using System.IO;
using System.Web.Configuration;
using System.Web.Security;
using System.Configuration;
using System.Web.Caching;

namespace Lynicon.Utility
{
    /// <summary>
    /// Utilities for making requests
    /// </summary>
    public static class WebX
    {
        public const string dummyPageKey = "_L24DummyPage";

        /// <summary>
        /// Set basic authentication on a WebRequest
        /// </summary>
        /// <param name="req">The web request</param>
        /// <param name="user">The user name</param>
        /// <param name="pass">The password</param>
        public static void SetBasicAuth(this WebRequest req, string user, string pass)
        {
            string authInfo = user + ":" + pass;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            req.Headers["Authorization"] = "Basic " + authInfo;
        }

        /// <summary>
        /// Get the response of a web request as a string
        /// </summary>
        /// <param name="req">The web request</param>
        /// <returns>The resposne</returns>
        public static string GetResponseString(this HttpWebRequest req)
        {
            return new StreamReader(req.GetResponse().GetResponseStream()).ReadToEnd();
        }

        /// <summary>
        /// Get the input stream of a request as a string
        /// </summary>
        /// <param name="req">The request</param>
        /// <returns>The input stream as a string</returns>
        public static string GetRequestString(HttpRequestBase req)
        {
            string documentContents;
            using (Stream receiveStream = req.InputStream)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    documentContents = readStream.ReadToEnd();
                }
            }
            return documentContents;
        }

        /// <summary>
        /// Make a post request from a url and a post body
        /// </summary>
        /// <param name="url">The url for the request</param>
        /// <param name="body">The body of the post</param>
        /// <returns>The web request</returns>
        public static HttpWebRequest GetPostRequest(string url, string body)
        {
            return GetPostRequest(url, body, null);
        }
        /// <summary>
        /// Make a post request from a url and a post body
        /// </summary>
        /// <param name="url">The url for the request</param>
        /// <param name="body">The body of the post</param>
        /// <param name="setReq">Action method to alter the request</param>
        /// <returns>The web request</returns>
        public static HttpWebRequest GetPostRequest(string url, string body, Action<HttpWebRequest> setReq)
        {
            return GetPostRequest(url, body, setReq, null);
        }
        /// <summary>
        /// Make a post request from a url and a post body
        /// </summary>
        /// <param name="url">The url for the request</param>
        /// <param name="body">The body of the post</param>
        /// <param name="setReq">Action method to alter the request</param>
        /// <param name="encoding">The encoding of the request</param>
        /// <returns>The web request</returns>
        public static HttpWebRequest GetPostRequest(string url, string body, Action<HttpWebRequest> setReq, Encoding encoding)
        {
            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = body.Length;
            if (setReq != null)
                setReq(req);
            StreamWriter stOut = new StreamWriter(req.GetRequestStream(), encoding ?? System.Text.Encoding.ASCII);
            stOut.Write(body);
            stOut.Close();
            return req;
        }
        /// <summary>
        /// Make a post request from a url and a post body
        /// </summary>
        /// <param name="url">The url for the request</param>
        /// <param name="postVals">List of values to put in the post body</param>
        /// <returns>The web request</returns>
        public static HttpWebRequest GetPostRequest(string url, Dictionary<string, string> postVals)
        {
            return GetPostRequest(url,
                postVals.Select(kvp => kvp.Key + "=" + HttpUtility.UrlEncode(kvp.Value))
                    .Join("&"));
        }

        /// <summary>
        /// Make a get request
        /// </summary>
        /// <param name="url">The url</param>
        /// <returns>The response from the request</returns>
        public static string MakeGetRequest(string url)
        {
            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            req.Method = "GET";
            return req.GetResponseString();
        }

        /// <summary>
        /// Get the external IP of the current server
        /// </summary>
        /// <returns>The external IP</returns>
        public static string GetExternalIp()
        {
            return MakeGetRequest("http://checkip.dyndns.org").After(":").UpTo("<").Trim();
        }

        /// <summary>
        /// Get the best signature for the given request
        /// </summary>
        /// <param name="req">Request</param>
        /// <returns>A string which is as unique as possible identifying the request</returns>
        public static string GetSignature(HttpRequestBase req)
        {
            return req.UserHostAddress + req.UserAgent;
        }

        /// <summary>
        /// Store a value in the asp.net cache, initialising with a given function, for a given duration
        /// </summary>
        /// <typeparam name="T">Type of object to store</typeparam>
        /// <param name="key">The cache key</param>
        /// <param name="initialise">The function to set the initial value</param>
        /// <param name="seconds">The time to save the value in the cache</param>
        /// <returns>The cached/initialised value of the object</returns>
        public static T GetItemWithCaching<T>(string key, Func<T> initialise, int seconds)
        {
            var item = (T)HttpContext.Current.Cache.Get(key);
            if (item == null)
            {
                item = initialise();
                HttpContext.Current.Cache.Add(key, item, null, DateTime.Now.AddSeconds(seconds), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            }
            return item;
        }
    }
}
