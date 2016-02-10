using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public abstract class BlockMatcher
    {
        public bool IsContainer { get; protected set; }

        public abstract MatchLineState Match(ref StringLiner liner, MatchLineState matchLineState, ref object matchContext);

        public abstract Block New();
    }
}