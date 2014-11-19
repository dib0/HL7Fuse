using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NHapi.Base.Model;
using NHapi.Base.Util;
using NHapi.Base.Parser;
using HL7Fuse.Logging;
using System.Configuration;

namespace HL7Fuse.Hub.EndPoints
{
    internal class HttpEndPoint : BaseEndPoint
    {
        #region Private properties
        private string serverUri = string.Empty;
        #endregion

        #region Public properties
        public bool IgnoreSSLErrors
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public HttpEndPoint(string uri, string serverCommunicationName, string serverEnvironment, bool ignoreSSLErrors)
        {
            serverUri = uri;
            ServerCommunicationName = serverCommunicationName;
            ServerEnvironment = serverEnvironment;
            IgnoreSSLErrors = ignoreSSLErrors;
        }
        #endregion

        #region Public methods
        public override bool Send(IMessage msg)
        {
            bool result = true; 
            EditMessageHeader(msg);

            try
            {
                PipeParser parser = new PipeParser();
                string res = SendRequest(parser.Encode(msg));
                IMessage response = parser.Parse(res);

                Terser terser = new Terser(response);
                string ackCode = terser.Get("/MSA-1");
                result = (ackCode == "AA");
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("Error in delivering message '{0}' to Http endpoint with uri '{1}'. Error: {2}", msg.GetStructureName(), serverUri, ex.Message);
                result = false;
            }

            return result;
        }
        #endregion

        #region Private methods
        private string SendRequest(string message)
        {
            return SendRequest(message, "application/hl7-v2");
        }

        private string SendRequest(string message, string content)
        {
            HttpWebRequest request = WebRequest.CreateHttp(serverUri);
            if (IgnoreSSLErrors)
            {
                request.ServerCertificateValidationCallback +=
                delegate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                        System.Security.Cryptography.X509Certificates.X509Chain chain,
                                        System.Net.Security.SslPolicyErrors sslPolicyErrors)
                {
                    return true; // **** Always accept
                };
            }

            request.Method = "POST";
            request.ContentType = content;
            // Send the message
            StreamWriter ts = new StreamWriter(request.GetRequestStream());
            ts.Write(message);
            ts.Flush();

            // Get the result
            HttpWebResponse result;
            try
            {
                result = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException webEx)
            {
                result = (HttpWebResponse)webEx.Response;
            }

            string replyMessage = string.Empty;
            using (StreamReader sr = new StreamReader(result.GetResponseStream()))
                replyMessage = sr.ReadToEnd();

            return replyMessage;
        }
        #endregion
    }
}
