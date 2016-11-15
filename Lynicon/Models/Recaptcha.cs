using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lynicon.Config;
using System.Configuration;
using System.Net;
using Lynicon.Utility;
using System.Web;
using System.Web.Mvc;
using Lynicon.Binding;

namespace Lynicon.Models
{
    [ModelBinder(typeof(RecaptchaBinder))]
    public class Recaptcha
    {
        private CaptchaElement Config
        {
            get
            {
                LyniconSection lyniconSection = ConfigurationManager.GetSection("l24CM/basic") as LyniconSection;
                return lyniconSection.LyniconCaptcha;
            }
        }

        public string PublicKey
        {
            get { return Config.PublicKey; }
        }

        // Model binding fills these in
        public string recaptcha_challenge_field { get; set; }
        public string recaptcha_response_field { get; set; }

        public bool Verify()
        {
            string remoteip = HttpContext.Current.Request.UserHostAddress;
            // for testing, local request
            if (remoteip == "127.0.0.1")
                remoteip = WebX.GetExternalIp();

            HttpWebRequest req = WebX.GetPostRequest(Config.VerifyUrl,
                new Dictionary<string, string> {
                    { "privatekey", Config.PrivateKey },
                    { "remoteip", remoteip },
                    { "challenge", recaptcha_challenge_field },
                    { "response", recaptcha_response_field }
                });

            string resp = req.GetResponseString();
            bool success = (resp.UpTo("\n").Trim().ToLower() == "true");
            if (!success)
                errorCode = resp.After("\n").Trim();
            return success;
        }

        private string errorCode = null;
        public string GetError()
        {
            return errorCode;
        }
    }
}
