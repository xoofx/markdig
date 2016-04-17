// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Syntax;

namespace Markdig.Parsers
{
    /// <summary>
    /// A delegates that allows to porcess attached attributes at <see cref="BlockParser"/> time.
    /// </summary>
    /// <param name="processor">The processor.</param>
    /// <param name="slice">The slice to look for attached attributes.</param>
    /// <param name="block">The block.</param>
    /// <returns><c>true</c> if attributes were found; otherwise <c>false</c></returns>
    public delegate bool TryParseAttributesDelegate(
        BlockProcessor processor, ref StringSlice slice, IBlock block);

    /// <summary>
    /// An interface used to tag <see cref="BlockParser"/> that supports parsing <see cref="Markdig.Renderers.Html.HtmlAttributes"/>
    /// </summary>
    public interface IAttributesParseable
    {
        /// <summary>
        /// A delegates that allows to process attached attributes
        /// </summary>
        TryParseAttributesDelegate TryParseAttributes { get; set; }
    }
}