// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.Html.Inlines
{
    /// <summary>
    /// A HTML renderer for an <see cref="EmphasisInline"/>.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Html.HtmlObjectRenderer{Markdig.Syntax.Inlines.EmphasisInline}" />
    public class EmphasisInlineRenderer : HtmlObjectRenderer<EmphasisInline>
    {
        /// <summary>
        /// Delegates to get the tag associated to an <see cref="EmphasisInline"/> object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The HTML tag associated to this <see cref="EmphasisInline"/> object</returns>
        public delegate string GetTagDelegate(EmphasisInline obj);

        /// <summary>
        /// Initializes a new instance of the <see cref="EmphasisInlineRenderer"/> class.
        /// </summary>
        public EmphasisInlineRenderer()
        {
            GetTag = GetDefaultTag;
        }

        /// <summary>
        /// Gets or sets the GetTag delegate.
        /// </summary>
        public GetTagDelegate GetTag { get; set; }

        protected override void Write(HtmlRenderer renderer, EmphasisInline obj)
        {
            string tag = null;
            if (renderer.EnableHtmlForInline)
            {
                tag = GetTag(obj);
                renderer.Write("<").Write(tag).WriteAttributes(obj).Write(">");
            }
            renderer.WriteChildren(obj);
            if (renderer.EnableHtmlForInline)
            {
                renderer.Write("</").Write(tag).Write(">");
            }
        }

        /// <summary>
        /// Gets the default HTML tag for ** and __ emphasis.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public string GetDefaultTag(EmphasisInline obj)
        {
            if (obj.DelimiterChar == '*' || obj.DelimiterChar == '_')
            {
                return obj.IsDouble ? "strong" : "em";
            }
            return null;
        }
    }
}