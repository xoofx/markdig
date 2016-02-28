using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsers
{
    public class ParserBase : ICharacterParser
    {
        public char[] OpeningCharacters { get; protected set; }

        public int Index { get; internal set; }
    }


    public abstract class InlineParser : ParserBase
    {
        public delegate void CloseBlockDelegate(InlineParserState state);

        public abstract bool Match(InlineParserState state, ref StringSlice slice);

        public CloseBlockDelegate OnCloseBlock { get; set; }
    }
}