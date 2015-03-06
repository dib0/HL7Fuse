using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7Fuse.Hub.V24
{
    /// <summary>
    /// Message factory
    /// </summary>
    public class MessageFactoryFile : MessageFactoryBaseFile
    {
        #region Public properties
        public override string Name
        {
            get { return "V24.MessageFactoryFile"; }
        }
        #endregion
    }
}
