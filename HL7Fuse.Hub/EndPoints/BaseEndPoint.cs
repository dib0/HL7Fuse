using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHapi.Base.Model;
using NHapi.Base.Util;

namespace HL7Fuse.Hub.EndPoints
{
    public class BaseEndPoint : IEndPoint
    {
        #region Public properties
        public string ServerCommunicationName
        {
            get;
            set;
        }

        public string ServerEnvironment
        {
            get;
            set;
        }
        #endregion

        #region Virtual methods
        public virtual bool Send(IMessage msg)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Protected methods
        protected void EditMessageHeader(IMessage msg)
        {
            Terser terser = new Terser(msg);
            terser.Set("/MSH-3", ConfigurationManager.AppSettings["CommunicationName"]);
            terser.Set("/MSH-4", ConfigurationManager.AppSettings["EnvironmentIdentifier"]);
            terser.Set("/MSH-5", ServerCommunicationName);
            terser.Set("/MSH-6", ServerEnvironment);
        }
        #endregion
    }
}
