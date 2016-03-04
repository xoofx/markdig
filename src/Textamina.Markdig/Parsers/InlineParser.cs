// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsers
{
    public abstract class InlineParser : ParserBase<InlineParserState>
    {
        public abstract bool Match(InlineParserState state, ref StringSlice slice);
    }
}