using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using NHapi.Base.Model;
using NHapi.Base.Util;

namespace HL7Fuse.Hub.EndPoints
{
    public class CustomEndPoint : BaseEndPoint
    {
        #region Virtual methods
        public virtual void Setup(XmlNodeList config)
        {
        }
        #endregion
    }
}
