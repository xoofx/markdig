// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Syntax;

namespace Markdig.Extensions.AutoIdentifiers
{
    /// <summary>
    /// A link reference definition to a <see cref="HeadingBlock"/> stored at the <see cref="MarkdownDocument"/> level.
    /// </summary>
    /// <seealso cref="Markdig.Syntax.LinkReferenceDefinition" />
    public class HeadingLinkReferenceDefinition : LinkReferenceDefinition
    {
        /// <summary>
        /// Gets or sets the heading related to this link reference definition.
        /// </summary>
        public HeadingBlock Heading { get; set; }
    }
}