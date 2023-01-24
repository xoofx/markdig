// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax;

namespace Markdig.Extensions.Footnotes;

/// <summary>
/// A link reference definition stored at the <see cref="MarkdownDocument"/> level.
/// </summary>
/// <seealso cref="LinkReferenceDefinition" />
public class FootnoteLinkReferenceDefinition : LinkReferenceDefinition
{
    public FootnoteLinkReferenceDefinition(Footnote footnote)
    {
        Footnote = footnote;
    }

    /// <summary>
    /// Gets or sets the footnote related to this link reference definition.
    /// </summary>
    public Footnote Footnote { get; set; }
}