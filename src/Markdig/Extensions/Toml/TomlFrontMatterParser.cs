// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Extensions.Yaml;
using Markdig.Parsers;

namespace Markdig.Extensions.Toml;

/// <summary>
/// Block parser for a TOML frontmatter.
/// </summary>
/// <seealso cref="TomlFrontMatterBlock" />
public sealed class TomlFrontMatterParser : YamlFrontMatterParser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TomlFrontMatterParser"/> class.
    /// </summary>
    public TomlFrontMatterParser() : base('+')
    {
    }

    /// <summary>
    /// Creates the TOML frontmatter block.
    /// </summary>
    /// <param name="processor">The block processor.</param>
    /// <returns>The TOML frontmatter block.</returns>
    protected override YamlFrontMatterBlock CreateFrontMatterBlock(BlockProcessor processor)
    {
        return new TomlFrontMatterBlock(this);
    }
}