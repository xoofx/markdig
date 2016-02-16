using System.Collections.Generic;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class Inline : List<StringLiner>
    {
        public void TrimStart()
        {
            if (Count == 0)
            {
                return;
            }

            // Strip leading and 
            var liner = this[0];
            while (Utility.IsSpace(liner.Current))
            {
                liner.NextChar();
            }
        }

        public void TrimEnd()
        {
            if (Count == 0)
            {
                return;
            }

            var liner = this[Count - 1];
            for (int i = liner.End; i >= liner.Start; i--)
            {
                liner.End = i;
                if (!Utility.IsSpace(liner[i]))
                {
                    break;
                }
            }
        }

        public void Trim()
        {
            TrimStart();
            TrimEnd();
        }
    }
}