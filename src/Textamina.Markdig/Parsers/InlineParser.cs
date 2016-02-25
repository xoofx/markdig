namespace Textamina.Markdig.Parsers
{
    public abstract class InlineParser : ICharacterParser
    {
        public char[] OpeningCharacters { get; protected set; }

        public abstract bool Match(InlineParserState state);
    }
}