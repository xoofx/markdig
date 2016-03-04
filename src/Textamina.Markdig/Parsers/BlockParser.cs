// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsers
{
    /// <summary>
    /// Base class for a parser of a <see cref="Block"/>
    /// </summary>
    /// <seealso cref="ParserBase{BlockParserState}" />
    public abstract class BlockParser : ParserBase<BlockParserState>, IBlockParser<BlockParserState>
    {
        /// <summary>
        /// Determines whether the specified char is an opening character.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns><c>true</c> if the specified char is an opening character.</returns>
        public bool HasOpeningCharacter(char c)
        {
            if (OpeningCharacters != null)
            {
                for (int i = 0; i < OpeningCharacters.Length; i++)
                {
                    if (OpeningCharacters[i] == c)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether this instance can interrupt the specified block being processed.
        /// </summary>
        /// <param name="state">The parser state.</param>
        /// <param name="block">The block being processed.</param>
        /// <returns><c>true</c> if this parser can interrupt the specified block being processed.</returns>
        public virtual bool CanInterrupt(BlockParserState state, Block block)
        {
            // By default, all blocks can interrupt a ParagraphBlock except:
            // - Setext heading
            // - Indented code block
            // - HTML blocks
            return true;
        }

        /// <summary>
        /// Tries to match a block opening.
        /// </summary>
        /// <param name="state">The parser state.</param>
        /// <returns>The result of the match</returns>
        public abstract BlockState TryOpen(BlockParserState state);

        /// <summary>
        /// Tries to continue matching a block already opened.
        /// </summary>
        /// <param name="state">The parser state.</param>
        /// <param name="block">The block already opened.</param>
        /// <returns>The result of the match. By default, don't expect any newline</returns>
        public virtual BlockState TryContinue(BlockParserState state, Block block)
        {
            // By default we don't expect any newline
            return BlockState.None;
        }

        /// <summary>
        /// Called when a block matched by this parser is being closed (to allow final computation on the block).
        /// </summary>
        /// <param name="state">The parser state.</param>
        /// <param name="block">The block being closed.</param>
        /// <returns><c>true</c> to keep the block; <c>false</c> to remove it. True by default.</returns>
        public virtual bool Close(BlockParserState state, Block block)
        {
            // By default keep the block
            return true;
        }
    }
}