using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using RuleEngine.Core.Interfaces;
using Models = RuleEngine.Core.Models;
using RuleEngine.Core.Services;
using System.Xml.Linq;
using RuleEngine.Core.Models;

namespace RuleEngine.Test
{
    internal class XmlRuleEngineTests
    {
        private XmlRuleEngine? _xmlRuleEngine;
        private Mock<IRuleParser>? _mockRuleParser;
        private List<Rule>? _mockRules;


        [SetUp]
        public void Setup()
        {
            _mockRuleParser = new Mock<IRuleParser>();          
            var settings = Options.Create(new RuleEngineSettings
            {
                RulesPath = "ruleEngine.Definitions\\RuleDefinitions\\Rules.xml"
            });          
            _mockRules =
            [
                new Rule { TemplateA = "templateA", TemplateB = "templateB", ConditionType = "ChildSiblingRestriction" },
                new Rule { TemplateA = "templateA", TemplateB = "templateB", ConditionType = "CousinRequirement" },
                new Rule { TemplateA = "templateA", TemplateB = "templateB", ConditionType = "SequentialRequirement" },
                new Rule { TemplateA = "templateA", TemplateC = "templateC", ConditionType = "GlobalRestriction" }
            ];           
            _mockRuleParser.Setup(m => m.ParseRules(It.IsAny<string>())).Returns(_mockRules);          
            _xmlRuleEngine = new XmlRuleEngine(_mockRuleParser.Object, settings);
        }

        [Test]
        public void ValidateDocument_ShouldPass_AllRules()
        {
            string xmlString = @"
                <root>
                    <parent1>
                        <templateA />
                    </parent1>
                    <parent2>
                        <nodeA>
                            <templateA /> 
                        </nodeA>
                        <nodeB>
                            <templateB />
                        </nodeB>
                    </parent2>
                    <parent3>
                        <templateA/>
                        <templateB/>
                    </parent3>
                </root>";

            XDocument document = XDocument.Parse(xmlString);

            bool isValid = _xmlRuleEngine.ValidateDocument(document, null);
            Assert.IsTrue(isValid, "Expected document to be valid when all rules are satisfied.");

        }

        [Test]
        public void Should_Pass_When_When_ChildSiblingRestriction_Is_Met()
        {
            string xmlString = @"
                            <parent>
                                <templateA />
                                <templateC />
                            </parent>
                                ";

            XDocument document = XDocument.Parse(xmlString);

            bool isValid = _xmlRuleEngine.ValidateDocument(document, _mockRules?.Where(x => x.ConditionType == "ChildSiblingRestriction").FirstOrDefault());
            Assert.IsTrue(isValid);

        }

        [Test]
        public void Should_Pass_When_CousinRequirement_Is_Met()
        {
            var document = XDocument.Parse(@"  
                                            <parent>
                                                <nodeA>
                                                   <templateA />
                                                </nodeA>
    
                                                <nodeB>
                                                   <templateB />
                                                </nodeB> 
                                            </parent>
");

            var result = _xmlRuleEngine.ValidateDocument(document, _mockRules?.Where(x => x.ConditionType == "CousinRequirement").FirstOrDefault());
            Assert.IsTrue(result);
        }

        [Test]
        public void Should_Pass_When_SequentialRequirement_Is_Met()
        {
            var document = XDocument.Parse(@"
                                                <parent>
                                                    <templateA /> 
                                                    <templateB />
                                                </parent>
                                           ");

            var result = _xmlRuleEngine.ValidateDocument(document, _mockRules?.Where(x => x.ConditionType == "SequentialRequirement").FirstOrDefault());
            Assert.IsTrue(result);
        }

        [Test]
        public void Should_Pass_When_GlobalRestriction_Is_Met()
        {
            var document = XDocument.Parse(@"
                                            <document>
                                               <parent>
                                                  <templateX />
                                               </parent>
                                            </document>
                                           ");

            var result = _xmlRuleEngine.ValidateDocument(document, _mockRules?.Where(x => x.ConditionType == "GlobalRestriction").FirstOrDefault());
            Assert.IsTrue(result);
        }
    }
}
