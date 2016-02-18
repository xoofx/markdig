using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public abstract class LeafBlock : Block
    {
        protected LeafBlock()
        {
            Lines = new StringLineGroup();
        }

        public StringLineGroup Lines { get; set; }

        public Inline Inline { get; set; }

        public bool NoInline { get; set; }

        internal void Append(StringLine line)
        {
            Lines.Add(line);
        }
    }
}