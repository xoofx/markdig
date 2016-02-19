using System.Collections.Generic;

namespace Textamina.Markdig.Syntax
{
    public class Document : ContainerBlock
    {
        public Document()
        {
            LinkReferenceDefinitions = new Dictionary<string, LinkReferenceDefinitionBlock>();
        }

        public Dictionary<string, LinkReferenceDefinitionBlock> LinkReferenceDefinitions { get; }
    }
}