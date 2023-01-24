// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Globalization;
using Markdig.Syntax;

namespace Markdig.Renderers.Normalize;

/// <summary>
/// A Normalize renderer for a <see cref="ListBlock"/>.
/// </summary>
/// <seealso cref="NormalizeObjectRenderer{ListBlock}" />
public class ListRenderer : NormalizeObjectRenderer<ListBlock>
{
    protected override void Write(NormalizeRenderer renderer, ListBlock listBlock)
    {
        renderer.EnsureLine();
        var compact = renderer.CompactParagraph;
        renderer.CompactParagraph = !listBlock.IsLoose;
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
            for (var i = 0; i < listBlock.Count; i++)
            {
                var item = listBlock[i];
                var listItem = (ListItemBlock) item;
                renderer.EnsureLine();

                renderer.Write(index.ToString(CultureInfo.InvariantCulture));
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
                    renderer.EnsureLine();
                    renderer.WriteLine();
                }
            }
        }
        else
        {
            for (var i = 0; i < listBlock.Count; i++)
            {
                var item = listBlock[i];
                var listItem = (ListItemBlock) item;
                renderer.EnsureLine();
                renderer.Write(renderer.Options.ListItemCharacter ?? listBlock.BulletType);
                renderer.Write(' ');
                renderer.PushIndent("  ");
                renderer.WriteChildren(listItem);
                renderer.PopIndent();
                if (i + 1 < listBlock.Count && listBlock.IsLoose)
                {
                    renderer.EnsureLine();
                    renderer.WriteLine();
                }
            }
        }
        renderer.CompactParagraph = compact;

        renderer.FinishBlock(true);
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