using System.Diagnostics;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Syntax
{
    [DebuggerDisplay("{GetType().Name} Line: {Line}, {Lines}")]
    public abstract class LeafBlock : Block
    {
        protected LeafBlock(BlockParser parser) : base(parser)
        {
            ProcessInlines = false;
        }

        public StringSliceList Lines { get; set; }

        public Inline Inline { get; set; }

        public bool ProcessInlines { get; set; }

        public void AppendLine(ref StringSlice line)
        {
            if (Lines == null)
            {
                Lines = new StringSliceList();
            }
            Lines.Append(ref line);
        }

        public virtual string ToInlineText()
        {
            return Lines.ToString();
        }
    }
}