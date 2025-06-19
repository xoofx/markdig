// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Syntax;

namespace Markdig.Renderers.Roundtrip;

/// <summary>
/// A Roundtrip renderer for a <see cref="ListBlock"/>.
/// </summary>
/// <seealso cref="RoundtripObjectRenderer{ListBlock}" />
public class ListRenderer : RoundtripObjectRenderer<ListBlock>
{
    protected override void Write(RoundtripRenderer renderer, ListBlock listBlock)
    {
        renderer.RenderLinesBefore(listBlock);
        if (listBlock.IsOrdered)
        {
            for (var i = 0; i < listBlock.Count; i++)
            {
                var item = listBlock[i];
                var listItem = (ListItemBlock) item;
                renderer.RenderLinesBefore(listItem);

                var bws = listItem.TriviaBefore.ToString();
                var bullet = listItem.SourceBullet.ToString();
                var delimiter = listBlock.OrderedDelimiter;
                renderer.PushIndent(new string[] { $"{bws}{bullet}{delimiter}" });
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
        else
        {
            for (var i = 0; i < listBlock.Count; i++)
            {
                var item = listBlock[i];
                var listItem = (ListItemBlock) item;
                renderer.RenderLinesBefore(listItem);

                StringSlice bws = listItem.TriviaBefore;
                char bullet = listBlock.BulletType;
                StringSlice aws = listItem.TriviaAfter;

                renderer.PushIndent(new string[] { $"{bws}{bullet}{aws}" });
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
}