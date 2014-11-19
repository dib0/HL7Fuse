using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Xml;
using HL7Fuse.Hub.EndPoints;
using System.Reflection;

namespace HL7Fuse.Hub.Configuration
{
    class EndPointConfigurationHandler : IConfigurationSectionHandler
    {
        #region Public methods
        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            Dictionary<string, IEndPoint> result = new Dictionary<string, IEndPoint>();

            foreach (XmlNode node in section.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Comment)
                {
                    switch (node.Name)
                    {
                        case "MLLPClientEndPoint":
                            result.Add(node.Attributes["name"].Value, GetMLLPClientEndPoint(node));
                            break;
                        case "FileEndpoint":
                            result.Add(node.Attributes["name"].Value, GetFileEndPoint(node));
                            break;
                        case "HttpEndPoint":
                            result.Add(node.Attributes["name"].Value, GetHttpEndPoint(node));
                            break;
                        case "SSLEndPoint":
                            result.Add(node.Attributes["name"].Value, GetSSLEndPoint(node));
                            break;
                        case "CustomEndPoint":
                            result.Add(node.Attributes["name"].Value, GetCustomEndPoint(node));
                            break;
                        default:
                            throw new Exception("Invalid endpoint name in Endpoints section.");
                    }
                }
            }

            return result;
        }
        #endregion

        #region Private methods
        private IEndPoint GetMLLPClientEndPoint(XmlNode node)
        {
            string host = node.Attributes["host"].Value;
            int port = int.Parse(node.Attributes["port"].Value);
            string serverCommunicationName = node.Attributes["serverCommunicationName"].Value;
            string serverEnvironment = node.Attributes["serverEnvironment"].Value;

            return new MLLPClientEndPoint(host, port, serverCommunicationName, serverEnvironment);
        }

        private IEndPoint GetFileEndPoint(XmlNode node)
        {
            string target = node.Attributes["targetDirectory"].Value;

            return new FileEndPoint(target);
        }

        private IEndPoint GetHttpEndPoint(XmlNode node)
        {
            string host = node.Attributes["serverUri"].Value;
            string serverCommunicationName = node.Attributes["serverCommunicationName"].Value;
            string serverEnvironment = node.Attributes["serverEnvironment"].Value;
            bool ignoreSSLErrors = false;
            if (!bool.TryParse(node.Attributes["acceptAllSSlCertificates"].Value, out ignoreSSLErrors))
                ignoreSSLErrors = false;

            return new HttpEndPoint(host, serverCommunicationName, serverEnvironment, ignoreSSLErrors);
        }

        private IEndPoint GetSSLEndPoint(XmlNode node)
        {
            string host = node.Attributes["host"].Value;
            int port = int.Parse(node.Attributes["port"].Value);
            string serverCommunicationName = node.Attributes["serverCommunicationName"].Value;
            string serverEnvironment = node.Attributes["serverEnvironment"].Value;

            string pathToCertificate = node.Attributes["clientSideCertificatePath"].Value;
            string certPassword = node.Attributes["clientSideCertificatePassword"].Value;

            SSLClientEndPoint result = null;
            if (string.IsNullOrEmpty(pathToCertificate) && string.IsNullOrEmpty(certPassword))
                result = new SSLClientEndPoint(host, port, serverCommunicationName, serverEnvironment);
            else
                result = new SSLClientEndPoint(host, port, serverCommunicationName, serverEnvironment, pathToCertificate, certPassword);

            return result;
        }

        private IEndPoint GetCustomEndPoint(XmlNode node)
        {
            string customType = node.Attributes["type"].Value;
            string[] cType = customType.Split(',');

            if (cType.Count() != 2)
                throw new Exception("Invalid type definition for custom end point in the configuration file.");

            CustomEndPoint result = (CustomEndPoint) Activator.CreateInstance(cType[1], cType[0]).Unwrap();
            // Run the setup
            result.Setup(node.ChildNodes);

            return result;
        }
        #endregion
    }
}
