using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHapi.Base.Model;
using NHapiTools.Base.Net;

namespace HL7Fuse.Hub
{
    internal interface IEndPoint
    {
        bool Send(IMessage msg);
    }
}
