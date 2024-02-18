using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using RuleEngine.API.Controllers;
using RuleEngine.Core.Interfaces;
using System.Text;
using System.Xml.Linq;
namespace RuleEngine.Test
{
    public class RuleEngineControllerTest
    {
        private RuleEngineController? _controller;
        private Mock<IRuleParser>? _mockRuleParser;
        private Mock<IXmlRuleEngine>? _mockXmlRuleEngine;

        [SetUp]
        public void Setup()
        {
            _mockRuleParser = new Mock<IRuleParser>();
            _mockXmlRuleEngine = new Mock<IXmlRuleEngine>(); 

           
            _controller = new RuleEngineController(_mockRuleParser.Object, _mockXmlRuleEngine.Object);
        }

        [Test]
        public void ValidateDocument_ShouldReturnOk_WhenDocumentIsValid()
        {
            // Arrange
            var xmlContent = "<root><validXml>true</validXml></root>";
            var fileName = "test.xml";
            var xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlContent));

            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(_ => _.FileName).Returns(fileName);
            mockFile.Setup(_ => _.OpenReadStream()).Returns(xmlStream);
            mockFile.Setup(_ => _.Length).Returns(xmlStream.Length);

           
            _mockXmlRuleEngine.Setup(engine => engine.ValidateDocument(It.IsAny<XDocument>(), null)).Returns(true);

            // Act
            var result = _controller.ValidateDocument(mockFile.Object);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var value = okResult.Value as dynamic;
            Assert.IsTrue(value.IsValid);
        }

        [Test]
        public void ValidateDocument_ShouldReturnBadRequest_WhenFileIsNull()
        {
            // Act
            var result = _controller.ValidateDocument(null);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
    }
}
