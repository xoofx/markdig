using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public abstract class Inline
    {

    }

    public abstract class LeafInline : Inline
    {
        public StringSlice Literal;
    }

    public abstract class ContainerInline : Inline
    {
        public Inline FirstChild { get; set; }

        public Inline NextSibling { get; set; }
    }





}