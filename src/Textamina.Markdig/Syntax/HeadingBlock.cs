using System.Diagnostics;
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// Repressents a thematic break.
    /// </summary>
    [DebuggerDisplay("{GetType().Name} Line: {Line}, {Lines} Level: {Level}")]
    public class HeadingBlock : LeafBlock
    {
        public HeadingBlock(BlockParser parser) : base(parser)
        {
        }

        public int Level { get; set; }
    }
}