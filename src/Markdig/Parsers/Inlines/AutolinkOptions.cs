// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

namespace Markdig.Parsers.Inlines;

public class AutolinkOptions : LinkOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to enable HTML parsing. Default is <c>true</c>
    /// </summary>
    public bool EnableHtmlParsing { get; set; }
}
