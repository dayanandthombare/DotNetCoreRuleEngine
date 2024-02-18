using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using RuleEngine.Core.Interfaces;
using RuleEngine.Core.Services;
using System.ComponentModel.DataAnnotations;
using DTO = RuleEngine.API.DTOs;


namespace RuleEngine.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RuleEngineController : ControllerBase
    {
        private readonly IRuleParser _ruleParser;
        private readonly IXmlRuleEngine _ruleEngine;
        public RuleEngineController(IRuleParser ruleParser, IXmlRuleEngine ruleEngine)
        {
            _ruleParser = ruleParser;
            _ruleEngine = ruleEngine;
        }

        [HttpPost("validate")]
        public IActionResult ValidateDocument([FromForm] IFormFile xmlDocument)
        {
            if (xmlDocument == null)
            {
                return BadRequest("Invalid request. Please provide both an XML document and a rules path.");
            }
            try
            {
                XDocument document;
                using (var stream = xmlDocument.OpenReadStream())
                {
                    document = XDocument.Load(stream);
                }
                var isValid = _ruleEngine.ValidateDocument(document, null);

                return Ok(new DTO.ValidationResult { IsValid = isValid });
            }
            catch (System.Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }
    }
}
