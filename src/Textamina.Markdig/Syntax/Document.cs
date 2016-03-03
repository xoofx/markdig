using System;
using System.Collections.Generic;

namespace Textamina.Markdig.Syntax
{
    public class Document : ContainerBlock
    {
        public Document() : base(null)
        {
            LinkReferenceDefinitions = new Dictionary<string, LinkReferenceDefinition>(StringComparer.OrdinalIgnoreCase);
        }

        public Dictionary<string, LinkReferenceDefinition> LinkReferenceDefinitions { get; }
    }
}