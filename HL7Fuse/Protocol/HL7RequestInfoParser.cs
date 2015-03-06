using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase.Protocol;
using NHapi.Base.Parser;
using NHapiTools.Base.Parser;
using NHapiTools.Base.Validation;
using NHapi.Base.Model;
using System.Configuration;

namespace HL7Fuse.Protocol
{
    public class HL7RequestInfoParser : IRequestInfoParser<HL7RequestInfo>
    {
        #region Private properties
        private bool HandleEachMessageAsEvent
        {
            get
            {
                bool result = false;
                if (!bool.TryParse(ConfigurationManager.AppSettings["HandleEachMessageAsEvent"], out result))
                    result = false;

                return result;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Parse the message to a RequestInfo class
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public HL7RequestInfo ParseRequestInfo(string message)
        {
            return ParseRequestInfo(message, string.Empty);
        }

        public HL7RequestInfo ParseRequestInfo(string message, string protocol)
        {
            HL7RequestInfo result = new HL7RequestInfo();
            PipeParser parser = new PipeParser();

            try
            {
                ConfigurableContext configContext = new ConfigurableContext(parser.ValidationContext);
                parser.ValidationContext = configContext;
            }
            catch
            {
                // Ignore any error, since the config is probably missing
            }

            IMessage hl7Message = parser.Parse(message);

            result = new HL7RequestInfo();
            if (HandleEachMessageAsEvent)
                result.Key = "V" + hl7Message.Version.Replace(".", "") + "." + hl7Message.GetStructureName();
            else
                result.Key = "V" + hl7Message.Version.Replace(".", "") + ".MessageFactory";

            if (!string.IsNullOrEmpty(protocol))
                result.Key += protocol;

            result.Message = hl7Message;

            // Parse the message
            return result;
        }
        #endregion
    }
}
