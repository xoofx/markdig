// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Syntax;

namespace Markdig.Parsers
{
    /// <summary>
    /// Base interface for a <see cref="BlockParser"/>.
    /// </summary>
    /// <typeparam name="TProcessor"></typeparam>
    /// <seealso cref="Markdig.Parsers.IMarkdownParser{T}" />
    public interface IBlockParser<in TProcessor> : IMarkdownParser<TProcessor>
    {
        /// <summary>
        /// Determines whether this instance can interrupt the specified block being processed.
        /// </summary>
        /// <param name="processor">The parser processor.</param>
        /// <param name="block">The block being processed.</param>
        /// <returns><c>true</c> if this parser can interrupt the specified block being processed.</returns>
        bool CanInterrupt(TProcessor processor, Block block);

        /// <summary>
        /// Tries to match a block opening.
        /// </summary>
        /// <param name="processor">The parser processor.</param>
        /// <returns>The result of the match</returns>
        BlockState TryOpen(TProcessor processor);

        /// <summary>
        /// Tries to continue matching a block already opened.
        /// </summary>
        /// <param name="processor">The parser processor.</param>
        /// <param name="block">The block already opened.</param>
        /// <returns>The result of the match. By default, don't expect any newline</returns>
        BlockState TryContinue(TProcessor processor, Block block);

        /// <summary>
        /// Called when a block matched by this parser is being closed (to allow final computation on the block).
        /// </summary>
        /// <param name="processor">The parser processor.</param>
        /// <param name="block">The block being closed.</param>
        /// <returns><c>true</c> to keep the block; <c>false</c> to remove it. True by default.</returns>
        bool Close(TProcessor processor, Block block);
    }
}