// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Runtime.CompilerServices;
using Textamina.Markdig.Helpers;

namespace Textamina.Markdig.Parsers
{
    public enum BlockState
    {
        None,

        Skip,

        Continue,

        ContinueDiscard,

        Break,

        BreakDiscard
    }

    public static class BlockStateExtensions
    {
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsDiscard(this BlockState blockState)
        {
            return blockState == BlockState.ContinueDiscard || blockState == BlockState.BreakDiscard;
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsContinue(this BlockState blockState)
        {
            return blockState == BlockState.Continue || blockState == BlockState.ContinueDiscard;
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsBreak(this BlockState blockState)
        {
            return blockState == BlockState.Break || blockState == BlockState.BreakDiscard;
        }
    }
}