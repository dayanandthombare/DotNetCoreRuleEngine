using RuleEngine.Core.Interfaces;
using RuleEngine.Core.Models;
using System;
using System.Xml.Linq;

namespace RuleEngine.Definitions.Parser
{
    public class XmlRuleParser : IRuleParser
    {
        public List<Rule> ParseRules(string rulesPath)
        {
            var rulesList = new List<Rule>();
            var rulesDoc = XDocument.Load(rulesPath);

            foreach (var ruleElement in rulesDoc.Descendants("Rule"))
            {
                var rule = new Rule
                {
                    
                    Id = ruleElement.Attribute("id")?.Value,
                    TemplateA = ruleElement.Element("TemplateA")?.Value,
                    TemplateB = ruleElement.Element("TemplateB")?.Value,
                    ConditionType = ruleElement.Element("Condition")?.Attribute("type")?.Value
                };

                rulesList.Add(rule);
            }

            return rulesList;
        }
    }
}
