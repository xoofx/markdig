// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Parsers;

namespace Markdig.Syntax;

/// <summary>
/// Block representing a document with characters but no blocks. This can
/// happen when an input document consists solely of trivia.
/// </summary>
public sealed class EmptyBlock  : LeafBlock
{
    public EmptyBlock (BlockParser? parser) : base(parser)
    {
    }
}
