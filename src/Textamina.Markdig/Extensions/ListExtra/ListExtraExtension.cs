// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Extensions.ListExtra
{
    /// <summary>
    /// Extension for adding new type of list items (a., A., i., I.)
    /// </summary>
    /// <seealso cref="Textamina.Markdig.IMarkdownExtension" />
    public class ListExtraExtension : IMarkdownExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListExtraExtension"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public ListExtraExtension(ListExtraOptions options = ListExtraOptions.Default)
        {
            Options = options;
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
        public ListExtraOptions Options { get; }

        public void Setup(MarkdownPipeline pipeline)
        {
            var parser = pipeline.BlockParsers.Find<ListBlockParser>();
            if (parser != null)
            {
                // TODO: We don't check if there is already an existing parser
                var hasRoman = (Options & (ListExtraOptions.RomanLower | ListExtraOptions.RomanUpper)) != 0;

                if ((Options & ListExtraOptions.AlphaLower) != 0)
                {
                    parser.ItemParsers.Add(new AlphaListItemParser(false, hasRoman));
                }
                if ((Options & ListExtraOptions.AlphaUpper) != 0)
                {
                    parser.ItemParsers.Add(new AlphaListItemParser(true, hasRoman));
                }

                if ((Options & ListExtraOptions.RomanLower) != 0)
                {
                    parser.ItemParsers.Add(new RomanListItemParser(false));
                }
                if ((Options & ListExtraOptions.RomanUpper) != 0)
                {
                    parser.ItemParsers.Add(new RomanListItemParser(true));
                }
            }
        }
    }
}