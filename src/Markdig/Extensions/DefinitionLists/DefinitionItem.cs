// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Extensions.DefinitionLists;

/// <summary>
/// A definition item contains zero to multiple <see cref="DefinitionTerm"/> 
/// and definitions (any <see cref="Block"/>)
/// </summary>
/// <seealso cref="ContainerBlock" />
public class DefinitionItem : ContainerBlock
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefinitionItem"/> class.
    /// </summary>
    /// <param name="parser">The parser used to create this block.</param>
    public DefinitionItem(BlockParser parser) : base(parser)
    {
    }

    /// <summary>
    /// Gets or sets the opening character for this definition item (either `:` or `~`)
    /// </summary>
    public char OpeningCharacter { get; set; }
}