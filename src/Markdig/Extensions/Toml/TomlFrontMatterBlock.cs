// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Extensions.Yaml;
using Markdig.Parsers;

namespace Markdig.Extensions.Toml;

/// <summary>
/// A TOML frontmatter block.
/// </summary>
/// <seealso cref="YamlFrontMatterBlock" />
public sealed class TomlFrontMatterBlock : YamlFrontMatterBlock
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TomlFrontMatterBlock"/> class.
    /// </summary>
    /// <param name="parser">The parser.</param>
    public TomlFrontMatterBlock(BlockParser parser) : base(parser, '+')
    {
    }
}