// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Extensions.Yaml;

/// <summary>
/// Base block parser for a front matter between two lines of three fence characters,
/// such as <c>---</c> for <see cref="YamlFrontMatterParser"/> and <c>+++</c> for <see cref="Toml.TomlFrontMatterParser"/>.
/// </summary>
/// <seealso cref="YamlFrontMatterBlock" />
public abstract class FrontMatterParser : BlockParser
{
    // We reuse a FencedCodeBlock parser to grab a frontmatter, only active if it happens on the first line of the document.

    /// <summary>
    /// Initializes a new instance of the <see cref="FrontMatterParser"/> class.
    /// </summary>
    /// <param name="fenceChar">The character used to delimit the frontmatter block.</param>
    protected FrontMatterParser(char fenceChar)
    {
        FenceChar = fenceChar;
        OpeningCharacters = [fenceChar];
    }

    /// <summary>
    /// Allows the <see cref="YamlFrontMatterBlock"/> to appear in the middle of the markdown file.
    /// </summary>
    public bool AllowInMiddleOfDocument { get; set; }

    /// <summary>
    /// Gets the character used to delimit the frontmatter block.
    /// </summary>
    public char FenceChar { get; }

    /// <summary>
    /// Gets a value indicating whether a <c>...</c> line also closes the frontmatter block, as in YAML.
    /// </summary>
    protected virtual bool AllowDotsFence => false;

    /// <summary>
    /// Creates the front matter block.
    /// </summary>
    /// <param name="processor">The block processor</param>
    /// <returns>The front matter block</returns>
    protected abstract YamlFrontMatterBlock CreateFrontMatterBlock(BlockProcessor processor);

    /// <summary>
    /// Tries to match a block opening.
    /// </summary>
    /// <param name="processor">The parser processor.</param>
    /// <returns>The result of the match</returns>
    public override BlockState TryOpen(BlockProcessor processor)
    {
        // We expect no indentation for a fenced code block.
        if (processor.IsCodeIndent)
        {
            return BlockState.None;
        }

        // Only accept a frontmatter at the beginning of the file
        if (!AllowInMiddleOfDocument && processor.Start != 0)
        {
            return BlockState.None;
        }

        int count = 0;
        var line = processor.Line;
        char c = line.CurrentChar;

        // Must consist of exactly three fence characters
        while (c == FenceChar && count < 4)
        {
            count++;
            c = line.NextChar();
        }

        // If three fence characters (optionally followed by whitespace)
        // this is a front matter block
        if (count == 3 && c.IsWhiteSpaceOrZero() && line.TrimEnd())
        {
            bool hasFullYamlFrontMatter = false;
            // We make sure that there is a closing frontmatter somewhere in the document
            // so here we work on the full document instead of just the line
            var fullLine = new StringSlice(line.Text, line.Start, line.Text.Length - 1);
            c = fullLine.CurrentChar;
            while (c != '\0')
            {
                c = fullLine.NextChar();
                if (c == '\n' || c == '\r')
                {
                    var nc = fullLine.PeekChar();
                    if (c == '\r' && nc == '\n')
                    {
                        c = fullLine.NextChar();
                    }
                    nc = fullLine.PeekChar();
                    if (nc == FenceChar)
                    {
                        if (fullLine.NextChar() == FenceChar && fullLine.NextChar() == FenceChar && fullLine.NextChar() == FenceChar && (fullLine.NextChar() == '\0' || fullLine.SkipSpacesToEndOfLineOrEndOfDocument()))
                        {
                            hasFullYamlFrontMatter = true;
                            break;
                        }
                    }
                    else if (AllowDotsFence && nc == '.')
                    {
                        if (fullLine.NextChar() == '.' && fullLine.NextChar() == '.' && fullLine.NextChar() == '.' && (fullLine.NextChar() == '\0' || fullLine.SkipSpacesToEndOfLineOrEndOfDocument()))
                        {
                            hasFullYamlFrontMatter = true;
                            break;
                        }
                    }
                }
            }

            if (hasFullYamlFrontMatter)
            {
                // Create a front matter block
                var block = this.CreateFrontMatterBlock(processor);
                block.Column = processor.Column;
                block.Span.Start = 0;
                block.Span.End = line.Start;

                // Store the number of matched string into the context
                processor.NewBlocks.Push(block);

                // Discard the current line as it is already parsed
                return BlockState.ContinueDiscard;
            }
        }

        return BlockState.None;
    }

    /// <summary>
    /// Tries to continue matching a block already opened.
    /// </summary>
    /// <param name="processor">The parser processor.</param>
    /// <param name="block">The block already opened.</param>
    /// <returns>The result of the match. By default, don't expect any newline</returns>
    public override BlockState TryContinue(BlockProcessor processor, Block block)
    {
        // Determine if we have a closing fence.
        // It can start or end with three fence characters, or with <c>...</c> when AllowDotsFence is true.
        var line = processor.Line;
        var c = line.CurrentChar;
        if (processor.Column == 0 && (c == FenceChar || (AllowDotsFence && c == '.')))
        {
            int count = line.CountAndSkipChar(c);
            c = line.CurrentChar;

            // If we have a closing fence, close it and discard the current line
            // The line must contain only fence characters and optional following whitespace.
            if (count == 3 && !processor.IsCodeIndent && c.IsWhiteSpaceOrZero() && line.TrimEnd())
            {
                block.UpdateSpanEnd(line.Start - 1);

                // Don't keep the last line
                return BlockState.BreakDiscard;
            }
        }

        // Reset the indentation to the column before the indent
        processor.GoToColumn(processor.ColumnBeforeIndent);

        return BlockState.Continue;
    }
}