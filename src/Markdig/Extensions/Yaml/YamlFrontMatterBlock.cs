// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Extensions.Yaml;

/// <summary>
/// A YAML frontmatter block.
/// </summary>
/// <seealso cref="CodeBlock" />
public class YamlFrontMatterBlock : CodeBlock
{
    /// <summary>
    /// Initializes a new instance of the <see cref="YamlFrontMatterBlock"/> class.
    /// </summary>
    /// <param name="parser">The parser.</param>
    public YamlFrontMatterBlock(BlockParser parser) : this(parser, '-')
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YamlFrontMatterBlock"/> class with the specified fence character.
    /// </summary>
    /// <param name="parser">The parser.</param>
    /// <param name="fenceChar">The character used to delimit the frontmatter block.</param>
    protected YamlFrontMatterBlock(BlockParser parser, char fenceChar) : base(parser)
    {
        FenceChar = fenceChar;
    }

    /// <summary>
    /// Gets the character used to delimit this frontmatter block.
    /// </summary>
    public char FenceChar { get; }
}