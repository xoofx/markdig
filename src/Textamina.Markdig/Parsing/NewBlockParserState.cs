


using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsing
{
    public interface IBlockParser
    {
        char[] OpeningCharacters { get; }

        BlockState TryOpen(BlockParserState state);

        BlockState TryContinue(BlockParserState state);

        bool Close(BlockParserState state, Block block);
    }

    public abstract class NewBlockParser : IBlockParser
    {
        public char[] OpeningCharacters { get; protected set; }

        public abstract BlockState TryOpen(BlockParserState state);

        public virtual BlockState TryContinue(BlockParserState state)
        {
            // By default we don't expect any newline
            return BlockState.None;
        }

        public virtual bool Close(BlockParserState state, Block block)
        { 
            // By default keep the block
            return true;
        }
    }


    public class ThematicBreakBlockParser : NewBlockParser
    {
        public ThematicBreakBlockParser()
        {
            OpeningCharacters = new [] {'-', '_', '*'};
        }

        public override BlockState TryOpen(BlockParserState state)
        {
            var liner = state.Line;
            // 4.1 Thematic breaks 
            // A line consisting of 0-3 spaces of indentation, followed by a sequence of three or more matching -, _, or * characters, each followed optionally by any number of spaces
            var c = liner.Current;

            var matchChar = liner.Current;
            var count = 1;
            c = liner.NextChar();
            bool hasSpacesSinceLastMatch = false;
            bool hasInnerSpaces = false;
            while (c != '\0')
            {
                if (c == matchChar)
                {
                    if (hasSpacesSinceLastMatch)
                    {
                        hasInnerSpaces = true;
                    }

                    count++;
                }
                else if (!c.IsSpace())
                {
                    return BlockState.None;
                }
                else if (c.IsSpace())
                {
                    hasSpacesSinceLastMatch = true;
                }
                c = liner.NextChar();
            }

            // If it as less than 3 chars or it is a setex heading and we are already in a paragraph, let the paragraph handle it
            var previousParagraph = state.LastBlock as ParagraphBlock;

            var isSetexHeading = previousParagraph != null && matchChar == '-' && !hasInnerSpaces;
            if (isSetexHeading)
            {
                var parent = previousParagraph.Parent;
                if (parent is QuoteBlock || (parent is ListItemBlock && previousParagraph.Column != column))
                {
                    isSetexHeading = false;
                }
            }

            if (count < 3 || isSetexHeading)
            {
                return BlockState.None;
            }

            state.NewBlocks.Push(new BreakBlock(this) {Column = column});
            return BlockState.LastDiscard;
        }
    }


    public enum BlockState
    {
        None,

        Skip,

        Continue,

        ContinueDiscard,

        Last,

        LastDiscard
    }
}