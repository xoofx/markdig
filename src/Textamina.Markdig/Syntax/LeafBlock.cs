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

        public StringLineGroup Lines { get; set; }

        public Inline Inline { get; set; }

        public bool ProcessInlines { get; set; }

        public void AppendLine(ref StringSlice slice, int column, int line)
        {
            if (Lines == null)
            {
                Lines = new StringLineGroup();
            }

            var stringLine = new StringLine(ref slice, line, column, 0); // TODO: POSITION


            // Regular case, we are not in the middle of a tab
            if (slice.CurrentChar != '\t' || !CharHelper.IsAcrossTab(column))
            {
                Lines.Add(ref stringLine);
            }
            else
            {
                // We need to expand tabs to spaces
                var builder = StringBuilderCache.Local();
                for (int i = column; i < CharHelper.AddTab(column); i++)
                {
                    builder.Append(' ');
                }
                builder.Append(slice.Text, slice.Start + 1, slice.Length - 1);
                stringLine.Slice = new StringSlice(builder.ToString());
                Lines.Add(ref stringLine);
            }
        }

        public virtual StringSlice ToInlineSlice()
        {
            return Lines.ToSlice();
        }
    }
}