using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NHapi.Base.Model;
using NHapi.Base.Util;

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

        public string FieldFilter
        {
            get;
            set;
        }

        public string FieldFilterValue
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

            // Check the field filter
            if (result)
                result = CheckFieldFilter(message);
            
            return result;
        }
        #endregion

        #region Private methods
        private bool CheckFieldFilter(IMessage message)
        {
            bool result = true;
            if (string.IsNullOrWhiteSpace(FieldFilter) && string.IsNullOrWhiteSpace(FieldFilterValue))
                return result;

            Terser terser = new Terser(message);
            string msgFieldValue = terser.Get(FieldFilter);
            result = Compare(FieldFilterValue, msgFieldValue);

            return result;
        }

        private bool Compare(string pattern, string compareTo)
        {
            bool result = false;
            if (pattern == null)
                pattern = string.Empty;
            if (compareTo == null)
                compareTo = string.Empty;

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
