using RuleEngine.Core.Helpers;
using RuleEngine.Core.Interfaces;
using System.Xml.Linq;
using RuleEngine.Core.Models;
using Microsoft.Extensions.Options;

namespace RuleEngine.Core.Services
{
    public class XmlRuleEngine : IXmlRuleEngine
    {
        private readonly IRuleParser _ruleParser;
        private List<Models.Rule> _rules;
        private readonly string? _rulesPath;
        public XmlRuleEngine(IRuleParser ruleParser, IOptions<RuleEngineSettings> settings)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo directoryInfo = new(currentDirectory);
            string? targetDirectory = directoryInfo.Parent?.FullName;
            _rulesPath = settings.Value.RulesPath;
            var fullPath = Path.Combine(targetDirectory, _rulesPath);
            _ruleParser = ruleParser;
            _rules = _ruleParser.ParseRules(fullPath);
        }
        public virtual bool ValidateDocument(XDocument document, Models.Rule? ruleAsParam)
        {
            var _applicableRules = ruleAsParam != null ?
                          _rules.Where(x => x.ConditionType == ruleAsParam.ConditionType).ToList() :
                          _rules;

            foreach (var rule in _applicableRules)
            {
                switch (rule.ConditionType)
                {
                    case "ChildSiblingRestriction":
                        if (!ValidateChildSiblingRestriction(document, rule)) return false;
                        break;
                    case "CousinRequirement":
                        if (!ValidateCousinRequirement(document, rule)) return false;
                        break;
                    case "SequentialRequirement":
                        if (!ValidateSequentialRequirement(document, rule)) return false;
                        break;
                    case "GlobalRestriction":
                        if (!ValidateGlobalRestriction(document, rule)) return false;
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown rule condition type: {rule.ConditionType}");
                }
            }
            return true;
        }

        private bool ValidateChildSiblingRestriction(XDocument document, Models.Rule rule)
        {
            var templateANodes = document.Descendants().Where(e => XmlHelper.IsTemplateMatch(e, rule.TemplateA));
            foreach (var nodeA in templateANodes)
            {
                var siblingElements = XmlHelper.GetAllSiblings(nodeA);
                if (siblingElements.Any(sibling => XmlHelper.IsTemplateMatch(sibling, rule.TemplateB)))
                {
                    return false;
                }
            }
            return true;
        }
        private bool ValidateCousinRequirement(XDocument document, Models.Rule rule)
        {
            foreach (var element in document.Descendants())
            {
                if (XmlHelper.IsTemplateMatch(element, rule.TemplateA))
                {
                    var cousins = XmlHelper.GetCousins(element);
                    if (!cousins.Any(cousin => XmlHelper.IsTemplateMatch(cousin, rule.TemplateB)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private bool ValidateSequentialRequirement(XDocument document, Models.Rule rule)
        {
            foreach (var parentElement in document.Descendants())
            {
                var children = parentElement.Elements().ToList();
                for (int i = 0; i < children.Count - 1; i++)
                {
                    if (XmlHelper.IsTemplateMatch(children[i], rule.TemplateA))
                    {
                        if (!XmlHelper.HasFollowingSiblingWithTemplate(children[i], rule.TemplateB))
                        {
                            return false;
                        }
                        break; 
                    }
                }
            }
            return true;
        }
        private bool ValidateGlobalRestriction(XDocument document, Models.Rule rule)
        {
            bool isTemplateAPresent = document.Descendants().Any(element => XmlHelper.IsTemplateMatch(element, rule.TemplateA));
            if (isTemplateAPresent)
            {
                bool isTemplateCPresent = document.Descendants().Any(element => XmlHelper.IsTemplateMatch(element, rule.TemplateC));
                return !isTemplateCPresent;
            }
            return true;
        }
    }
}

