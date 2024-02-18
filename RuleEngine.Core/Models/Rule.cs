using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleEngine.Core.Models
{
    public class Rule
    {
        public string? Id { get; set; }
        public string? TemplateA { get; set; }
        public string? TemplateB { get; set; }
        public string? TemplateC { get; set; }
        public string? ConditionType { get; set; }
    }
}
