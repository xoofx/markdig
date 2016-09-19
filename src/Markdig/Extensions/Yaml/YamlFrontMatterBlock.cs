// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Extensions.Yaml
{
    /// <summary>
    /// A YAML frontmatter block.
    /// </summary>
    /// <seealso cref="Markdig.Syntax.CodeBlock" />
    public class YamlFrontMatterBlock : CodeBlock, IFencedBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YamlFrontMatterBlock"/> class.
        /// </summary>
        /// <param name="parser">The parser.</param>
        public YamlFrontMatterBlock(BlockParser parser) : base(parser)
        {
        }

        public string Info { get; set; }

        public string Arguments { get; set; }

        public int FencedCharCount { get; set; }

        public char FencedChar { get; set; }
    }
}