using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NHapi.Base.Model;
using NHapi.Base.Util;
using NHapiTools.Base.Net;
using NHapi.Base.Parser;

namespace HL7Fuse.Hub.EndPoints
{
    internal class FileEndPoint : BaseEndPoint
    {
        #region Private properties
        private string outputDir;
        #endregion

        #region Public properties
        #endregion

        #region Constructor
        public FileEndPoint(string targetDir)
        {
            outputDir = targetDir;
            if (!outputDir.EndsWith("\\") || !outputDir.EndsWith("/"))
            {
                if (outputDir.Contains("\\"))
                    outputDir += "\\";
                else
                    outputDir += "/";
            }
        }
        #endregion

        #region Public methods
        public override bool Send(IMessage msg)
        {
            try
            {
                string path = outputDir + GetFileName(msg);
                using (StreamWriter sw = new StreamWriter(File.OpenWrite(path)))
                {
                    PipeParser parser = new PipeParser();
                    sw.Write(parser.Encode(msg));
                }
            }
            catch (IOException)
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Private methods
        private string GetFileName(IMessage msg)
        {
            // Format filename as yyyyMMDD_HHmmSS_EVENTNAME.HL7
            return string.Format("{0}_{1}_{2}.HL7", DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmssfff"), msg.GetStructureName());
        }
        #endregion
    }
}
