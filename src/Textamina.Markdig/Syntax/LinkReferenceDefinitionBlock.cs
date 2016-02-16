using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class LinkReferenceDefinitionBlock : LeafBlock
    {
        public static readonly BlockParser Parser = new ParserInternal();


        private class ParserInternal : BlockParser
        {
            public override MatchLineResult Match(ref MatchLineState state)
            {




                return MatchLineResult.None;

            }
        }
    }
}