// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Runtime.CompilerServices;
using Markdig.Helpers;

namespace Markdig.Parsers
{
    /// <summary>
    /// Extensions used by <see cref="BlockState"/>.
    /// </summary>
    public static class BlockStateExtensions
    {
        /// <summary>
        /// Determines whether this <see cref="BlockState"/> is discarded.
        /// </summary>
        /// <param name="blockState">State of the block.</param>
        /// <returns><c>true</c> if the block state is in discard state</returns>
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsDiscard(this BlockState blockState)
        {
            return blockState == BlockState.ContinueDiscard || blockState == BlockState.BreakDiscard;
        }

        /// <summary>
        /// Determines whether this <see cref="BlockState"/> is in a continue state.
        /// </summary>
        /// <param name="blockState">State of the block.</param>
        /// <returns><c>true</c> if the block state is in continue state</returns>
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsContinue(this BlockState blockState)
        {
            return blockState == BlockState.Continue || blockState == BlockState.ContinueDiscard;
        }

        /// <summary>
        /// Determines whether this <see cref="BlockState"/> is in a break state.
        /// </summary>
        /// <param name="blockState">State of the block.</param>
        /// <returns><c>true</c> if the block state is in break state</returns>
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsBreak(this BlockState blockState)
        {
            return blockState == BlockState.Break || blockState == BlockState.BreakDiscard;
        }
    }
}