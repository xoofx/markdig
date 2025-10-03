// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Parsers;

namespace Markdig.Extensions.AutoLinks;

public class AutoLinkOptions : LinkOptions
{
    public AutoLinkOptions()
    {
        ValidPreviousCharacters = "*_~(";
    }

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
