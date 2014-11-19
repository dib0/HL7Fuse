using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHapi.Base.Model;
using NHapi.Base.Util;
using NHapiTools.Base.Net;

namespace HL7Fuse.Hub.EndPoints
{
    internal class SSLClientEndPoint : MLLPClientEndPoint
    {
        #region Constructor
        public SSLClientEndPoint(string hostname, int port, string serverCommunicationName, string serverEnvironment) : this(hostname, port, serverCommunicationName, serverEnvironment, null, null)
        {  }

        public SSLClientEndPoint(string hostname, int port, string serverCommunicationName, string serverEnvironment, string pathToCertificate, string certificatePassword)
        {
            mllpClient = new SimpleMLLPClient(hostname, port);

            // Add client certificate authentication if needed
            if (!string.IsNullOrEmpty(pathToCertificate) && !string.IsNullOrEmpty(certificatePassword))
                mllpClient.AddCertificate(pathToCertificate, certificatePassword);
            mllpClient.EnableSsl();

            ServerCommunicationName = serverCommunicationName;
            ServerEnvironment = serverEnvironment;
        }
        #endregion
    }
}
