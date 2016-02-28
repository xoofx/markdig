namespace Textamina.Markdig.Parsers
{
    public class ParserBase<T> : ICharacterParser<T>
    {
        public char[] OpeningCharacters { get; protected set; }

        public virtual void Initialize(T state)
        {
        }

        public int Index { get; internal set; }
    }
}