// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Parsers;

namespace Markdig.Extensions.Yaml;

/// <summary>
/// Block parser for a YAML frontmatter.
/// </summary>
/// <seealso cref="YamlFrontMatterBlock" />
public class YamlFrontMatterParser : FrontMatterParser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="YamlFrontMatterParser"/> class.
    /// </summary>
    public YamlFrontMatterParser() : base('-')
    {
    }

    /// <summary>
    /// Gets a value indicating whether a <c>...</c> line also closes the frontmatter block: <c>true</c> for YAML.
    /// </summary>
    protected override bool AllowDotsFence => true;

    /// <summary>
    /// Creates the front matter block.
    /// </summary>
    /// <param name="processor">The block processor</param>
    /// <returns>The front matter block</returns>
    protected override YamlFrontMatterBlock CreateFrontMatterBlock(BlockProcessor processor)
    {
        return new YamlFrontMatterBlock(this);
    }
}