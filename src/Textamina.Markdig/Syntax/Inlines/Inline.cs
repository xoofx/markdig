using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public abstract class Inline
    {
        public Inline NextSibling { get; set; }
    }
}