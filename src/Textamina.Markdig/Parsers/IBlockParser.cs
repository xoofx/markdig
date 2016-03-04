// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsers
{
    public interface IBlockParser<in T> : IMarkdownParser<T>
    {
        bool CanInterrupt(BlockParserState state, Block block);

        BlockState TryOpen(BlockParserState state);

        BlockState TryContinue(BlockParserState state, Block block);

        bool Close(BlockParserState state, Block block);
    }
}