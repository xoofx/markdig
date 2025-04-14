// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.Diagnostics;

using Markdig.Helpers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace Markdig.Parsers;

public abstract class FencedBlockParserBase : BlockParser, IAttributesParseable
{
    /// <summary>
    /// Delegate used to parse the string on the first line after the fenced code block special characters (usually ` or ~)
    /// </summary>
    /// <param name="state">The parser processor.</param>
    /// <param name="line">The being processed line.</param>
    /// <param name="fenced">The fenced code block.</param>
    /// <param name="openingCharacter">The opening character for the fenced code block (usually ` or ~)</param>
    /// <returns><c>true</c> if parsing of the line is successfull; <c>false</c> otherwise</returns>
    public delegate bool InfoParserDelegate(BlockProcessor state, ref StringSlice line, IFencedBlock fenced, char openingCharacter);


    /// <summary>
    /// Gets or sets the information parser.
    /// </summary>
    public InfoParserDelegate? InfoParser { get; set; }

    /// <summary>
    /// A delegates that allows to process attached attributes
    /// </summary>
    public TryParseAttributesDelegate? TryParseAttributes { get; set; }
}

/// <summary>
/// Base parser for fenced blocks (opened by 3 or more character delimiters on a first line, and closed by at least the same number of delimiters)
/// </summary>
/// <seealso cref="BlockParser" />
public abstract class FencedBlockParserBase<T> : FencedBlockParserBase where T : Block, IFencedBlock
{
    private static readonly TransformedStringCache s_infoStringCache = new(static infoString => HtmlHelper.Unescape(infoString));
    private static readonly TransformedStringCache s_argumentsStringCache = new(static argumentsString => HtmlHelper.Unescape(argumentsString));
    private TransformedStringCache? _infoPrefixCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="FencedBlockParserBase{T}"/> class.
    /// </summary>
    protected FencedBlockParserBase()
    {
        InfoParser = DefaultInfoParser;
        MinimumMatchCount = 3;
        MaximumMatchCount = int.MaxValue;
    }

    private string? _infoPrefix;
    /// <summary>
    /// Gets or sets the language prefix (default is "language-")
    /// </summary>
    public string? InfoPrefix
    {
        get => _infoPrefix;
        set
        {
            if (_infoPrefix != value)
            {
                _infoPrefixCache = new TransformedStringCache(infoString => value + infoString);
                _infoPrefix = value;
            }
        }
    }

    public int MinimumMatchCount { get; set; }

    public int MaximumMatchCount { get; set; }

    private enum ParseState
    {
        AfterFence,
        Info,
        AfterInfo,
        Args,
        AfterArgs,
    }

    /// <summary>
    /// The roundtrip parser for the information after the fenced code block special characters (usually ` or ~)
    /// </summary>
    /// <param name="blockProcessor">The parser processor.</param>
    /// <param name="line">The line.</param>
    /// <param name="fenced">The fenced code block.</param>
    /// <param name="openingCharacter">The opening character for this fenced code block.</param>
    /// <returns><c>true</c> if parsing of the line is successfull; <c>false</c> otherwise</returns>
    public static bool RoundtripInfoParser(BlockProcessor blockProcessor, ref StringSlice line, IFencedBlock fenced, char openingCharacter)
    {
        var start = line.Start;
        var end = start - 1;
        var afterFence = new StringSlice(line.Text, start, end);
        var info = new StringSlice(line.Text, start, end);
        var afterInfo = new StringSlice(line.Text, start, end);
        var arg = new StringSlice(line.Text, start, end);
        var afterArg = new StringSlice(line.Text, start, end);
        ParseState state = ParseState.AfterFence;

        for (int i = line.Start; i <= line.End; i++)
        {
            char c = line.Text[i];
            // An info string cannot contain any backticks (unless it is a tilde block)
            if (c == '`' && openingCharacter == '`')
            {
                return false;
            }
            switch (state)
            {
                case ParseState.AfterFence:
                    if (c.IsSpaceOrTab())
                    {
                        afterFence.End += 1;
                    }
                    else
                    {
                        state = ParseState.Info;
                        info.Start = i;
                        info.End = i;
                        afterFence.End = i - 1;
                    }
                    break;
                case ParseState.Info:
                    if (c.IsSpaceOrTab())
                    {
                        state = ParseState.AfterInfo;
                        afterInfo.Start = i;
                        afterInfo.End = i;
                    }
                    else
                    {
                        info.End += 1;
                    }
                    break;
                case ParseState.AfterInfo:
                    if (c.IsSpaceOrTab())
                    {
                        afterInfo.End += 1;
                    }
                    else
                    {
                        arg.Start = i;
                        arg.End = i;
                        state = ParseState.Args;
                    }
                    break;
                case ParseState.Args:
                    // walk from end, as rest (except trailing spaces) is args
                    for (int j = line.End; j > start; j--)
                    {
                        c = line[j];
                        if (c.IsSpaceOrTab())
                        {
                            afterArg.Start = i;
                        }
                        else
                        {
                            arg.End = j;
                            afterArg.Start = j + 1;
                            afterArg.End = line.End;
                            goto end;
                        }
                    }
                    goto end;
                case ParseState.AfterArgs:
                    {
                        return false;
                    }
            }
        }

    end:
        fenced.TriviaAfterFencedChar = afterFence;
        fenced.Info = s_infoStringCache.Get(info.AsSpan());
        fenced.UnescapedInfo = info;
        fenced.TriviaAfterInfo = afterInfo;
        fenced.Arguments = HtmlHelper.Unescape(arg.ToString());
        fenced.UnescapedArguments = arg;
        fenced.TriviaAfterArguments = afterArg;
        fenced.InfoNewLine = line.NewLine;

        return true;
    }

