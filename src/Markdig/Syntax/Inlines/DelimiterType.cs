// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;

namespace Markdig.Syntax.Inlines
{
    /// <summary>
    /// Gets the type of a <see cref="DelimiterInline"/>.
    /// </summary>
    [Flags]
    public enum DelimiterType
    {
        /// <summary>
        /// An undefined open or close delimiter.
        /// </summary>
        Undefined,

        /// <summary>
        /// An open delimiter.
        /// </summary>
        Open,

        /// <summary>
        /// A close delimiter.
        /// </summary>
        Close,
    }
}