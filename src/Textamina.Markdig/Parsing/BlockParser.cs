using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsing
{
    public abstract class BlockParser
    {
        public abstract MatchLineResult Match(ref StringLiner liner, ref Block block);
    }
}