using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Xml;
using HL7Fuse.Hub.EndPoints;

namespace HL7Fuse.Hub.Configuration
{
    class HL7RoutingRulesConfigurationHandler : IConfigurationSectionHandler
    {
        #region Public methods
        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            List<RoutingRuleSet> result = new List<RoutingRuleSet>();

            foreach (XmlNode node in section.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Comment)
                {
                    if (node.Name.ToLower() == "rule")
                    {
                        result.Add(GetRoutingRule(node));
                    }
                }
            }

            return result;
        }
        #endregion

        #region Private methods
        private RoutingRuleSet GetRoutingRule(XmlNode node)
        {
            RoutingRuleSet rule = new RoutingRuleSet(node.ChildNodes);
            rule.EndPoint = node.Attributes["endpoint"].Value;
            rule.RouteOnValidRules = node.Attributes["routeOnValidRules"].Value;

            return rule;
        }
        #endregion
    }
}
