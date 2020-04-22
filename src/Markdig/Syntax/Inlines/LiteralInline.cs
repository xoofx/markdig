// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Diagnostics;
using Markdig.Helpers;

namespace Markdig.Syntax.Inlines
{
    /// <summary>
    /// A literal inline.
    /// </summary>
    /// <seealso cref="LeafInline" />
    [DebuggerDisplay("{Content}")]
    public class LiteralInline : LeafInline
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LiteralInline"/> class.
        /// </summary>
        public LiteralInline()
        {
            Content = new StringSlice(null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LiteralInline"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        public LiteralInline(StringSlice content)
        {
            Content = content;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LiteralInline"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public LiteralInline(string text)
        {
            if (text == null) ThrowHelper.ArgumentNullException_text();
            Content = new StringSlice(text);
        }

        /// <summary>
        /// The content as a <see cref="StringSlice"/>.
        /// </summary>
        public StringSlice Content;

        /// <summary>
        /// A boolean indicating whether the first character of this literal is escaped by `\`.
        /// </summary>
        public bool IsFirstCharacterEscaped { get; set; }

        public override string ToString()
        {
            return Content.ToString();
        }
    }
}