using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsers
{
    public interface ICharacterParser
    {
        char[] OpeningCharacters { get; }

        int Index { get; }
    }

    public interface IBlockParser : ICharacterParser
    {
        bool CanInterrupt(BlockParserState state, Block block);

        BlockState TryOpen(BlockParserState state);

        BlockState TryContinue(BlockParserState state, Block block);

        bool Close(BlockParserState state, Block block);
    }
}