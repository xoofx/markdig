using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsers
{
    public abstract class InlineParser : ICharacterParser
    {
        public delegate void CloseBlockDelegate(InlineParserState state);

        public char[] OpeningCharacters { get; protected set; }

        public abstract bool Match(InlineParserState state, ref StringSlice slice);

        public CloseBlockDelegate OnCloseBlock { get; set; }
    }
}