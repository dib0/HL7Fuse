using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHapi.Base.Model;

namespace HL7Fuse.Hub.Handling
{
    /// <summary>
    /// Interface to provide handling or alteration of HL7 messages before
    /// the message is send to one or more endpoints.
    /// </summary>
    interface IMessageHandler
    {
        IMessage HandleMessage(IMessage message);
    }
}
