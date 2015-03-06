using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHapiTools.Base.Util;
using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase.Protocol;

namespace HL7Fuse.Protocol
{
    public class MLLPBeginEndMarkReceiveFilter : BeginEndMarkReceiveFilter<HL7RequestInfo>
    {
        //Both begin mark and end mark can be two or more bytes
        private readonly static byte[] BeginMark = new byte[] { 11 }; // HEX 0B
        private readonly static byte[] EndMark = new byte[] { 28, 13 }; // HEX 1C, 0D

        public MLLPBeginEndMarkReceiveFilter()
            : base(BeginMark, EndMark) //pass in the begin mark and end mark
        {

        }

        protected override HL7RequestInfo ProcessMatchedRequest(byte[] readBuffer, int offset, int length)
        {
            byte[] msg = new byte[length];
            Array.Copy(readBuffer, offset, msg, offset, length);
            
            string result = System.Text.UTF8Encoding.UTF8.GetString(msg);
            // Remove the begin and end marks
            if (result.Length > 3)
            {
                StringBuilder sb = new StringBuilder(result);
                MLLP.StripMLLPContainer(sb);

                result = sb.ToString();
            }
            // Remove empty space at the end of the message
            result = result.TrimEnd(new char[]{' ', '\r', '\n'});

            HL7RequestInfoParser parser = new HL7RequestInfoParser();
            return parser.ParseRequestInfo(result, "MLLP");
        }
    }
}
