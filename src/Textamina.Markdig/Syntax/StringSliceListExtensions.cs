namespace Textamina.Markdig.Syntax
{
    public static class StringSliceListExtensions
    {
        public static void Trim(this StringSliceList slices)
        {
            for (int i = 0; i < slices.Count; i++)
            {
                slices.Slices[i].Trim();
            }
        }
         
    }
}