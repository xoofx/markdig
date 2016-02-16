namespace Textamina.Markdig.Syntax
{
    public struct StringSlice
    {
        public string Text;

        public int Start;

        public int End;

        public int Length
        {
            get { return End - Start; }

            set { End = Start + value; }
        }
    }
}