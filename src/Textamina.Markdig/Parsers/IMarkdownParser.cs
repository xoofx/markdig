// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
namespace Textamina.Markdig.Parsers
{
    public interface IMarkdownParser<in TState>
    {
        char[] OpeningCharacters { get; }

        void Initialize(TState state);

        int Index { get; }
    }
}