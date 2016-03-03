namespace Textamina.Markdig.Parsers
{
    public class ParserBase<T> : IMarkdownParser<T>
    {
        public char[] OpeningCharacters { get; set; }

        public virtual void Initialize(T state)
        {
        }

        public int Index { get; internal set; }
    }
}