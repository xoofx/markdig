// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

namespace Markdig.Extensions.RelativeFileLinks;

/// <summary>
/// Options for the <see cref="RelativeFileLinkExtension"/>.
/// </summary>
public sealed class RelativeFileLinkOptions
{
    /// <summary>
    /// Gets the file extensions of the link targets to rewrite, compared case-insensitively (`.md` and `.markdown` by default). Clear the list to replace the defaults.
    /// </summary>
    public List<string> SourceExtensions { get; } = [".md", ".markdown"];

    /// <summary>
    /// Gets or sets the file extension that replaces a source extension (`.html` by default). An empty string produces extensionless links.
    /// </summary>
    public string TargetExtension { get; set; } = ".html";
}
