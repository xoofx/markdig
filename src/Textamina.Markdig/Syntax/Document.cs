using System;
using System.Collections.Generic;

namespace Textamina.Markdig.Syntax
{
    public class Document : ContainerBlock
    {
        public Document() : base(null)
        {
            LinkReferenceDefinitions = new Dictionary<string, LinkReferenceDefinitionBlock>(StringComparer.OrdinalIgnoreCase);
        }

        public Dictionary<string, LinkReferenceDefinitionBlock> LinkReferenceDefinitions { get; }
    }
}