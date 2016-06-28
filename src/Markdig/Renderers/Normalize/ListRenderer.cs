// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Globalization;
using Markdig.Syntax;

namespace Markdig.Renderers.Normalize
{
    /// <summary>
    /// A Normalize renderer for a <see cref="ListBlock"/>.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Normalize.NormalizeObjectRenderer{Markdig.Syntax.ListBlock}" />
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
                foreach (var item in listBlock)
                {
                    var listItem = (ListItemBlock)item;
                    renderer.EnsureLine();

                    renderer.Write(index + "");
                    renderer.Write(listBlock.OrderedDelimiter);
                    renderer.Write(' ');
                    renderer.PushIndent("  ");  // TODO: output accurate indent
                    renderer.WriteChildren(listItem);
                    renderer.PopIndent();
                    switch (listBlock.BulletType)
                    {
                        case '1':
                            index++;
                            break;
                    }
                }
            }
            else
            {
                foreach (var item in listBlock)
                {
                    var listItem = (ListItemBlock)item;
                    renderer.EnsureLine();
                    renderer.Write("- ");
                    renderer.PushIndent("  ");
                    renderer.WriteChildren(listItem);
                    renderer.PopIndent();
                }
            }

            renderer.CompactParagraph = compact;
            renderer.WriteLine();
        }
    }
}