using System.Diagnostics;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Syntax
{
    [DebuggerDisplay("{GetType().Name} Line: {Line}, {Lines}")]
    public abstract class LeafBlock : Block
    {
        protected LeafBlock(BlockParser parser) : base(parser)
        {
        }

        public StringSliceList Lines { get; set; }

        public Inline Inline { get; set; }

        public bool ProcessInlines { get; set; }

        public void AppendLine(ref StringSlice slice, int column, int line1)
        {
            if (Lines == null)
            {
                Lines = new StringSliceList();
            }

            // Regular case, we are not in the middle of a tab
            if (slice.CurrentChar != '\t' || (column & 3) == 0)
            {
                Lines.Add(ref slice);
            }
            else
            {
                // We need to expand tabs to spaces
                var builder = StringBuilderCache.Local();
                for (int i = column; i < (((column + 4) >> 2) << 2); i++)
                {
                    builder.Append(' ');
                }
                builder.Append(slice.Text, slice.Start + 1, slice.Length - 1);
                var newSlice = new StringSlice(builder.ToString());
                Lines.Add(ref newSlice);
            }
        }

        public virtual StringSlice ToInlineSlice()
        {
            return Lines.ToSlice();
        }
    }
}