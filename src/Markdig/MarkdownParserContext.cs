using System.Collections.Generic;

namespace Markdig
{
    /// <summary>
    /// Provides a context that can be used as part of parsing Markdown documents.
    /// </summary>
    public sealed class MarkdownParserContext
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
