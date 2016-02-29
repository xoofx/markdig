namespace Textamina.Markdig.Parsers
{
    public interface IMarkdownParser<in TState>
    {
        char[] OpeningCharacters { get; }

        void Initialize(TState state);

        int Index { get; }
    }
}