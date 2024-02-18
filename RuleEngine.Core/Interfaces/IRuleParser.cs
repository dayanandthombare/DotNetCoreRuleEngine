using RuleEngine.Core.Models;

namespace RuleEngine.Core.Interfaces
{
    public interface IRuleParser
    {
        List<Rule> ParseRules(string rulesPath);
    }
}
