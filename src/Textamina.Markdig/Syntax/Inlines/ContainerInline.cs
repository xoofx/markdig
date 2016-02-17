namespace Textamina.Markdig.Syntax
{
    public abstract class ContainerInline : Inline
    {
        public Inline FirstChild { get; set; }
    }
}