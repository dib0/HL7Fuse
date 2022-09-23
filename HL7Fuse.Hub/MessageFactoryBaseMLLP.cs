using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase.Command;
using HL7Fuse.Services.Protocol;
using HL7Fuse.Services;

namespace HL7Fuse.Hub
{
    public class MessageFactoryBaseMLLP : ICommand<MLLPSession, HL7RequestInfo>
    {
        #region Public properties
        public virtual string Name
        {
            get { return string.Empty; }
        }
        #endregion

        #region Public methods
        public void ExecuteCommand(MLLPSession session, HL7RequestInfo requestInfo)
        {
            // Handle event
            ConnectionManager.Instance.SendMessage(requestInfo.Message);
        }
        #endregion
    }
}
