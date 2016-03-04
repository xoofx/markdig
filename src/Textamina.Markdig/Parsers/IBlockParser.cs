// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsers
{
    /// <summary>
    /// Base interface for a <see cref="BlockParser"/>.
    /// </summary>
    /// <typeparam name="TParserState"></typeparam>
    /// <seealso cref="Textamina.Markdig.Parsers.IMarkdownParser{T}" />
    public interface IBlockParser<in TParserState> : IMarkdownParser<TParserState>
    {
        /// <summary>
        /// Determines whether this instance can interrupt the specified block being processed.
        /// </summary>
        /// <param name="state">The parser state.</param>
        /// <param name="block">The block being processed.</param>
        /// <returns><c>true</c> if this parser can interrupt the specified block being processed.</returns>
        bool CanInterrupt(BlockParserState state, Block block);

        /// <summary>
        /// Tries to match a block opening.
        /// </summary>
        /// <param name="state">The parser state.</param>
        /// <returns>The result of the match</returns>
        BlockState TryOpen(BlockParserState state);

        /// <summary>
        /// Tries to continue matching a block already opened.
        /// </summary>
        /// <param name="state">The parser state.</param>
        /// <param name="block">The block already opened.</param>
        /// <returns>The result of the match. By default, don't expect any newline</returns>
        BlockState TryContinue(BlockParserState state, Block block);

        /// <summary>
        /// Called when a block matched by this parser is being closed (to allow final computation on the block).
        /// </summary>
        /// <param name="state">The parser state.</param>
        /// <param name="block">The block being closed.</param>
        /// <returns><c>true</c> to keep the block; <c>false</c> to remove it. True by default.</returns>
        bool Close(BlockParserState state, Block block);
    }
}