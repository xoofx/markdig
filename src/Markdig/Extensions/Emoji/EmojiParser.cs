// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Parsers.Inlines;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.Emoji;

/// <summary>
/// The inline parser used for emojis.
/// </summary>
/// <seealso cref="InlineParser" />
public class EmojiParser : InlineParser, IPostInlineProcessor
{
    private readonly EmojiMapping _emojiMapping;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmojiParser"/> class.
    /// </summary>
    public EmojiParser(EmojiMapping emojiMapping)
    {
        _emojiMapping = emojiMapping;
        OpeningCharacters = _emojiMapping.OpeningCharacters;
    }

    /// <summary>
    /// Attempts to match the parser at the current position.
    /// </summary>
    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        // Previous char must be a space or non-alphanumeric.
        var prevChar = slice.PeekCharExtra(-1);
        if (char.IsLetterOrDigit(prevChar))
        {
            return false;
        }

        // Try to match an emoji shortcode or smiley
        if (!_emojiMapping.PrefixTree.TryMatchLongest(slice.Text.AsSpan(slice.Start, slice.Length), out KeyValuePair<string, string> match))
        {
            return false;
        }

        // Leave a trailing emphasis delimiter run to the emphasis parser. Only convert
        // the candidate after emphasis processing, if the whole match remains literal.
        var lastChar = match.Key[match.Key.Length - 1];
        if (processor.Parsers.Find<EmphasisInlineParser>() is { } emphasisParser && emphasisParser.HasEmphasisChar(lastChar))
        {
            int prefixLength = match.Key.Length - 1;
            while (prefixLength > 0 && match.Key[prefixLength - 1] == lastChar)
            {
                prefixLength--;
            }
            // Mappings made entirely of this delimiter character retain their existing priority.
            if (prefixLength > 0)
            {
                processor.Inline = new PendingEmoji(match)
                {
                    Content = new StringSlice(slice.Text, slice.Start, slice.Start + prefixLength - 1),
                    Span = new(processor.GetSourcePosition(slice.Start, out int pendingLine, out int pendingColumn),
                        processor.GetSourcePosition(slice.Start + prefixLength - 1)),
                    Line = pendingLine,
                    Column = pendingColumn
                };
                processor.ParserStates[Index] = this;
                slice.Start += prefixLength;
                return true;
            }
        }

        // Push the EmojiInline
        processor.Inline = new EmojiInline(match.Value)
        {
            Span =
            {
                Start = processor.GetSourcePosition(slice.Start, out int line, out int column),
            },
            Line = line,
            Column = column,
            Match = match.Key
        };
        processor.Inline.Span.End = processor.Inline.Span.Start + match.Key.Length - 1;

        // Move the cursor to the character after the matched string
        slice.Start += match.Key.Length;

        return true;
    }

    /// <summary>
    /// Converts deferred emoji candidates whose trailing delimiters were not used by emphasis.
    /// </summary>
    public bool PostProcess(InlineProcessor state, Inline? root, Inline? lastChild, int postInlineProcessorIndex, bool isFinalProcessing)
    {
        if (state.ParserStates[Index] is null || root is not ContainerInline container)
        {
            return true;
        }

        state.PostProcessInlines(postInlineProcessorIndex + 1, root, lastChild, isFinalProcessing);
        foreach (var pending in container.FindDescendants<PendingEmoji>())
        {
            int end = pending.Content.Start + pending.Match.Key.Length - 1;
            LiteralInline? suffix = pending;
            if (pending.Content.End < end)
            {
                suffix = pending.NextSibling as LiteralInline;
                if (suffix is null || suffix.IsFirstCharacterEscaped ||
                    !ReferenceEquals(suffix.Content.Text, pending.Content.Text) ||
                    suffix.Content.Start != pending.Content.End + 1 || suffix.Content.End < end)
                {
                    // Emphasis consumed some of the candidate (possibly across a container boundary).
                    if (isFinalProcessing)
                    {
                        pending.ReplaceBy(new LiteralInline(pending.Content)
                        {
                            Span = pending.Span, Line = pending.Line, Column = pending.Column
                        });
                    }
                    continue;
                }
            }

            var emoji = new EmojiInline(pending.Match.Value)
            {
                Match = pending.Match.Key,
                Span = new(pending.Span.Start, pending.Span.Start + pending.Match.Key.Length - 1),
                Line = pending.Line,
                Column = pending.Column
            };
            if (suffix.Content.End > end)
            {
                // Literal parsing may have coalesced following text into the candidate or its suffix.
                var remainder = new LiteralInline(new StringSlice(suffix.Content.Text, end + 1, suffix.Content.End))
                {
                    Span = new(state.GetSourcePosition(end + 1, out int line, out int column), suffix.Span.End),
                    Line = line,
                    Column = column
                };
                suffix.InsertAfter(remainder);
            }
            if (suffix != pending)
            {
                suffix.Remove();
            }
            pending.ReplaceBy(emoji);
        }
        // Subsequent processors have already run above.
        return false;
    }

    private sealed class PendingEmoji(KeyValuePair<string, string> match) : LiteralInline
    {
        public KeyValuePair<string, string> Match { get; } = match;
    }
}
