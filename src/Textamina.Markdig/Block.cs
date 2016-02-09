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

        protected Block(IBlockFactory factory, Block parent)
        {
            Factory = factory;
            Parent = parent;
            IsOpen = true;
        }

        internal bool IsOpen;

        public IBlockFactory Factory { get; }

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

    public interface IBlockFactory
    {
        bool Match(ref StringLiner liner);

        Block New(Block parent);
    }


    public abstract class BlockContainer : Block
    {

        protected BlockContainer(IBlockFactory factory, Block parent) : base(factory, parent)
        {
            Children = new List<Block>();
        }

        public List<Block> Children { get; }
    }

    public abstract class BlockLeaf
    {
        
    }


    public struct StringLiner
    {
        public StringBuilder Text;

        public int Column;

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public char NextChar()
        {
            return Column < Text.Length ? Text[Column++] : (char)0;
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public void Reset()
        {
            Column = 0;
        }
    }


    public class BlockQuote : BlockContainer
    {
        public static readonly IBlockFactory DefaultFactory = new FactoryInternal();

        public BlockQuote(Block parent) : base(DefaultFactory, parent)
        {
        }

        private class FactoryInternal : IBlockFactory
        {
            public bool Match(ref StringLiner liner)
            {
                // 5.1 Block quotes 
                // A block quote marker consists of 0-3 spaces of initial indent, plus (a) the character > together with a following space, or (b) a single character > not followed by a space.

                var c = liner.NextChar();
                if (Charset.IsSpace(c))
                {
                    c = liner.NextChar();
                    if (Charset.IsSpace(c))
                    {
                        c = liner.NextChar();
                        if (Charset.IsSpace(c))
                        {
                            c = liner.NextChar();
                        }
                    }
                }

                if (c != '>')
                {
                    return false;
                }

                c = liner.NextChar();
                if (Charset.IsSpace(c))
                {
                    liner.NextChar();
                }
                return true;
            }

            public Block New(Block parent)
            {
                return new BlockQuote(parent);
            }
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

        private List<IBlockFactory> blockFactories;
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

                int previousColumn = 0;
                foreach (var block in blocks)
                {
                    if (block.IsOpen)
                    {
                        if (block.Factory.Match(ref liner))
                        {
                            
                        }
                        else if (block is BlockContainer)
                        {
                            block.IsOpen = false;
                        }
                        // Leave the block open
                    }
                    else
                    {
                        liner.Column = previousColumn;
                    }

                    previousColumn = liner.Column;
                }

                // Take the last block
                var currentBlock = blocks.Peek();
                foreach (var blockFactory in blockFactories)
                {
                    if (blockFactory.Match(ref liner))
                    {
                        if (currentBlock.Factory != blockFactory)
                        {
                            var newBlock = blockFactory.New(currentBlock);
                            blocks.Push(newBlock);
                            currentBlock = newBlock;
                        }

                    }



                }









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

                if (c == '\0')
                {
                    // 2.3 Insecure characters
                    // For security reasons, the Unicode character U+0000 must be replaced with the REPLACEMENT CHARACTER (U+FFFD).
                    c = '\ufffd';
                }

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
        public static bool IsSpace(char c)
        {
            // 2.1 Characters and lines 
            // A space is U+0020.
            return c == ' ';
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