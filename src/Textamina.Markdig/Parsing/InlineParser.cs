using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsing
{
    public abstract class InlineParser
    {
        public char[] FirstChars { get; protected set; }

        public abstract bool Match(ref MatchInlineState state);

        public virtual void Close(ref MatchInlineState state, Inline inline)
        {
        }
    }
}