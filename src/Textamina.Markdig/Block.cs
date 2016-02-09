using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Textamina.Markdig
{
    public abstract class Block
    {
        protected List<StringBuilder> lines;

        protected Block(BlockMatcher matcher, Block parent)
        {
            Matcher = matcher;
            Parent = parent;
            IsOpen = true;
        }

        internal bool IsOpen;

        public BlockMatcher Matcher { get; }

        public Block Parent { get; }

        internal void Append(StringBuilder line)
        {
            if (lines == null)
            {
                lines = new List<StringBuilder>();
            }
            lines.Add(line);
        }
    }

    public abstract class BlockMatcher
    {
        public abstract MatchLineState Match(ref StringLiner liner, MatchLineState matchLineState, ref object matchContext);

        public abstract Block New(Block parent);
    }


    public abstract class BlockContainer : Block
    {

        protected BlockContainer(BlockMatcher matcher, Block parent) : base(matcher, parent)
        {
            Children = new List<Block>();
        }

        public List<Block> Children { get; }
    }

    public abstract class BlockLeaf : Block
    {
        protected BlockLeaf(BlockMatcher matcher, Block parent) : base(matcher, parent)
        {
        }
    }


    public struct StringLiner
    {
        public StringBuilder Text;

        public int Column;

        public char Current;

        private bool? isBlankLine;



        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public char NextChar()
        {
            return Current = (Column < Text.Length ? Text[Column++] : (char)0);
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public void Reset()
        {
            Column = 0;
        }

        public bool IsBlankLine()
        {
            if (isBlankLine.HasValue)
            {
                return isBlankLine.Value;
            }

            int columnSave = Column;
            while (Charset.IsSpace(Current) || Charset.IsTab(Current))
            {
                NextChar();
            }

            isBlankLine = IsEol;

            Column = columnSave;
            if (Column - 1 < Text.Length)
            {
                Current = Text[Column - 1];
            }

            return isBlankLine.Value;
        }

        public bool IsEol => Column == Text.Length;

        public bool SkipLeadingSpaces3()
        {
            // TODO: Handle correctly 2.2 Tabs 
            var c = NextChar();
            if (Charset.IsSpace(c))
            {
                c = NextChar();
                if (Charset.IsSpace(c))
                {
                    c = NextChar();
                    if (Charset.IsSpace(c))
                    {
                        c = NextChar();
                    }
                }
                return true;
            }
            return false;
        }
    }

    public class Document : BlockContainer
    {
        public Document() : base(null, null)
        {
        }
    }

    public class BlockParser
    {
        private StringLiner liner;
        private char? peekChar = null;
        private bool isEof;

        private List<BlockMatcher> blockMatchers;
        private Stack<Block> blocks;


        public BlockParser(TextReader reader)
        {
            Reader = reader;
            liner = new StringLiner() {Text = new StringBuilder()};
        }

        public TextReader Reader { get; }




        public Document Parse()
        {



            return null;
        }



        private void ParseLine()
        {
            while (isEof)
            {
                ReadLine();
                if (isEof && liner.Text.Length == 0)
                {
                    break;
                }

                // Skip leading spaces
                liner.SkipLeadingSpaces3();

                //int previousColumn = 0;
                //foreach (var block in blocks)
                //{
                //    if (block.IsOpen)
                //    {
                //        int minMatch = block.Matcher.Match(ref liner, TODO, ref TODO);
                //        if (minMatch >= 0)
                //        {
                            
                //        }
                //        else if (block is BlockContainer)
                //        {
                //            block.IsOpen = false;
                //        }
                //        // Leave the block open
                //    }
                //    else
                //    {
                //        liner.Column = previousColumn;
                //    }

                //    previousColumn = liner.Column;
                //}

                //// Take the last block
                //var currentBlock = blocks.Peek();
                //foreach (var blockFactory in blockMatchers)
                //{
                //    int minMatch = blockFactory.Match(ref liner, TODO, ref TODO);
                //    if (minMatch >= 0)
                //    {
                //        if (currentBlock.Matcher != blockFactory)
                //        {
                //            var newBlock = blockFactory.New(currentBlock);
                //            blocks.Push(newBlock);
                //            currentBlock = newBlock;
                //        }

                //    }



                //}









            }
        }

        private void ProcessLineWithCurrentBlock()
        {
            
        }

        







        private void ReadLine()
        {
            liner.Column = 0;
            var sb = liner.Text;
            sb.Clear();
            while (true)
            {
                char c;
                if (peekChar != null)
                {
                    c = peekChar.Value;
                    peekChar = null;
                }
                else
                {
                    var nextChar = Reader.Read();
                    if (nextChar < 0)
                    {
                        isEof = true;
                        return;
                    }
                    c = (char) nextChar;
                }

                // 2.3 Insecure characters
                c = Charset.EscapeInsecure(c);

                switch (c)
                {
                    case '\r':
                        var nextChar = Reader.Read();
                        if (nextChar > 0)
                        {
                            c = (char) nextChar;
                            if (c == '\n')
                            {
                                return;
                            }
                            peekChar = c;
                        }
                        return;
                    case '\n':
                        return;
                }
                sb.Append(c);
            }
        }
    }


    internal class Charset
    {
        [MethodImpl((MethodImplOptions)256)]
        public static bool IsWhiteSpace(char c)
        {
            // 2.1 Characters and lines 
            // A whitespace character is a space(U + 0020), tab(U + 0009), newline(U + 000A), line tabulation (U + 000B), form feed (U + 000C), or carriage return (U + 000D).
            return c == ' ' || c == '\t' || c == '\n' || c == '\v' || c == '\f' || c == '\r';
        }

        [MethodImpl((MethodImplOptions)256)]
        public static bool IsEof(char c)
        {
            return c == '\0';
        }

        [MethodImpl((MethodImplOptions)256)]
        public static bool IsSpace(char c)
        {
            // 2.1 Characters and lines 
            // A space is U+0020.
            return c == ' ';
        }

        [MethodImpl((MethodImplOptions)256)]
        public static bool IsTab(char c)
        {
            // 2.1 Characters and lines 
            // A space is U+0009.
            return c == '\t';
        }

        [MethodImpl((MethodImplOptions)256)]
        public static bool IsSpaceOrTab(char c)
        {
            return IsSpace(c) || IsTab(c);
        }

        [MethodImpl((MethodImplOptions)256)]
        public static char EscapeInsecure(char c)
        {
            // 2.3 Insecure characters
            // For security reasons, the Unicode character U+0000 must be replaced with the REPLACEMENT CHARACTER (U+FFFD).
            return c == '\0' ? '\ufffd' : c;
        }

        public static bool IsASCIIPunctuation(char c)
        {
            // 2.1 Characters and lines 
            // An ASCII punctuation character is !, ", #, $, %, &, ', (, ), *, +, ,, -, ., /, :, ;, <, =, >, ?, @, [, \, ], ^, _, `, {, |, }, or ~.
            switch (c)
            {
                case '!':
                case '"':
                case '#':
                case '$':
                case '%':
                case '&':
                case '\'':
                case '(':
                case ')':
                case '*':
                case '+':
                case ',':
                case '-':
                case '.':
                case '/':
                case ':':
                case ';':
                case '<':
                case '=':
                case '>':
                case '?':
                case '@':
                case '[':
                case '\\':
                case ']':
                case '^':
                case '_':
                case '`':
                case '{':
                case '|':
                case '}':
                case '~':
                    return true;
            }
            return false;
        }
    }
}