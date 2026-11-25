// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Buffers;

namespace Markdig.Parsers;

/// <summary>
/// A list of <see cref="InlineParser"/>.
/// </summary>
/// <seealso cref="ParserList{InlineParser, InlineParserState}" />
public class InlineParserList : ParserList<InlineParser, InlineProcessor>
{
    // Parsers that search their possible matches themselves, or null if there are none.
    private readonly InlineParser[]? _searchingParsers;

    // The opening characters of all other parsers.
    private readonly SearchValues<char>? _otherOpeningCharacters;

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
            var searchingParsers = new List<InlineParser>();
            var otherOpeningCharacters = new List<char>();
            foreach (var openingChar in OpeningCharacters)
            {
                var openingParsers = GetParsersForOpeningCharacter(openingChar)!;
                if (!Array.TrueForAll(openingParsers, openingParser => openingParser.CanSearchPossibleMatch))
                {
                    otherOpeningCharacters.Add(openingChar);
                    continue;
                }

                foreach (var openingParser in openingParsers)
                {
                    if (!searchingParsers.Contains(openingParser))
                    {
                        searchingParsers.Add(openingParser);
                    }
                }
            }

            if (searchingParsers.Count > 0)
            {
                _searchingParsers = searchingParsers.ToArray();
                _otherOpeningCharacters = SearchValues.Create(otherOpeningCharacters.ToArray());
            }
        }
    }

    /// <summary>
    /// Gets the registered post inline processors.
    /// </summary>
    public IPostInlineProcessor[] PostInlineProcessors { get; private set; }

    /// <summary>
    /// Searches for an opening character like <see cref="ParserList{T, TState}.IndexOfOpeningCharacter"/>, skipping
    /// the positions where <see cref="InlineParser.IndexOfPossibleMatch"/> rules out every parser of the character.
    /// </summary>
    /// <param name="processor">The processor that parses the text. It keeps the last possible match found.</param>
    /// <param name="text">The text.</param>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    /// <returns>The position of the first opening character that may start a match; if not found, returns -1</returns>
    internal int IndexOfOpeningCharacterThatMayMatch(InlineProcessor processor, string text, int start, int end)
    {
        if (_searchingParsers is null)
        {
            return IndexOfOpeningCharacter(text, start, end);
        }

        int index = text.AsSpan(start, end - start + 1).IndexOfAny(_otherOpeningCharacters!);
        if (index >= 0)
        {
            index += start;
        }

        // A possible match depends on the text alone, so the last one found stays valid until the scan passes it
        ref var possibleMatch = ref processor.PossibleMatch;
        if (!ReferenceEquals(possibleMatch.Text, text) || possibleMatch.End != end ||
            start < possibleMatch.Start || possibleMatch.Index < start)
        {
            SearchPossibleMatch(ref possibleMatch, text, start, end);
        }

        return possibleMatch.Index <= end && (index < 0 || possibleMatch.Index < index) ? possibleMatch.Index : index;
    }

    private void SearchPossibleMatch(ref PossibleMatch possibleMatch, string text, int start, int end)
    {
        int index = int.MaxValue;

        foreach (var parser in _searchingParsers!)
        {
            int parserIndex = parser.IndexOfPossibleMatch(text, start, end);
            if (parserIndex >= 0 && parserIndex < index)
            {
                index = parserIndex;
            }
        }

        possibleMatch.Text = text;
        possibleMatch.Start = start;
        possibleMatch.End = end;
        possibleMatch.Index = index;
    }

    /// <summary>
    /// The first possible match of the searching parsers from <see cref="Start"/> to <see cref="End"/> of a text.
    /// </summary>
    internal struct PossibleMatch
    {
        /// <summary>
        /// The text that was searched.
        /// </summary>
        public string? Text;

        /// <summary>
        /// The position from which the text was searched.
        /// </summary>
        public int Start;

        /// <summary>
        /// The last position of the text.
        /// </summary>
        public int End;

        /// <summary>
        /// The position of the possible match, or <see cref="int.MaxValue"/> if there is none.
        /// </summary>
        public int Index;
    }
}
