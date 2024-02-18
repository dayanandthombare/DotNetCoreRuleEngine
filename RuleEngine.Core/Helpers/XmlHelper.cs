using System.Xml.Linq;

namespace RuleEngine.Core.Helpers
{
    public class XmlHelper
    {
        public static bool IsTemplateMatch(XElement element, string templateId)
        {
            return element.Attribute("templateid")?.Value == templateId;
        }     
        public static IEnumerable<XElement> GetAllSiblings(XElement element)
        {
            if (element.Parent == null) return Enumerable.Empty<XElement>();
            return element.Parent.Elements().Where(e => e != element);
        }     
        public static bool HasFollowingSiblingWithTemplate(XElement element, string templateId)
        {
            bool foundCurrentElement = false;
            foreach (var sibling in GetAllSiblings(element))
            {
                if (foundCurrentElement && IsTemplateMatch(sibling, templateId))
                {
                    return true;
                }
                if (sibling == element)
                {
                    foundCurrentElement = true;
                }
            }
            return false;
        }
     
        public static IEnumerable<XElement> GetCousins(XElement element)
        {
            var parent = element.Parent;
            if (parent == null) return Enumerable.Empty<XElement>();

            var grandparents = parent.Parent;
            if (grandparents == null) return Enumerable.Empty<XElement>();

          
            var cousins = grandparents.Elements()
                                      .Where(x => x != parent) 
                                      .SelectMany(x => x.Elements()); 

            return cousins;
        }
    }
}
