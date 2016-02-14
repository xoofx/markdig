using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public abstract class BlockBuilder
    {
        public abstract bool Match(ref StringLiner liner, ref Block block);
    }
}