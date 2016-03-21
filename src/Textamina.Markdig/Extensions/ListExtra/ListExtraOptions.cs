// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;

namespace Textamina.Markdig.Extensions.ListExtra
{
    /// <summary>
    /// Options for enabling support for list extra.
    /// </summary>
    [Flags]
    public enum ListExtraOptions
    {
        /// <summary>
        /// Allows all extra list (default).
        /// </summary>
        Default = AlphaLower | AlphaUpper | RomanLower | RomanUpper,

        /// <summary>
        /// A list starting by an alphabetical character lowercase `a` to `z`
        /// </summary>
        AlphaLower = 1,

        /// <summary>
        /// A list starting by a alphabetical character uppercase `A` to `Z`
        /// </summary>
        AlphaUpper = 2,

        /// <summary>
        /// A list starting by a roman number `i` `ii`, `iv`
        /// </summary>
        RomanLower = 4,

        /// <summary>
        /// A list starting by a roman number `I`, `II`, `IV`
        /// </summary>
        RomanUpper = 8,
    }
}