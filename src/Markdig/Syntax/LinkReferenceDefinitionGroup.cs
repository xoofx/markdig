// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Markdig.Syntax
{
    /// <summary>
    /// Contains all the <see cref="LinkReferenceDefinition"/> found in a document.
    /// </summary>
    /// <seealso cref="Markdig.Syntax.ContainerBlock" />
    public class LinkReferenceDefinitionGroup : ContainerBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkReferenceDefinitionGroup"/> class.
        /// </summary>
        public LinkReferenceDefinitionGroup() : base(null)
        {
            Links = new Dictionary<string, LinkReferenceDefinition>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets an association between a label and the corresponding <see cref="LinkReferenceDefinition"/>
        /// </summary>
        public Dictionary<string, LinkReferenceDefinition> Links { get; }

        public void Set(string label, LinkReferenceDefinition link)
        {
            if (link == null) throw new ArgumentNullException(nameof(link));
            Links[label] = link;
            if (!Contains(link))
            {
                Add(link);
            }
        }

        public bool TryGet(string label, out LinkReferenceDefinition link)
        {
            return Links.TryGetValue(label, out link);
        }
    }
}