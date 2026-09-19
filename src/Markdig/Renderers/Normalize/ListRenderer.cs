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
    /// <summary>
    /// Writes the object to the specified renderer.
    /// </summary>
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
                renderer.PushHangingIndent($"{index.ToString(CultureInfo.InvariantCulture)}{listBlock.OrderedDelimiter} ");
                if (listItem.Count == 0)
                {
                    renderer.Write(""); // trigger writing of indent
                }
                else
                {
                    renderer.WriteChildren(listItem);
                }
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
                renderer.PushHangingIndent($"{renderer.Options.ListItemCharacter ?? listBlock.BulletType} ");
                if (listItem.Count == 0)
                {
                    renderer.Write(""); // trigger writing of indent
                }
                else
                {
                    renderer.WriteChildren(listItem);
                }
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
}
