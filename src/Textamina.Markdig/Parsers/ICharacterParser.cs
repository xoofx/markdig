namespace Textamina.Markdig.Parsers
{
    public interface ICharacterParser<in TState>
    {
        char[] OpeningCharacters { get; }

        void Initialize(TState state);

        int Index { get; }
    }
}