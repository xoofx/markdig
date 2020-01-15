// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System;

namespace Markdig.Extensions.MediaLinks
{
    /// <summary>
    /// Provides url for media links.
    /// </summary>
    public interface IHostProvider
    {
        /// <summary>
        /// "class" attribute of generated iframe.
        /// </summary>
        string Class { get; }

        /// <summary>
        /// Generate url for iframe.
        /// </summary>
        /// <param name="mediaUri">Input media uri.</param>
        /// <param name="isSchemaRelative"><see langword="true"/> if <paramref name="mediaUri"/> is a schema relative uri, i.e. uri starts with "//".</param>
        /// <param name="iframeUrl">Generated url for iframe.</param>
        /// <seealso href="https://tools.ietf.org/html/rfc3986#section-4.2"/>
        bool TryHandle(Uri mediaUri, bool isSchemaRelative, out string iframeUrl);

        /// <summary>
        /// Should the generated iframe has allowfullscreen attribute.
        /// </summary>
        /// <remarks>
        /// Should be false for audio embedding.
        /// </remarks>
        bool AllowFullScreen { get; }
    }
}
