using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsers
{
    public abstract class BlockParser : ParserBase<BlockParserState>
    {
        protected BlockParser()
        {
        }

        public bool HasOpeningCharacter(char c)
        {
            if (OpeningCharacters != null)
            {
                for (int i = 0; i < OpeningCharacters.Length; i++)
                {
                    if (OpeningCharacters[i] == c)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual bool CanInterrupt(BlockParserState state, Block block)
        {
            // By default, all blocks can interrupt a paragraph except:
            // - setext heading
            // - indented code block
            // - a special HTML blocks
            return true;
        }

        public abstract BlockState TryOpen(BlockParserState state);

        public virtual BlockState TryContinue(BlockParserState state, Block block)
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
}