    /// <summary>
    /// The default parser for the information after the fenced code block special characters (usually ` or ~)
    /// </summary>
    /// <param name="state">The parser processor.</param>
    /// <param name="line">The line.</param>
    /// <param name="fenced">The fenced code block.</param>
    /// <param name="openingCharacter">The opening character for this fenced code block.</param>
    /// <returns><c>true</c> if parsing of the line is successfull; <c>false</c> otherwise</returns>
    public static bool DefaultInfoParser(BlockProcessor state, ref StringSlice line, IFencedBlock fenced, char openingCharacter)
    {
        int firstSpace = -1;
        ReadOnlySpan<char> lineSpan = line.AsSpan();

        if (!lineSpan.IsEmpty)
        {
            if (openingCharacter == '`')
            {
                firstSpace = lineSpan.IndexOfAny(' ', '\t', '`');

                // An info string cannot contain any backticks (unless it is a tilde block)
                if (firstSpace >= 0 && lineSpan.Slice(firstSpace).Contains('`'))
                {
                    return false;
                }
            }
            else
            {
                firstSpace = lineSpan.IndexOfAny(' ', '\t');
            }
        }

        StringSlice infoStringSlice;

        if (firstSpace >= 0)
        {
            firstSpace += line.Start;
            infoStringSlice = new StringSlice(line.Text, line.Start, firstSpace - 1);

            var argStringSlice = new StringSlice(line.Text, firstSpace, line.End);
            argStringSlice.Trim();
            fenced.Arguments = s_argumentsStringCache.Get(argStringSlice.AsSpan());
        }
        else
        {
            infoStringSlice = line;
            fenced.Arguments = string.Empty;
        }

        infoStringSlice.Trim();

        fenced.Info = s_infoStringCache.Get(infoStringSlice.AsSpan());

        return true;
    }

    public override BlockState TryOpen(BlockProcessor processor)
    {
        // We expect no indentation for a fenced code block.
        if (processor.IsCodeIndent)
        {
            return BlockState.None;
        }

        // Match fenced char
        var line = processor.Line;
        char matchChar = line.CurrentChar;
        int count = line.CountAndSkipChar(matchChar);

        // A fenced codeblock requires at least 3 opening chars
        if (count < MinimumMatchCount || count > MaximumMatchCount)
        {
            return BlockState.None;
        }

        // specs spaces: Is space and tabs? or only spaces? Use space and tab for this case
        if (!processor.TrackTrivia)
        {
            line.TrimStart();
        }

        var fenced = CreateFencedBlock(processor);
        {
            fenced.Column = processor.Column;
            fenced.FencedChar = matchChar;
            fenced.OpeningFencedCharCount = count;
            fenced.Span.Start = processor.Start;
            fenced.Span.End = line.Start;
        };

        // Try to parse any attached attributes
        TryParseAttributes?.Invoke(processor, ref line, fenced);

        // If the info parser was not successful, early exit
        if (InfoParser != null && !InfoParser(processor, ref line, fenced, matchChar))
        {
            return BlockState.None;
        }

        // Add the language as an attribute by default
        string? info = fenced.Info;

        if (!string.IsNullOrEmpty(info))
        {
            Debug.Assert(_infoPrefixCache is not null || InfoPrefix is null);
            string infoWithPrefix = _infoPrefixCache?.Get(info!) ?? info!;
            fenced.GetAttributes().AddClass(infoWithPrefix);
        }

        // Store the number of matched string into the context
        processor.NewBlocks.Push(fenced);

        // Discard the current line as it is already parsed
        return BlockState.ContinueDiscard;
    }

    protected abstract T CreateFencedBlock(BlockProcessor processor);

    public override BlockState TryContinue(BlockProcessor processor, Block block)
    {
        var fence = (IFencedBlock)block;

        // Match if we have a closing fence
        var line = processor.Line;
        var sourcePosition = processor.Start;
        var closingCount = line.CountAndSkipChar(fence.FencedChar);

        char c = line.CurrentChar;

        // If we have a closing fence, close it and discard the current line
        // The line must contain only fence opening character followed only by whitespaces.
        var startBeforeTrim = line.Start;

        if (fence.OpeningFencedCharCount <= closingCount &&
            !processor.IsCodeIndent &&
            c.IsWhiteSpaceOrZero() &&
            line.TrimEnd())
        {
            block.UpdateSpanEnd(startBeforeTrim - 1);

            fence.ClosingFencedCharCount = closingCount;

            if (processor.TrackTrivia)
            {
                fence.NewLine = line.NewLine;
                fence.TriviaBeforeClosingFence = processor.UseTrivia(sourcePosition - 1);
                fence.TriviaAfter = new StringSlice(line.Text, processor.Start + closingCount, processor.Line.End);
            }

            // Don't keep the last line
            return BlockState.BreakDiscard;
        }

        // Reset the indentation to the column before the indent
        processor.GoToColumn(processor.ColumnBeforeIndent);

        return BlockState.Continue;
    }
}