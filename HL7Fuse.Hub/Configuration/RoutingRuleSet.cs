using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using NHapi.Base.Model;

namespace HL7Fuse.Hub.Configuration
{
    class RoutingRuleSet
    {
        #region Private properties
        private List<RoutingRule> rules;
        #endregion

        #region Public properties
        public string EndPoint
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public RoutingRuleSet(XmlNodeList childnodes)
        {
            rules = new List<RoutingRule>();

            foreach (XmlNode node in childnodes)
            {
                RoutingRule rule = new RoutingRule();
                rule.Include = (node.Name == "include");
                rule.Hl7Version = node.Attributes["hl7Version"].Value;
                rule.Structurename = node.Attributes["structurename"].Value;
                rules.Add(rule);

            }
        }
        #endregion

        #region Public methods
        public bool IncludeEndpoint(IMessage message)
        {
            bool result = true;

            // Run include rules
            List<RoutingRule> incRules = rules.Where(r => r.Include).ToList();
            foreach (RoutingRule rule in incRules)
            {
                // If any include rules match, this endpoint is applicable
                if (rule.Match(message))
                    result = true;
            }

            if (result)
            {
                // Run exclude rules
                List<RoutingRule> exclRules = rules.Where(r => !r.Include).ToList();
                foreach (RoutingRule rule in exclRules)
                {
                    // If any exclude rules match, this endpoint is not applicable
                    if (rule.Match(message))
                        result = false;
                }
            }

            return result;
        }
        #endregion
    }
}
