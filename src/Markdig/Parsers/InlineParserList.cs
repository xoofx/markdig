// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;

namespace Markdig.Parsers;

/// <summary>
/// A list of <see cref="InlineParser"/>.
/// </summary>
/// <seealso cref="ParserList{InlineParser, InlineParserState}" />
public class InlineParserList : ParserList<InlineParser, InlineProcessor>
{
    // ASCII opening characters whose parsers all offer InlineParser.PrecheckMatch, or null if there are none.
    private readonly bool[]? _prechecked;

    /// <summary>
    /// Initializes a new instance of the InlineParserList class.
    /// </summary>
    public InlineParserList(IEnumerable<InlineParser> parsers) : base(parsers)
    {
        // Prepare the list of post inline processors
        var postInlineProcessors = new List<IPostInlineProcessor>();
        foreach (var parser in this)
        {
            if (parser is IPostInlineProcessor delimProcessor)
            {
                postInlineProcessors.Add(delimProcessor);
            }
        }
        PostInlineProcessors = postInlineProcessors.ToArray();

        // A global parser is tried at every opening character, so skipping one would change its results
        if (GlobalParsers is null)
        {
            foreach (var openingChar in OpeningCharacters)
            {
                if (openingChar < 128 && Array.TrueForAll(GetParsersForOpeningCharacter(openingChar)!, parser => parser.HasMatchPrecheck))
                {
                    _prechecked ??= new bool[128];
                    _prechecked[openingChar] = true;
                }
            }
        }
    }

    /// <summary>
    /// Gets the registered post inline processors.
    /// </summary>
    public IPostInlineProcessor[] PostInlineProcessors { get; private set; }

    /// <summary>
    /// Searches for an opening character like <see cref="ParserList{T, TState}.IndexOfOpeningCharacter"/>, skipping
    /// the positions where <see cref="InlineParser.PrecheckMatch"/> rules out every parser of the character.
    /// </summary>
    internal int IndexOfOpeningCharacterThatMayMatch(string text, int start, int end)
    {
        int index = IndexOfOpeningCharacter(text, start, end);

        var prechecked = _prechecked;
        if (prechecked is not null)
        {
            while (index >= 0)
            {
                char c = text[index];
                if (c >= prechecked.Length || !prechecked[c])
                {
                    break;
                }

                var slice = new StringSlice(text, index, end);
                foreach (var parser in GetParsersForOpeningCharacter(c)!)
                {
                    if (parser.PrecheckMatch(slice))
                    {
                        return index;
                    }
                }

                index = IndexOfOpeningCharacter(text, index + 1, end);
            }
        }

        return index;
    }
}
