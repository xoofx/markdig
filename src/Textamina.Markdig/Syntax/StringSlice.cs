namespace Textamina.Markdig.Syntax
{
    public struct StringSlice
    {
        public StringSlice(string text, int start, int end)
        {
            Text = text;
            Start = start;
            End = end;
        }

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