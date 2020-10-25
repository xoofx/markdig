// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Collections.Generic;
using Markdig.Helpers;
using Markdig.Syntax;

namespace Markdig.Renderers.Roundtrip
{
    /// <summary>
    /// A Normalize renderer for a <see cref="ListBlock"/>.
    /// </summary>
    /// <seealso cref="NormalizeObjectRenderer{ListBlock}" />
    public class ListRenderer : RoundtripObjectRenderer<ListBlock>
    {
        protected override void Write(RoundtripRenderer renderer, ListBlock listBlock)
        {
            renderer.RenderLinesBefore(listBlock);
            if (listBlock.IsOrdered)
            {
                int index = 0;
                if (listBlock.OrderedStart != null)
                {
                    switch (listBlock.BulletType)
                    {
                        case '1':
                            int.TryParse(listBlock.OrderedStart, out index);
                            break;
                    }
                }
                var writeLine = false;
                for (var i = 0; i < listBlock.Count; i++)
                {
                    var item = listBlock[i];
                    var listItem = (ListItemBlock) item;
                    if (writeLine)
                    {
                        renderer.WriteLine();
                    }

                    renderer.Write(listItem.BeforeWhitespace);
                    //renderer.Write(index.ToString(CultureInfo.InvariantCulture));
                    renderer.Write(listItem.SourceBullet);
                    renderer.Write(listBlock.OrderedDelimiter);
                    renderer.Write(' ');
                    renderer.PushIndent(new string(' ', IntLog10Fast(index) + 3));
                    renderer.WriteChildren(listItem);
                    renderer.PopIndent();
                    switch (listBlock.BulletType)
                    {
                        case '1':
                            index++;
                            break;
                    }
                    if (i + 1 < listBlock.Count && listBlock.IsLoose)
                    {
                        //renderer.EnsureLine();
                        renderer.WriteLine();
                    }
                    writeLine = true;
                }
            }
            else
            {
                for (var i = 0; i < listBlock.Count; i++)
                {
                    var item = listBlock[i];
                    var listItem = (ListItemBlock) item;
                    renderer.RenderLinesBefore(listItem);

                    StringSlice bws = listItem.BeforeWhitespace;
                    char bullet = listBlock.BulletType;
                    StringSlice aws = listItem.AfterWhitespace;

                    renderer.PushIndent(new List<string> { $@"{bws}{bullet}{aws}" });
                    if (listItem.Count == 0)
                    {
                        renderer.Write(""); // trigger writing of indent
                    }
                    else
                    {
                        renderer.WriteChildren(listItem);
                    }
                    renderer.PopIndent();

                    renderer.RenderLinesAfter(listItem);
                }
            }

            renderer.RenderLinesAfter(listBlock);
        }


        private static int IntLog10Fast(int input) =>
            (input < 10) ? 0 :
            (input < 100) ? 1 :
            (input < 1000) ? 2 :
            (input < 10000) ? 3 :
            (input < 100000) ? 4 :
            (input < 1000000) ? 5 :
            (input < 10000000) ? 6 :
            (input < 100000000) ? 7 :
            (input < 1000000000) ? 8 : 9;
    }
}