using System;
using System.Text;
using Textamina.Markdig.Helpers;

namespace Textamina.Markdig.Syntax
{
    public class StringSliceList
    {
        private static readonly StringSlice[] Empty = new StringSlice[0];

        public StringSliceList()
        {
            Slices = Empty;
        }

        public StringSliceList(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            Append(new StringSlice(text));
        }

        public StringSlice[] Slices;

        public int Count;

        public void Append(ref StringSlice slice)
        {
            if (Count == Slices.Length) IncreaseCapacity();
            Slices[Count++] = slice;
        }

        public void Append(StringSlice slice)
        {
            if (Count == Slices.Length) IncreaseCapacity();
            Slices[Count++] = slice;
        }

        private void IncreaseCapacity()
        {
            int newCapacity = Slices.Length == 0 ? 4 : Slices.Length * 2;
            var newItems = new StringSlice[newCapacity];
            if (Count > 0)
            {
                Array.Copy(Slices, 0, newItems, 0, Count);
            }
            Slices = newItems;
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool replaceEndOfLineBySpace)
        {
            var newLine = replaceEndOfLineBySpace ? ' ' : '\n';
            var builder = StringBuilderCache.Local();
            for (int i = 0; i < Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(newLine);
                }
                if (!Slices[i].IsEndOfSlice)
                {
                    builder.Append(Slices[i].Text, Slices[i].Start, Slices[i].Length);
                }
            }
            var str = builder.ToString();
            builder.Clear();
            return str;
        }
    }
}