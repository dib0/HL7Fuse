using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase.Protocol;
using NHapi.Base.Model;

namespace HL7Fuse.Protocol
{
    public class HL7RequestInfo : IRequestInfo
    {
        public string Key
        {
            get;
            set;
        }

        public IMessage Message
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }

        public bool WasUnknownRequest
        {
            get;
            set;
        }

        public bool HasError
        {
            get
            {
                return (ErrorMessage != null);
            }
        }

        /// <summary>
        /// By default an (N)ACK message will be sent (unsollicited communication pattern).
        /// If you, however, need to return some other message (like in a sollicited communication pattern
        /// you can do this by filling the ResponseMessage with the response you want to send.
        /// </summary>
        public IMessage ResponseMessage
        {
            get;
            set;
        }
    }
}
