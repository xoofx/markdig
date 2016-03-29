// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;

namespace Textamina.Markdig.Extensions.AutoIdentifiers
{
    /// <summary>
    /// Options for the <see cref="AutoIdentifierExtension"/>.
    /// </summary>
    [Flags]
    public enum AutoIdentifierOptions
    {
        /// <summary>
        /// No options
        /// </summary>
        None,

        /// <summary>
        /// Allows to link to a header by using the same text as the header for the link label.
        /// </summary>
        AutoLink
    }
}