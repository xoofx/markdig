using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class BlockQuote : BlockContainer
    {
        public static readonly BlockParser Parser = new ParserInternal();

        private class ParserInternal : BlockParser
        {
            public override MatchLineResult Match(ref StringLiner liner, ref Block block)
            {
                liner.SkipLeadingSpaces3();

                // 5.1 Block quotes 
                // A block quote marker consists of 0-3 spaces of initial indent, plus (a) the character > together with a following space, or (b) a single character > not followed by a space.
                var c = liner.Current;
                if (c != '>')
                {
                    return MatchLineResult.None;
                }

                c = liner.NextChar();
                if (Utility.IsSpace(c))
                {
                    liner.NextChar();
                }

                if (block == null)
                {
                    block = new BlockQuote();
                }

                return MatchLineResult.Continue;
            }
        }
    }
}