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
        private RuleValidationMethod validationMethod;
        #endregion

        #region Public properties
        public string RouteOnValidRules
        {
            get
            {
                return Enum.GetName(typeof(RuleValidationMethod), validationMethod);
            }
            set
            {
                switch (value.ToLower())
                {
                    case "all" :
                        validationMethod = RuleValidationMethod.All;
                        break;
                    case "any" :
                        validationMethod = RuleValidationMethod.Any;
                        break;
                    default :
                        throw new Exception(string.Format("Unknown value {0} for Rule validation method.", value));
                }
            }
        }

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
                if (node.NodeType != XmlNodeType.Comment)
                {
                    RoutingRule rule = new RoutingRule();
                    rule.Include = (node.Name == "include");
                    rule.Hl7Version = node.Attributes["hl7Version"].Value;
                    rule.Structurename = node.Attributes["structurename"].Value;

                    if (node.Attributes["fieldFilter"] != null)
                    {
                        if (node.Attributes["fieldFilterValue"] == null)
                            throw new ArgumentNullException("If fieldFilter is used in a routing rule, the fieldFilterValue must be set.");

                        rule.FieldFilter = node.Attributes["fieldFilter"].Value;
                        rule.FieldFilterValue = node.Attributes["fieldFilterValue"].Value;
                    }

                    rules.Add(rule);
                }
            }
        }
        #endregion

        #region Public methods
        public bool IncludeEndpoint(IMessage message)
        {
            bool result = false;
            if (validationMethod == RuleValidationMethod.All)
                result = true;

            // Run include rules
            List<RoutingRule> incRules = rules.Where(r => r.Include).ToList();
            foreach (RoutingRule rule in incRules)
            {
                if (validationMethod == RuleValidationMethod.All)
                {
                    // If all include rules match, this endpoint is applicable
                    if (!rule.Match(message))
                        result = false;
                }
                else
                {
                    // If any include rules match, this endpoint is applicable
                    if (rule.Match(message))
                        result = true;
                }
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
