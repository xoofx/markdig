// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Markdig.Extensions.SmartyPants
{
    /// <summary>
    /// A HTML renderer for a <see cref="SmartyPant"/>.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Html.HtmlObjectRenderer{SmartyPant}" />
    public class HtmlSmartyPantRenderer : HtmlObjectRenderer<SmartyPant>
    {
        private static readonly SmartyPantOptions DefaultOptions = new SmartyPantOptions();

        private readonly SmartyPantOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlSmartyPantRenderer"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public HtmlSmartyPantRenderer(SmartyPantOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            this.options = options;
        }

        protected override void Write(HtmlRenderer renderer, SmartyPant obj)
        {
            string text;
            if (!options.Mapping.TryGetValue(obj.Type, out text))
            {
                DefaultOptions.Mapping.TryGetValue(obj.Type, out text);
            }
            renderer.Write(text);
        }
    }
}