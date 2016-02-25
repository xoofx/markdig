using System;
using System.Collections.Generic;
using System.Diagnostics;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsers
{
    public class BlockParserState
    {
        private readonly ParserList<BlockParser> blockParsers;
        private int currentStackIndex;

        public BlockParserState(StringBuilderCache stringBuilders, Document root)
        {
            if (stringBuilders == null) throw new ArgumentNullException(nameof(stringBuilders));
            if (root == null) throw new ArgumentNullException(nameof(root));
            StringBuilders = stringBuilders;
            Root = root;
            NewBlocks = new Stack<Block>();
            root.IsOpen = true;
            Stack = new List<Block> {root};

            blockParsers = new ParserList<BlockParser>()
            {
                new ThematicBreakParser(),
                new HeadingBlockParser(),
                new QuoteBlockParser(),
                //ListBlock.Default,

                new HtmlBlockParser(),
                new FencedCodeBlockParser(),
                new CodeBlockParser(),
                new ParagraphBlockParser(),
            };
            blockParsers.Initialize();
        }

        public List<Block> Stack { get; }

        public Stack<Block> NewBlocks { get; }

        public ContainerBlock CurrentContainer { get; private set; }

        public Block LastBlock { get; private set; }

        public Block NextContinue => currentStackIndex + 1 < Stack.Count ? Stack[currentStackIndex + 1] : null;

        public Document Root { get; }

        public bool ContinueProcessingLine { get; set; }

        public StringSlice Line;

        public int LineIndex { get; private set; }

        public bool IsBlankLine => CurrentChar == '\0';

        public bool IsEndOfLine => Line.IsEndOfSlice;

        public char CurrentChar => Line.CurrentChar;

        public char NextChar()
        {
            var c = Line.CurrentChar;
            if (c == '\t')
            {
                Column = ((Column + 3) >> 2) << 2;
            }
            else
            {
                Column++;
            }
            return Line.NextChar();
        }

        public char CharAt(int index) => Line[index];

        public int Start => Line.Start;

        public int EndOffset => Line.End;

        public int Indent => Column - ColumnBeforeIndent;

        public bool IsCodeIndent => Indent >= 4;

        public int ColumnBeforeIndent { get; private set; }

        public int StartBeforeIndent { get; private set; }

        public int Column { get; set; }

        public StringBuilderCache StringBuilders { get; }

        public char PeekChar(int offset)
        {
            return Line.PeekChar(offset);
        }

        private void ResetLine(StringSlice newLine)
        {
            Line = newLine;
            Column = 0;
            ColumnBeforeIndent = 0;
            StartBeforeIndent = 0;
        }

        public void ResetIndent()
        {
            StartBeforeIndent = Start;
            ColumnBeforeIndent = Column;
        }

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
                    Column = ((Column + 4) >> 2) << 2;
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

        public void MoveTo(int newStart)
        {
            Line.Start = 0;
            Column = 0;
            ColumnBeforeIndent = 0;
            StartBeforeIndent = 0;
            for (; Line.Start < newStart; Line.Start++)
            {
                var c = Line.Text[Line.Start];
                if (c == '\t')
                {
                    Column = ((Column + 3) >> 2) << 2;
                }
                else
                {
                    Column++;
                }
            }
        }

        public void Close(Block block)
        {
            // If we close a block, we close all blocks above
            for (int i = Stack.Count - 1; i >= 1; i--)
            {
                if (Stack[i] == block)
                {
                    for (int j = Stack.Count - 1; j >= i; j--)
                    {
                        Close(j);
                    }
                    break;
                }
            }
        }

        public void Discard(Block block)
        {
            for (int i = Stack.Count - 1; i >= 1; i--)
            {
                if (Stack[i] == block)
                {
                    block.Parent.Children.Remove(block);
                    Stack.RemoveAt(i);
                    break;
                }
            }
        }

        public void Close(int index)
        {
            var block = Stack[index];
            // If the pending object is removed, we need to remove it from the parent container
            if (!block.Parser.Close(this, block))
            {
                block.Parent?.Children.Remove(block);
            }
            Stack.RemoveAt(index);
        }

        public void CloseAll(bool force)
        {
            // Close any previous blocks not opened
            for (int i = Stack.Count - 1; i >= 1; i--)
            {
                var block = Stack[i];

                // Stop on the first open block
                if (!force && block.IsOpen)
                {
                    break;
                }
                Close(i);
            }
            UpdateLast(-1);
        }

        public void ProcessLine(string newLine)
        {
            ContinueProcessingLine = true;

            ResetLine(new StringSlice(newLine));
            LineIndex++;

            TryContinueBlocks();

            //// If we have already reached eol and the last block was a paragraph
            //// we close it
            //if (Line.IsEndOfSlice)
            //{
            //    int index = Stack.Count - 1;
            //    if (Stack[index] is ParagraphBlock)
            //    {
            //        Close(index);
            //        return;
            //    }
            //}

            // If the line was not entirely processed by pending blocks, try to process it with any new block
            TryOpenBlocks();

            // Close blocks that are no longer opened
            CloseAll(false);
        }

        private void OpenAll()
        {
            for (int i = 1; i < Stack.Count; i++)
            {
                Stack[i].IsOpen = true;
            }
        }

        internal void UpdateLast(int stackIndex)
        {
            currentStackIndex = stackIndex < 0 ? Stack.Count - 1 : stackIndex;
            LastBlock = null;
            for (int i = Stack.Count - 1; i >= 0; i--)
            {
                var block = Stack[i];
                if (LastBlock == null)
                {
                    LastBlock = block;
                }

                var container = block as ContainerBlock;
                if (container != null)
                {
                    CurrentContainer = container;
                    break;
                }
            }
        }

        private void TryContinueBlocks()
        {
            // Set all blocks non opened. 
            // They will be marked as open in the following loop
            for (int i = 1; i < Stack.Count; i++)
            {
                Stack[i].IsOpen = false;
            }

            // Process any current block potentially opened
            for (int i = 1; i < Stack.Count; i++)
            {
                var block = Stack[i];

                ParseIndent();

                // If we have a paragraph block, we want to try to match other blocks before trying the Paragraph
                if (block is ParagraphBlock)
                {
                    break;
                }

                // Else tries to match the Default with the current line
                var parser = block.Parser;


                // If we have a discard, we can remove it from the current state
                UpdateLast(i);
                var result = parser.TryContinue(this, block);
                if (result == BlockState.Skip)
                {
                    continue;
                }

                if (result == BlockState.None)
                {
                    break;
                }

                ResetIndent();

                // In case the BlockParser has modified the blockParserState we are iterating on
                if (i >= Stack.Count)
                {
                    i = Stack.Count - 1;
                }

                // If a parser is adding a block, it must be the last of the list
                if ((i + 1) < Stack.Count && NewBlocks.Count > 0)
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
                        leaf.AppendLine(ref Line);
                    }

                    if (NewBlocks.Count > 0)
                    {
                        throw new InvalidOperationException(
                            "The NewBlocks is not empty. This is happening if a LeafBlock is not the last to be pushed");
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

                bool isLast = i == Stack.Count - 1;
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

        private void TryOpenBlocks()
        {
            while (ContinueProcessingLine)
            {
                // Eat indent spaces before checking the character
                ParseIndent();

                var parsers = blockParsers.GetParsersForOpeningCharacter(CurrentChar);
                var globalParsers = blockParsers.GlobalParsers;

                if (parsers != null)
                {
                    if (TryOpenBlocks(parsers))
                    {
                        ResetIndent();
                        continue;
                    }
                }

                if (globalParsers != null && ContinueProcessingLine)
                {
                    if (TryOpenBlocks(globalParsers))
                    {
                        ResetIndent();
                        continue;
                    }
                }

                break;
            }
        }

        private bool TryOpenBlocks(BlockParser[] parsers)
        {
            for (int j = 0; j < parsers.Length; j++)
            {
                var blockParser = parsers[j];
                if (Line.IsEndOfSlice)
                {
                    ContinueProcessingLine = false;
                    break;
                }

                // UpdateLast the state of LastBlock and LastContainer
                UpdateLast(-1);

                // If a block parser cannot interrupt a paragraph, and the last block is a paragraph
                // we can skip this parser

                var lastBlock = LastBlock;
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
                UpdateLast(-1);

                var paragraph = LastBlock as ParagraphBlock;
                if (isLazyParagraph && paragraph != null)
                {
                    Debug.Assert(NewBlocks.Count == 0);

                    if (!result.IsDiscard())
                    {
                        paragraph.AppendLine(ref Line);
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

        private void ProcessNewBlocks(BlockState result, bool allowClosing)
        {
            var newBlocks = NewBlocks;
            while (newBlocks.Count > 0)
            {
                var block = newBlocks.Pop();

                block.Line = LineIndex;

                // If we have a leaf block
                var leaf = block as LeafBlock;
                if (leaf != null)
                {
                    if (!result.IsDiscard())
                    {
                        leaf.AppendLine(ref Line);
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
                    CurrentContainer.Children.Add(block);
                    block.Parent = CurrentContainer;
                }

                block.IsOpen = result.IsContinue();

                // Add a block blockParserState to the stack (and leave it opened)
                Stack.Add(block);

                if (leaf != null)
                {
                    ContinueProcessingLine = false;
                    return;
                }
            }
            ContinueProcessingLine = true;
        }

    }
}