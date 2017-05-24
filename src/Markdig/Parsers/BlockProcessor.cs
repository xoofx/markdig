// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Markdig.Helpers;
using Markdig.Syntax;

namespace Markdig.Parsers
{
    /// <summary>
    /// The block processor.
    /// </summary>
    public class BlockProcessor
    {
        private BlockProcessor root;
        private int currentStackIndex;
        private readonly BlockParserStateCache parserStateCache;
        private int originalLineStart = 0;

        private BlockProcessor(BlockProcessor root)
        {
            // These properties are not changing between a parent and a children BlockProcessor
            this.root = root;
            this.parserStateCache = root.parserStateCache;
            StringBuilders = root.StringBuilders;
            Document = root.Document;
            Parsers = root.Parsers;

            // These properties are local to a state
            OpenedBlocks = new List<Block>();
            NewBlocks = new Stack<Block>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockProcessor" /> class.
        /// </summary>
        /// <param name="stringBuilders">The string builders cache.</param>
        /// <param name="document">The document to build blocks into.</param>
        /// <param name="parsers">The list of parsers.</param>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        public BlockProcessor(StringBuilderCache stringBuilders, MarkdownDocument document, BlockParserList parsers)
        {
            if (stringBuilders == null) throw new ArgumentNullException(nameof(stringBuilders));
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (parsers == null) throw new ArgumentNullException(nameof(parsers));
            parserStateCache = new BlockParserStateCache(this);
            StringBuilders = stringBuilders;
            Document = document;
            document.IsOpen = true;
            Parsers = parsers;
            OpenedBlocks = new List<Block>();
            NewBlocks = new Stack<Block>();
            root = this;
            Open(document);
        }

        /// <summary>
        /// Gets the new blocks to push. A <see cref="BlockParser"/> is required to push new blocks that it creates to this property.
        /// </summary>
        public Stack<Block> NewBlocks { get; }

        /// <summary>
        /// Gets the list of <see cref="BlockParser"/> configured with this parser state.
        /// </summary>
        public BlockParserList Parsers { get; }

        /// <summary>
        /// Gets the current active container.
        /// </summary>
        public ContainerBlock CurrentContainer { get; private set; }

        /// <summary>
        /// Gets the last block that is opened.
        /// </summary>
        public Block CurrentBlock { get; private set; }

        /// <summary>
        /// Gets the last block that is created.
        /// </summary>
        public Block LastBlock { get; private set; }

        /// <summary>
        /// Gets the next block in a <see cref="BlockParser.TryContinue"/>.
        /// </summary>
        public Block NextContinue => currentStackIndex + 1 < OpenedBlocks.Count ? OpenedBlocks[currentStackIndex + 1] : null;

        /// <summary>
        /// Gets the root document.
        /// </summary>
        public MarkdownDocument Document { get; }

        /// <summary>
        /// The current line being processed.
        /// </summary>
        public StringSlice Line;

        /// <summary>
        /// Gets or sets the current line start position.
        /// </summary>
        public int CurrentLineStartPosition { get; private set; }

        /// <summary>
        /// Gets the index of the line in the source text.
        /// </summary>
        public int LineIndex { get; set; }

        /// <summary>
        /// Gets a value indicating whether the line is blank (valid only after <see cref="ParseIndent"/> has been called).
        /// </summary>
        public bool IsBlankLine => CurrentChar == '\0';

        /// <summary>
        /// Gets the current character being processed.
        /// </summary>
        public char CurrentChar => Line.CurrentChar;

        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Gets the position of the current character in the line being processed. 
        /// </summary>
        public int Start => Line.Start;

        /// <summary>
        /// Gets the current indent position (number of columns between the previous indent and the current position).
        /// </summary>
        public int Indent => Column - ColumnBeforeIndent;

        /// <summary>
        /// Gets a value indicating whether a code indentation is at the beginning of the line being processed.
        /// </summary>
        public bool IsCodeIndent => Indent >= 4;

        /// <summary>
        /// Gets the column position before the indent occured.
        /// </summary>
        public int ColumnBeforeIndent { get; private set; }

        /// <summary>
        /// Gets the character position before the indent occured.
        /// </summary>
        public int StartBeforeIndent { get; private set; }

        /// <summary>
        /// Gets the cache of string builders.
        /// </summary>
        public StringBuilderCache StringBuilders { get; }

        /// <summary>
        /// Gets the current stack of <see cref="Block"/> being processed.
        /// </summary>
        private List<Block> OpenedBlocks { get; }

        /// <summary>
        /// Gets or sets a value indicating whether to continue processing the current line.
        /// </summary>
        private bool ContinueProcessingLine { get; set; }

        /// <summary>
        /// Returns the next character in the line being processed. Update <see cref="Start"/> and <see cref="Column"/>.
        /// </summary>
        /// <returns>The next character or `\0` if end of line is reached</returns>
        public char NextChar()
        {
            var c = Line.CurrentChar;
            if (c == '\t')
            {
                Column = CharHelper.AddTab(Column);
            }
            else
            {
                Column++;
            }
            return Line.NextChar();
        }

        /// <summary>
        /// Returns the next character in the line taking into space taken by tabs. Update <see cref="Start"/> and <see cref="Column"/>.
        /// </summary>
        public void NextColumn()
        {
            var c = Line.CurrentChar;
            // If we are accross a tab, we should just add 1 column
            if (c == '\t' && CharHelper.IsAcrossTab(Column))
            {
                Column++;
            }
            else
            {
                Line.NextChar();
                Column++;
            }
        }

        /// <summary>
        /// Peeks a character at the specified offset from the current position in the line.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns>A character peeked at the specified offset</returns>
        public char PeekChar(int offset)
        {
            return Line.PeekChar(offset);
        }

        /// <summary>
        /// Restarts the indent from the current position.
        /// </summary>
        public void RestartIndent()
        {
            StartBeforeIndent = Start;
            ColumnBeforeIndent = Column;
        }

        /// <summary>
        /// Parses the indentation from the current position in the line, updating <see cref="StartBeforeIndent"/>, 
        /// <see cref="ColumnBeforeIndent"/>, <see cref="Start"/> and <see cref="Column"/> accordingly
        /// taking into account space taken by tabs.
        /// </summary>
        public void ParseIndent()
        {
            var c = CurrentChar;
            var previousStartBeforeIndent = StartBeforeIndent;
            var startBeforeIndent = Start;
            var previousColumnBeforeIndent = ColumnBeforeIndent;
            var columnBeforeIndent = Column;
            while (c !='\0')
            {
                if (c == '\t')
                {
                    Column = CharHelper.AddTab(Column);
                }
                else if (c == ' ')
                {
                    Column++;
                }
                else
                {
                    break;
                }
                c = Line.NextChar();
            }
            if (columnBeforeIndent == Column)
            {
                StartBeforeIndent = previousStartBeforeIndent;
                ColumnBeforeIndent = previousColumnBeforeIndent;
            }
            else
            {
                StartBeforeIndent = startBeforeIndent;
                ColumnBeforeIndent = columnBeforeIndent;
            }
        }

        /// <summary>
        /// Moves to the position to the specified column position, taking into account spaces in tabs.
        /// </summary>
        /// <param name="newColumn">The new column position to move the cursor to.</param>
        public void GoToColumn(int newColumn)
        {
            // Optimized path when we are moving above the previous start of indent
            if (newColumn >= ColumnBeforeIndent)
            {
                Line.Start = StartBeforeIndent;
                Column = ColumnBeforeIndent;
            }
            else
            {
                Line.Start = originalLineStart;
                Column = 0;
                ColumnBeforeIndent = 0;
                StartBeforeIndent = originalLineStart;
            }
            for (; Line.Start <= Line.End && Column < newColumn; Line.Start++)
            {
                var c = Line.Text[Line.Start];
                if (c == '\t')
                {
                    Column = CharHelper.AddTab(Column);
                }
                else
                {
                    if (!c.IsSpaceOrTab())
                    {
                        ColumnBeforeIndent = Column + 1;
                        StartBeforeIndent = Line.Start + 1;
                    }

                    Column++;
                }
            }
            if (Column > newColumn)
            {
                Column = newColumn;
                if (Line.Start > 0)
                {
                    Line.Start--;
                }
            }
        }

        /// <summary>
        /// Unwind any previous indent from the current character back to the first space.
        /// </summary>
        public void UnwindAllIndents()
        {
            // Find the previous first space on the current line
            var previousStart = Line.Start;
            for (; Line.Start > originalLineStart; Line.Start--)
            {
                var c = Line.PeekCharAbsolute(Line.Start - 1);
                if (c == 0)
                {
                    break;
                }
                if (!c.IsSpaceOrTab())
                {
                    break;
                }
            }
            var targetStart = Line.Start;
            // Nothing changed? Early exit
            if (previousStart == targetStart)
            {
                return;
            }

            // TODO: factorize the following code with what is done with GoToColumn

            // If we have found the first space, we need to recalculate the correct column
            Line.Start = originalLineStart;
            Column = 0;
            ColumnBeforeIndent = 0;
            StartBeforeIndent = originalLineStart;

            for (; Line.Start < targetStart; Line.Start++)
            {
                var c = Line.Text[Line.Start];
                if (c == '\t')
                {
                    Column = CharHelper.AddTab(Column);
                }
                else
                {
                    if (!c.IsSpaceOrTab())
                    {
                        ColumnBeforeIndent = Column + 1;
                        StartBeforeIndent = Line.Start + 1;
                    }

                    Column++;
                }
            }

            // Reset the indent
            ColumnBeforeIndent = Column;
            StartBeforeIndent = Start;
        }

        /// <summary>
        /// Moves to the position to the code indent (<see cref="ColumnBeforeIndent"/> + 4 spaces).
        /// </summary>
        /// <param name="columnOffset">The column offset to apply to this indent.</param>
        public void GoToCodeIndent(int columnOffset = 0)
        {
            GoToColumn(ColumnBeforeIndent + 4 + columnOffset);
        }

        /// <summary>
        /// Opens the specified block.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException">The block must be opened</exception>
        public void Open(Block block)
        {
            if (block == null) throw new ArgumentNullException(nameof(block));
            if (!block.IsOpen) throw new ArgumentException("The block must be opened", nameof(block));
            OpenedBlocks.Add(block);
        }

        /// <summary>
        /// Force closing the specified block.
        /// </summary>
        /// <param name="block">The block.</param>
        public void Close(Block block)
        {
            // If we close a block, we close all blocks above
            for (int i = OpenedBlocks.Count - 1; i >= 0; i--)
            {
                if (OpenedBlocks[i] == block)
                {
                    for (int j = OpenedBlocks.Count - 1; j >= i; j--)
                    {
                        Close(j);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Discards the specified block from the stack, remove from its parent.
        /// </summary>
        /// <param name="block">The block.</param>
        public void Discard(Block block)
        {
            for (int i = OpenedBlocks.Count - 1; i >= 1; i--)
            {
                if (OpenedBlocks[i] == block)
                {
                    block.Parent.Remove(block);
                    OpenedBlocks.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Processes a new line.
        /// </summary>
        /// <param name="newLine">The new line.</param>
        public void ProcessLine(StringSlice newLine)
        {
            CurrentLineStartPosition = newLine.Start;

            ContinueProcessingLine = true;

            ResetLine(newLine);

            TryContinueBlocks();

            // If the line was not entirely processed by pending blocks, try to process it with any new block
            TryOpenBlocks();

            // Close blocks that are no longer opened
            CloseAll(false);

            LineIndex++;
        }

        public BlockProcessor CreateChild()
        {
            var newState = parserStateCache.Get();
            return newState;
        }

        public void ReleaseChild()
        {
            if (this == root)
            {
                throw new InvalidOperationException("Cannot release the root parser state");
            }
            parserStateCache.Release(this);
        }

        internal bool IsOpen(Block block)
        {
            return OpenedBlocks.Contains(block);
        }

        /// <summary>
        /// Closes a block at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        private void Close(int index)
        {
            var block = OpenedBlocks[index];
            // If the pending object is removed, we need to remove it from the parent container
            if (block.Parser != null)
            {
                if (!block.Parser.Close(this, block))
                {
                    block.Parent?.Remove(block);
                }
                else
                {
                    // Invoke the Closed event
                    var blockClosed = block.Parser.GetClosedEvent;
                    blockClosed?.Invoke(this, block);
                }
            }
            OpenedBlocks.RemoveAt(index);
        }

        /// <summary>
        /// Closes all the blocks opened.
        /// </summary>
        /// <param name="force">if set to <c>true</c> [force].</param>
        internal void CloseAll(bool force)
        {
            // Close any previous blocks not opened
            for (int i = OpenedBlocks.Count - 1; i >= 1; i--)
            {
                var block = OpenedBlocks[i];

                // Stop on the first open block
                if (!force && block.IsOpen)
                {
                    break;
                }
                Close(i);
            }
            UpdateLastBlockAndContainer();
        }

        /// <summary>
        /// Mark all blocks in the stack as opened.
        /// </summary>
        private void OpenAll()
        {
            for (int i = 1; i < OpenedBlocks.Count; i++)
            {
                OpenedBlocks[i].IsOpen = true;
            }
        }
        
        /// <summary>
        /// Updates the <see cref="CurrentBlock"/> and <see cref="CurrentContainer"/>.
        /// </summary>
        /// <param name="stackIndex">Index of a block in a stack considered as the last block to update from.</param>
        private void UpdateLastBlockAndContainer(int stackIndex = -1)
        {
            currentStackIndex = stackIndex < 0 ? OpenedBlocks.Count - 1 : stackIndex;
            CurrentBlock = null;
            LastBlock = null;
            for (int i = OpenedBlocks.Count - 1; i >= 0; i--)
            {
                var block = OpenedBlocks[i];
                if (CurrentBlock == null)
                {
                    CurrentBlock = block;
                }

                var container = block as ContainerBlock;
                if (container != null)
                {
                    CurrentContainer = container;
                    LastBlock = CurrentContainer.LastChild;
                    break;
                }
            }
        }

        /// <summary>
        /// Tries to continue matching existing opened <see cref="Block"/>.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// A pending parser cannot add a new block when it is not the last pending block
        /// or
        /// The NewBlocks is not empty. This is happening if a LeafBlock is not the last to be pushed
        /// </exception>
        private void TryContinueBlocks()
        {
            // Set all blocks non opened. 
            // They will be marked as open in the following loop
            for (int i = 1; i < OpenedBlocks.Count; i++)
            {
                OpenedBlocks[i].IsOpen = false;
            }

            // Process any current block potentially opened
            for (int i = 1; i < OpenedBlocks.Count; i++)
            {
                var block = OpenedBlocks[i];

                ParseIndent();

                // If we have a paragraph block, we want to try to match other blocks before trying the Paragraph
                if (block is ParagraphBlock)
                {
                    break;
                }

                // Else tries to match the Default with the current line
                var parser = block.Parser;


                // If we have a discard, we can remove it from the current state
                UpdateLastBlockAndContainer(i);
                var result = parser.TryContinue(this, block);
                if (result == BlockState.Skip)
                {
                    continue;
                }

                if (result == BlockState.None)
                {
                    break;
                }

                RestartIndent();

                // In case the BlockParser has modified the BlockProcessor we are iterating on
                if (i >= OpenedBlocks.Count)
                {
                    i = OpenedBlocks.Count - 1;
                }

                // If a parser is adding a block, it must be the last of the list
                if ((i + 1) < OpenedBlocks.Count && NewBlocks.Count > 0)
                {
                    throw new InvalidOperationException("A pending parser cannot add a new block when it is not the last pending block");
                }

                // If we have a leaf block
                var leaf = block as LeafBlock;
                if (leaf != null && NewBlocks.Count == 0)
                {
                    ContinueProcessingLine = false;
                    if (!result.IsDiscard())
                    {
                        leaf.AppendLine(ref Line, Column, LineIndex, CurrentLineStartPosition);
                    }
                }

                // A block is open only if it has a Continue state.
                // otherwise it is a Break state, and we don't keep it opened
                block.IsOpen = result == BlockState.Continue || result == BlockState.ContinueDiscard;

                if (result == BlockState.BreakDiscard)
                {
                    ContinueProcessingLine = false;
                    break;
                }

                bool isLast = i == OpenedBlocks.Count - 1;
                if (ContinueProcessingLine)
                {
                    ProcessNewBlocks(result, false);
                }
                if (isLast || !ContinueProcessingLine)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// First phase of the process, try to open new blocks.
        /// </summary>
        private void TryOpenBlocks()
        {
            int previousStart = -1;
            while (ContinueProcessingLine)
            {
                // Security check so that the parser can't go into a crazy infinite loop if one extension is messing
                if (previousStart == Start)
                {
                    throw new InvalidOperationException($"The parser is in an invalid infinite loop while trying to parse blocks at line [{LineIndex}] with line [{Line}]");
                }
                previousStart = Start;

                // Eat indent spaces before checking the character
                ParseIndent();

                var parsers = Parsers.GetParsersForOpeningCharacter(CurrentChar);
                var globalParsers = Parsers.GlobalParsers;

                if (parsers != null)
                {
                    if (TryOpenBlocks(parsers))
                    {
                        RestartIndent();
                        continue;
                    }
                }

                if (globalParsers != null && ContinueProcessingLine)
                {
                    if (TryOpenBlocks(globalParsers))
                    {
                        RestartIndent();
                        continue;
                    }
                }

                break;
            }
        }

        /// <summary>
        /// Tries to open new blocks using the specified list of <see cref="BlockParser"/>
        /// </summary>
        /// <param name="parsers">The parsers.</param>
        /// <returns><c>true</c> to continue processing the current line</returns>
        private bool TryOpenBlocks(BlockParser[] parsers)
        {
            for (int j = 0; j < parsers.Length; j++)
            {
                var blockParser = parsers[j];
                if (Line.IsEmpty)
                {
                    ContinueProcessingLine = false;
                    break;
                }

                // UpdateLastBlockAndContainer the state of CurrentBlock and LastContainer
                UpdateLastBlockAndContainer();

                // If a block parser cannot interrupt a paragraph, and the last block is a paragraph
                // we can skip this parser

                var lastBlock = CurrentBlock;
                if (!blockParser.CanInterrupt(this, lastBlock))
                {
                    continue;
                }

                bool isLazyParagraph = blockParser is ParagraphBlockParser && lastBlock is ParagraphBlock;

                var result = isLazyParagraph
                    ? blockParser.TryContinue(this, lastBlock)
                    : blockParser.TryOpen(this);

                if (result == BlockState.None)
                {
                    // If we have reached a blank line after trying to parse a paragraph
                    // we can ignore it
                    if (isLazyParagraph && IsBlankLine)
                    {
                        ContinueProcessingLine = false;
                        break;
                    }
                    continue;
                }

                // Special case for paragraph
                UpdateLastBlockAndContainer();

                var paragraph = CurrentBlock as ParagraphBlock;
                if (isLazyParagraph && paragraph != null)
                {
                    Debug.Assert(NewBlocks.Count == 0);

                    if (!result.IsDiscard())
                    {
                        paragraph.AppendLine(ref Line, Column, LineIndex, CurrentLineStartPosition);
                    }

                    // We have just found a lazy continuation for a paragraph, early exit
                    // Mark all block opened after a lazy continuation
                    OpenAll();

                    ContinueProcessingLine = false;
                    break;
                }

                // Nothing found but the BlockParser may instruct to break, so early exit
                if (NewBlocks.Count == 0 && result == BlockState.BreakDiscard)
                {
                    ContinueProcessingLine = false;
                    break;
                }

                // If we have a container, we can retry to match against all types of block.
                ProcessNewBlocks(result, true);
                return ContinueProcessingLine;

                // We have a leaf node, we can stop
            }
            return false;
        }

        /// <summary>
        /// Processes any new blocks that have been pushed to <see cref="NewBlocks"/>.
        /// </summary>
        /// <param name="result">The last result of matching.</param>
        /// <param name="allowClosing">if set to <c>true</c> the processing of a new block will close existing opened blocks].</param>
        /// <exception cref="System.InvalidOperationException">The NewBlocks is not empty. This is happening if a LeafBlock is not the last to be pushed</exception>
        private void ProcessNewBlocks(BlockState result, bool allowClosing)
        {
            var newBlocks = NewBlocks;
            while (newBlocks.Count > 0)
            {
                var block = newBlocks.Pop();

                if (block.Parser == null)
                {
                    throw new InvalidOperationException($"The new block [{block.GetType()}] must have a valid Parser property");
                }

                block.Line = LineIndex;

                // If we have a leaf block
                var leaf = block as LeafBlock;
                if (leaf != null)
                {
                    if (!result.IsDiscard())
                    {
                        leaf.AppendLine(ref Line, Column, LineIndex, CurrentLineStartPosition);
                    }

                    if (newBlocks.Count > 0)
                    {
                        throw new InvalidOperationException(
                            "The NewBlocks is not empty. This is happening if a LeafBlock is not the last to be pushed");
                    }
                }

                if (allowClosing)
                {
                    // Close any previous blocks not opened
                    CloseAll(false);
                }

                // If previous block is a container, add the new block as a children of the previous block
                if (block.Parent == null)
                {
                    UpdateLastBlockAndContainer();
                    CurrentContainer.Add(block);
                }

                block.IsOpen = result.IsContinue();

                // Add a block BlockProcessor to the stack (and leave it opened)
                OpenedBlocks.Add(block);

                if (leaf != null)
                {
                    ContinueProcessingLine = false;
                    return;
                }
            }

            ContinueProcessingLine = !result.IsDiscard();
        }

        private void ResetLine(StringSlice newLine)
        {
            Line = newLine;
            Column = 0;
            ColumnBeforeIndent = 0;
            StartBeforeIndent = Start;
            originalLineStart = newLine.Start;
        }

        private void Reset()
        {
            Line = StringSlice.Empty;
            Column = 0;
            ColumnBeforeIndent = 0;
            StartBeforeIndent = 0;
            OpenedBlocks.Clear();
            NewBlocks.Clear();
        }

        private class BlockParserStateCache : ObjectCache<BlockProcessor>
        {
            private readonly BlockProcessor root;

            public BlockParserStateCache(BlockProcessor root)
            {
                this.root = root;
            }

            protected override BlockProcessor NewInstance()
            {
                return new BlockProcessor(root);
            }

            protected override void Reset(BlockProcessor instance)
            {
                instance.Reset();
            }
        }
    }
}