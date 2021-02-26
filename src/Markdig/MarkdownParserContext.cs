// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Collections.Generic;

namespace Markdig
{
    /// <summary>
    /// Provides a context that can be used as part of parsing Markdown documents.
    /// </summary>
    public class MarkdownParserContext
    {
        /// <summary>
        /// Gets or sets the context property collection.
        /// </summary>
        public Dictionary<object, object> Properties { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownParserContext" /> class.
        /// </summary>
        public MarkdownParserContext()
        {
            Properties = new Dictionary<object, object>();
        }
    }
}
