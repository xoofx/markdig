// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Renderers.Html.Inlines
{
    public class EmphasisInlineRenderer : HtmlObjectRenderer<EmphasisInline>
    {
        public delegate string GetTagDelegate(EmphasisInline obj);

        public EmphasisInlineRenderer()
        {
            GetTag = GetDefaultTag;
        }

        public GetTagDelegate GetTag { get; set; }

        protected override void Write(HtmlRenderer renderer, EmphasisInline obj)
        {
            string tag = null;
            if (renderer.EnableHtmlForInline)
            {
                tag = GetTag(obj);
                renderer.Write("<").Write(tag).Write(">");
            }
            renderer.WriteChildren(obj);
            if (renderer.EnableHtmlForInline)
            {
                renderer.Write("</").Write(tag).Write(">");
            }
        }

        public string GetDefaultTag(EmphasisInline obj)
        {
            if (obj.DelimiterChar == '*' || obj.DelimiterChar == '_')
            {
                return obj.Strong ? "strong" : "em";
            }
            return null;
        }
    }
}