using RuleEngine.Core.Models;
using System.Xml.Linq;

namespace RuleEngine.Core.Interfaces
{
    public interface IXmlRuleEngine
    {
        bool ValidateDocument(XDocument document, Models.Rule rule);
    }
}
