using System;
using System.Collections.Generic;

namespace Textamina.Markdig.Syntax
{
    public static class LinkReferenceDefinitionExtensions
    {
        public static Dictionary<string, LinkReferenceDefinition> GetLinkReferenceDefinitions(this Document document)
        {
            var references = document.GetData(LinkReferenceDefinition.DocumentKey) as Dictionary<string, LinkReferenceDefinition>;
            if (references == null)
            {
                references = new Dictionary<string, LinkReferenceDefinition>(StringComparer.OrdinalIgnoreCase);
                document.SetData(LinkReferenceDefinition.DocumentKey, references);
            }
            return references;
        }
    }
}