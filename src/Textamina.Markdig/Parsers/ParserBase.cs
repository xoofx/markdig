// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
namespace Textamina.Markdig.Parsers
{
    public class ParserBase<T> : IMarkdownParser<T>
    {
        public char[] OpeningCharacters { get; set; }

        public virtual void Initialize(T state)
        {
        }

        public int Index { get; internal set; }
    }
}