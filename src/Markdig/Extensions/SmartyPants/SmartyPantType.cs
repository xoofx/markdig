// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
namespace Markdig.Extensions.SmartyPants
{
    /// <summary>
    /// Types of a <see cref="SmartyPant"/>.
    /// </summary>
    public enum SmartyPantType
    {
        /// <summary>
        /// This is a single quote '
        /// </summary>
        Quote = 1,

        /// <summary>
        /// This is a left single quote ' -gt; &lsquo;
        /// </summary>
        LeftQuote,

        /// <summary>
        /// This is a right single quote ' -gt; &rsquo;
        /// </summary>
        RightQuote,

        /// <summary>
        /// This is a double quote "
        /// </summary>
        DoubleQuote,

        /// <summary>
        /// This is a left double quote " -gt; &ldquo;
        /// </summary>
        LeftDoubleQuote,

        /// <summary>
        /// This is a right double quote " -gt; &rdquo;
        /// </summary>
        RightDoubleQuote,

        /// <summary>
        /// This is a right double quote &lt;&lt; -gt; &laquo;
        /// </summary>
        LeftAngleQuote,

        /// <summary>
        /// This is a right angle quote >> -gt;  &raquo;
        /// </summary>
        RightAngleQuote,

        /// <summary>
        /// This is an ellipsis ... -gt; &hellip;
        /// </summary>
        Ellipsis,

        /// <summary>
        /// This is a ndash -- -gt; &ndash;
        /// </summary>
        Dash2,

        /// <summary>
        /// This is a mdash --- -gt; &mdash;
        /// </summary>
        Dash3,
    }
}