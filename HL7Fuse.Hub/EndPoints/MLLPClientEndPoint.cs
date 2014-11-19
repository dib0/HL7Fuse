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
    internal class MLLPClientEndPoint : BaseEndPoint
    {
        #region Protected properties
        protected SimpleMLLPClient mllpClient;
        #endregion

        #region Public properties
        public SimpleMLLPClient MllpClient
        {
            get
            {
                return mllpClient;
            }
        }
        #endregion

        #region Constructor
        protected MLLPClientEndPoint()
        {
        }

        public MLLPClientEndPoint(string hostname, int port, string serverCommunicationName, string serverEnvironment)
        {
            mllpClient = new SimpleMLLPClient(hostname, port);
            ServerCommunicationName = serverCommunicationName;
            ServerEnvironment = serverEnvironment;
        }
        #endregion

        #region Public methods
        public override bool Send(IMessage msg)
        {
            EditMessageHeader(msg);
            IMessage response = mllpClient.SendHL7Message(msg);

            Terser terser = new Terser(response);
            string ackCode = terser.Get("/MSA-1");

            return (ackCode == "AA");
        }
        #endregion
    }
}
