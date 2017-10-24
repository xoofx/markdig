// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;

namespace Markdig.Extensions.AutoIdentifiers
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
        None = 0,

        /// <summary>
        /// Default (<see cref="AutoLink"/>)
        /// </summary>
        Default = AutoLink | AllowOnlyAscii,

        /// <summary>
        /// Renders auto identifiers like GitHub.
        /// </summary>
        GitHub = Default | KeepOpeningDigits | DiscardDots,

        /// <summary>
        /// Allows to link to a header by using the same text as the header for the link label. Default is <c>true</c>
        /// </summary>
        AutoLink = 1,

        /// <summary>
        /// Allows only ASCII characters in the url (HTML 5 allows to have UTF8 characters). Default is <c>true</c>
        /// </summary>
        AllowOnlyAscii = 2,

        /// <summary>
        /// Allows to keep digits starting a heading (by default, it keeps only characters starting from the first letter)
        /// </summary>
        KeepOpeningDigits = 4,

        /// <summary>
        /// Discard dots when computing an identifier.
        /// </summary>
        DiscardDots = 8
    }
}