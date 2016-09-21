// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace Markdig.Extensions.DefinitionLists
{
    /// <summary>
    /// A HTML renderer for <see cref="DefinitionList"/>, <see cref="DefinitionItem"/> and <see cref="DefinitionTerm"/>.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Html.HtmlObjectRenderer{Markdig.Extensions.DefinitionLists.DefinitionList}" />
    public class HtmlDefinitionListRenderer : HtmlObjectRenderer<DefinitionList>
    {
        protected override void Write(HtmlRenderer renderer, DefinitionList list)
        {
            renderer.EnsureLine();
            renderer.Write("<dl").WriteAttributes(list).WriteLine(">");
            foreach (var item in list)
            {
                bool hasOpendd = false;
                var definitionItem = (DefinitionItem) item;
                int countdd = 0;
                bool lastWasSimpleParagraph = false;
                for (int i = 0; i < definitionItem.Count; i++)
                {
                    var definitionTermOrContent = definitionItem[i];
                    var definitionTerm = definitionTermOrContent as DefinitionTerm;
                    if (definitionTerm != null)
                    {
                        if (hasOpendd)
                        {
                            if (!lastWasSimpleParagraph)
                            {
                                renderer.EnsureLine();
                            }
                            renderer.WriteLine("</dd>");
                            lastWasSimpleParagraph = false;
                            hasOpendd = false;
                            countdd = 0;
                        }
                        renderer.Write("<dt").WriteAttributes(definitionTerm).Write(">");
                        renderer.WriteLeafInline(definitionTerm);
                        renderer.WriteLine("</dt>");
                    }
                    else
                    {
                        if (!hasOpendd)
                        {
                            renderer.Write("<dd").WriteAttributes(definitionItem).Write(">");
                            countdd = 0;
                            hasOpendd = true;
                        }

                        var nextTerm = i + 1 < definitionItem.Count ? definitionItem[i + 1] : null;
                        bool isSimpleParagraph = (nextTerm == null || nextTerm is DefinitionItem) && countdd == 0 &&
                                                 definitionTermOrContent is ParagraphBlock;

                        var saveImplicitParagraph = renderer.ImplicitParagraph;
                        if (isSimpleParagraph)
                        {
                            renderer.ImplicitParagraph = true;
                            lastWasSimpleParagraph = true;
                        }
                        renderer.Write(definitionTermOrContent);
                        renderer.ImplicitParagraph = saveImplicitParagraph;
                        countdd++;
                    }
                }
                if (hasOpendd)
                {
                    if (!lastWasSimpleParagraph)
                    {
                        renderer.EnsureLine();
                    }
                    renderer.WriteLine("</dd>");
                }
            }
            renderer.EnsureLine();
            renderer.WriteLine("</dl>");
        }
    }
}