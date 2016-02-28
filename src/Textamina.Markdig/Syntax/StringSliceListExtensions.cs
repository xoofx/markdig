namespace Textamina.Markdig.Syntax
{
    public static class StringSliceListExtensions
    {
        public static void Trim(this StringLineGroup slices)
        {
            for (int i = 0; i < slices.Count; i++)
            {
                slices.Lines[i].Slice.Trim();
            }
        }
         
    }
}