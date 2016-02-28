namespace Textamina.Markdig.Syntax.Inlines
{
    public class SoftlineBreakInline : LeafInline
    {
        public override string ToString()
        {
            return "\\n";
        }
    }
}