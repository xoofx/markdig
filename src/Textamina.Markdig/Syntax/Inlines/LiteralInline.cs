// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Text;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Syntax.Inlines
{
    public class LiteralInline : LeafInline
    {
        public LiteralInline()
        {
            Content = new StringSlice(null);
        }

        public LiteralInline(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            Content = new StringSlice(text);
        }

        public StringSlice Content;

        protected override void Close(InlineParserState state)
        {
        }

        public override string ToString()
        {
            return Content.ToString();
        }
    }
}