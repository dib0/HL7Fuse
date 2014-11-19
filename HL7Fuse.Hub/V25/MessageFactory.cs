using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7Fuse.Hub.V25
{
    /// <summary>
    /// Message factory
    /// </summary>
    public class MessageFactory : MessageFactoryBase
    {
        #region Public properties
        public override string Name
        {
            get { return "V25.MessageFactory"; }
        }
        #endregion
    }
}
