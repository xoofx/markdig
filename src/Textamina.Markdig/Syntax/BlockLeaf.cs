using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public abstract class BlockLeaf : Block
    {
        protected BlockLeaf()
        {
        }

        public Inline Inline { get; set; }

        internal void Append(StringLiner line)
        {
            if (Inline == null)
            {
                Inline = new Inline();
            }
            Inline.Add(line);
        }
    }
}