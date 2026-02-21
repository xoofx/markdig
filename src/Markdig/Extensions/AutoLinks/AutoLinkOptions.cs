// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Parsers;

namespace Markdig.Extensions.AutoLinks;

/// <summary>
/// Represents the AutoLinkOptions type.
/// </summary>
public class AutoLinkOptions : LinkOptions
{
    /// <summary>
    /// Initializes a new instance of the AutoLinkOptions class.
    /// </summary>
    public AutoLinkOptions()
    {
        ValidPreviousCharacters = "*_~(";
    }

    /// <summary>
    /// Gets or sets the valid previous characters.
    /// </summary>
    public string ValidPreviousCharacters { get; set; }

    /// <summary>
    /// Should a www link be prefixed with https:// instead of http:// (false by default)
    /// </summary>
    public bool UseHttpsForWWWLinks { get; set; }

    /// <summary>
    /// Should auto-linking allow a domain with no period, e.g. https://localhost (false by default)
    /// </summary>
    public bool AllowDomainWithoutPeriod { get; set; }
}
