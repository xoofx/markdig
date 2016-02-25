using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class QuoteBlock : ContainerBlock
    {
        public new static readonly BlockParser Parser = new ParserInternal();

        public QuoteBlock(BlockParser parser) : base(parser)
        {
        }

        private class ParserInternal : BlockParser
        {
            public override MatchLineResult Match(BlockParserState state)
            {
                if (state.IsCodeIndent)
                {
                    return MatchLineResult.None;
                }

                // 5.1 Block quotes 
                // A block quote marker consists of 0-3 spaces of initial indent, plus (a) the character > together with a following space, or (b) a single character > not followed by a space.
                var c = state.CurrentChar;
                var column = state.Column;
                if (c != '>')
                {
                    if (state.Pending != null && state.IsBlankLine)
                    {
                        state.Close(state.Pending);
                        return MatchLineResult.None;
                    }
                    return MatchLineResult.None;
                }

                c = state.NextChar();
                if (c.IsSpace())
                {
                    state.NextChar();
                }

                if (state.Pending == null)
                {
                    state.NewBlocks.Push(new QuoteBlock(this) { Column = column });
                }

                return MatchLineResult.Continue;
            }
        }
    }
}