using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsers
{
    public interface ICharacterParser<TState>
    {
        char[] OpeningCharacters { get; }

        void Initialize(TState state);

        int Index { get; }
    }

    public interface IBlockParser<T> : ICharacterParser<T>
    {
        bool CanInterrupt(BlockParserState state, Block block);

        BlockState TryOpen(BlockParserState state);

        BlockState TryContinue(BlockParserState state, Block block);

        bool Close(BlockParserState state, Block block);
    }
}