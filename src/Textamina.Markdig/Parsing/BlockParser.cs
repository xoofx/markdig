using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsing
{
    public abstract class BlockParser
    {
        protected BlockParser()
        {
            // By default, all blocks can interrupt a paragraph except:
            // - setext heading
            // - indented code block
            // - a special HTML blocks
            CanInterruptParagraph = true;
        }

        public bool CanInterruptParagraph { get; protected set; }

        public abstract MatchLineResult Match(BlockParserState state);

        public virtual void Close(BlockParserState state)
        {
        }
    }
}