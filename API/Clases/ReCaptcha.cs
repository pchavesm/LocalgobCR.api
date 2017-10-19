using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace MPZ_API.Clases
{
    [DataContract]
    public class ReCaptcha
    { 
        [DataMember(Name = "success")]
        public bool Success { get; set; }
        [DataMember(Name = "error-codes")]
        public List<string> ErrorCodes { get; set; }



        
       /*
        public static string Validate(string EncodedResponse)
        {
            var client = new System.Net.WebClient();

            string PrivateKey = "6Ld8zRsUAAAAAOvXU2EvQCFZYiYYL4YbNTLdGP7I";

            var GoogleReply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", PrivateKey,EncodedResponse));

            var captchaResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ReCaptcha>(GoogleReply);

            return captchaResponse.Success;
        }

        [JsonProperty("success")]
        public string Success
        {
            get { return m_Success; }
            set { m_Success = value; }
        }

        private string m_Success;
        [JsonProperty("error-codes")]
        public List<string> ErrorCodes
        {
            get { return m_ErrorCodes; }
            set { m_ErrorCodes = value; }
        }


        private List<string> m_ErrorCodes;*/

    }
}