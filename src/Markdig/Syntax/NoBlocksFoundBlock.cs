using Markdig.Parsers;

namespace Markdig.Syntax
{
    /// <summary>
    /// Block representing a document with characters but no blocks. This can
    /// happen when an input document consists solely of trivia.
    /// </summary>
    public sealed class NoBlocksFoundBlock : LeafBlock
    {
        public NoBlocksFoundBlock(BlockParser parser) : base(parser)
        {
        }
    }
}
