using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Extensions.DefinitionLists
{
    /// <summary>
    /// A definition item contains zero to multiple <see cref="DefinitionTerm"/> 
    /// and definitions (any <see cref="Block"/>)
    /// </summary>
    /// <seealso cref="Markdig.Syntax.ContainerBlock" />
    public class DefinitionItem : ContainerBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefinitionItem"/> class.
        /// </summary>
        /// <param name="parser">The parser used to create this block.</param>
        public DefinitionItem(BlockParser parser) : base(parser)
        {
        }

        /// <summary>
        /// Gets or sets the opening character for this definition item (either `:` or `~`)
        /// </summary>
        public char OpeningCharacter { get; set; }
    }
}