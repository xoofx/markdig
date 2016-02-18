using System.Text;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class LinkReferenceDefinitionBlock : LeafBlock
    {
        public static readonly BlockParser Parser = new ParserInternal();

        private StringBuilder label;
        private StringBuilder url;
        private StringBuilder title;

        private class ParserInternal : BlockParser
        {
            public override MatchLineResult Match(MatchLineState state)
            {
                var line = state.Line;
                line.SkipLeadingSpaces3();

                var c = line.Current;

                if (c != '[')
                {
                    return MatchLineResult.None;
                }










                return MatchLineResult.None;

            }
        }
    }
}