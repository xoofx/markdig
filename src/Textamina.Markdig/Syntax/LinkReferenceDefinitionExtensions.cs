// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
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