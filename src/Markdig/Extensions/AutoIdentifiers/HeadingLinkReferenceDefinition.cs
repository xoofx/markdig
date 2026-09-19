// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax;

namespace Markdig.Extensions.AutoIdentifiers;

/// <summary>
/// A link reference definition to a <see cref="HeadingBlock"/> stored at the <see cref="MarkdownDocument"/> level.
/// </summary>
/// <seealso cref="LinkReferenceDefinition" />
public class HeadingLinkReferenceDefinition : LinkReferenceDefinition
{
    /// <summary>
    /// Initializes a new instance of the HeadingLinkReferenceDefinition class.
    /// </summary>
    public HeadingLinkReferenceDefinition(HeadingBlock headling)
    {
        Heading = headling;
        // Created implicitly, so it must not resolve inside another still-open
        // link bracket, e.g. [Some text [Heading]](url).
        AllowResolutionInsideOpenLink = false;
    }

    /// <summary>
    /// Gets or sets the heading related to this link reference definition.
    /// </summary>
    public HeadingBlock Heading { get; set; }
}
