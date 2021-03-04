using Markdig.Parsers;

namespace Markdig.Syntax
{
    /// <summary>
    /// Block representing a document with characters but no blocks. This can
    /// happen when an input document consists solely of trivia.
    /// </summary>
    public sealed class EmptyBlock  : LeafBlock
    {
        public EmptyBlock (BlockParser parser) : base(parser)
        {
        }
    }
}
