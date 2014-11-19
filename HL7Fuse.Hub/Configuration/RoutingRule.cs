using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NHapi.Base.Model;

namespace HL7Fuse.Hub.Configuration
{
    class RoutingRule
    {
        #region Public properties
        public string Hl7Version
        {
            get;
            set;
        }

        public string Structurename
        {
            get;
            set;
        }

        public bool Include
        {
            get;
            set;
        }
        #endregion

        #region Public methods
        public bool Match(IMessage message)
        {
            bool result = false;

            result = Compare(Hl7Version, message.Version);
            if (result)
                result = Compare(Structurename, message.GetStructureName());
            
            return result;
        }
        #endregion

        #region Private methods
        private bool Compare(string pattern, string compareTo)
        {
            bool result = false;
            if (pattern.Contains('*') || pattern.Contains('?'))
                result = WildcardStringCompare(pattern, compareTo);
            else
                result = (pattern == compareTo);

            return result;
        }

        public bool WildcardStringCompare(string pattern, string compareTo)
        {
            string patt = "^" + Regex.Escape(pattern).
                               Replace(@"\*", ".*").
                               Replace(@"\?", ".") + "$";
            Regex regex = new Regex(patt);

            return regex.IsMatch(compareTo);
        }
        #endregion
    }
}
