using System.Collections.Generic;
using System.IO;
using System.Text;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsing
{
    public class MarkdownParser
    {
        private StringLiner liner;
        private bool isEof;

        private readonly List<BlockMatcher> matchers;
        private readonly Stack<BlockMatcherState> states;
        private readonly List<StringLiner> liners;
        private readonly Document document;

        public MarkdownParser(TextReader reader)
        {
            document = new Document();
            Reader = reader;
            matchers = new List<BlockMatcher>();
            liner = new StringLiner() {Text = new StringBuilder()};
            states = new Stack<BlockMatcherState>();
            liners = new List<StringLiner>();
            matchers = new List<BlockMatcher>()
            {
                BlockQuote.DefaultMatcher,
                //Break.DefaultMatcher,
                //CodeBlock.DefaultMatcher, 
                //FencedCodeBlock.DefaultMatcher,
                //Heading.DefaultMatcher,
                Paragraph.DefaultMatcher,
            };

            states.Push(new BlockMatcherState(null, MatchLineState.None, null, 0) { Block = document});
        }

        public TextReader Reader { get; }

        public Document Parse()
        {
            ParseLines();
            return document;
        }

        private void ParseLines()
        {
            while (!isEof)
            {
                ReadLine();

                int currentLineIndex = liners.Count;
                liners.Add(liner);
                bool processLiner = ProcessPendingBlocks(false);

                // If the line was not entirely processed by pending blocks, try to process it with any new block
                while (processLiner)
                {
                    // Take the last block
                    foreach (var matcher in matchers)
                    {
                        if (liner.IsEol)
                        {
                            processLiner = false;
                            break;
                        }

                        object matchContext = null;
                        var saveLiner = liner;
                        var lineState = matcher.Match(ref liner, MatchLineState.None, ref matchContext);
                        if (lineState == MatchLineState.Discard)
                        {
                            liner = saveLiner;
                            continue;
                        }

                        if (!matcher.IsContainer)
                        {
                            processLiner = false;
                        }

                        var matcherState = new BlockMatcherState(matcher, lineState, matchContext, currentLineIndex);
                        if (lineState != MatchLineState.Continue)
                        {
                            // We have a MatchLineState.Break
                            var block = matcher.New();
                            var leaf = block as BlockLeaf;
                            if (!liner.IsEol && leaf != null)
                            {
                                leaf.Append(liner);
                            }
                            matcherState.Block = block;
                        }

                        states.Push(matcherState);
                    }
                }
            }

            ProcessPendingBlocks(true);
        }


        private bool ProcessPendingBlocks(bool closeContainer)
        {
            int currentLineIndex = liners.Count - 1;
            bool processLiner = true;

            Block previousBlock = null;
            // Process any current block potentially opened
            foreach(var state in states)
            {
                // If we have a container and we are in a closing mode, close them
                if (closeContainer && state.Matcher.IsContainer)
                {
                    state.Block = state.Matcher.New();
                }

                // If the current matcher has already a block and the parent is a container
                // we can remove it from the current blocks
                if (state.Block != null)
                {
                    BlockContainer parentContainer = null;
                    if (i == 0)
                    {
                        parentContainer = document;
                    }
                    else
                    {
                        parentContainer = states[i - 1].Block as BlockContainer;
                    }

                    if (!closeContainer && parentContainer != null)
                    {
                        state.Block.Parent = parentContainer;
                        parentContainer.Children.Add(state.Block);
                        states.RemoveAt(i);
                        i--;
                    }

                    continue;
                }

                // Else tries to match the matcher with the current line
                var matcher = state.Matcher;
                var saveLiner = liner;
                var lineState = matcher.Match(ref liner, state.State, ref state.Context);

                // If we have a discard, we can remove it from the current state
                if (lineState == MatchLineState.Discard)
                {
                    // Restore the liner where it was
                    liner = saveLiner;
                    states.Pop();
                    break;
                }

                if (lineState >= MatchLineState.Break)
                {
                    var block = matcher.New();
                    state.Block = block;

                    // If it is a leaf content, we need to grab the content
                    if (!matcher.IsContainer)
                    {
                        var leaf = (BlockLeaf)block;

                        switch (lineState)
                        {
                            case MatchLineState.Break:
                                state.LineIndexEnd = currentLineIndex;
                                break;
                            case MatchLineState.BreakAndKeepCurrent:
                                state.LineIndexEnd = currentLineIndex - 1;
                                break;
                            case MatchLineState.BreakAndKeepOnlyIfEof:
                                if (isEof)
                                {
                                    state.LineIndexEnd = currentLineIndex - 1;
                                }
                                break;
                        }

                        for (int j = state.LineIndexStart; j <= state.LineIndexEnd; j++)
                        {
                            var linerToAdd = liners[j];
                            if (!linerToAdd.IsEol)
                            {
                                leaf.Append(linerToAdd);
                            }
                        }

                        for (int j = states.Count - 1; j > i; j--)
                        {
                            states.RemoveAt(i);
                            i--;
                        }
                    }
                }

                // Break on any 
                if (!matcher.IsContainer)
                {
                    processLiner = false;
                    break;
                }

                if (liner.IsEol)
                {
                    processLiner = false;
                }
            }

            return processLiner;
        }

        private void ReadLine()
        {
            liner.Text = new StringBuilder();
            var sb = liner.Text;
            bool isBlankLine = true;
            while (true)
            {
                var nextChar = Reader.Read();
                if (nextChar < 0)
                {
                    isEof = true;
                    break;
                }
                var c = (char) nextChar;

                // 2.3 Insecure characters
                c = Utility.EscapeInsecure(c);

                // Go to next char, expecting most likely a \n, otherwise skip it
                // TODO: Should we treat it as an error in no \n is following?
                if (c == '\r')
                {
                    continue;
                }

                if (c == '\n')
                {
                    break;
                }

                if (isBlankLine && !Utility.IsSpaceOrTab(c))
                {
                    isBlankLine = false;
                }
                sb.Append(c);
            }

            liner.Initialize(isBlankLine);
        }

        private class BlockMatcherState
        {
            public BlockMatcherState(BlockMatcher matcher, MatchLineState state, object context, int lineIndexStart)
            {
                Matcher = matcher;
                State = state;
                Context = context;
                LineIndexStart = lineIndexStart;
                LineIndexEnd = lineIndexStart;
            }

            public BlockMatcher Matcher;

            public MatchLineState State;

            public object Context;

            public int LineIndexStart;

            public int LineIndexEnd;

            public Block Block;
        }
    }
}