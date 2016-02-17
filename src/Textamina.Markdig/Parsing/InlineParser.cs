namespace Textamina.Markdig.Parsing
{
    public abstract class InlineParser
    {
        public char[] FirstChars { get; protected set; }

        public abstract bool Match(MatchInlineState state);
    }
}