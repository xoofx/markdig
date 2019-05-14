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
        /// A tag associated with the provider.
        /// </summary>
        string Tag { get; }

        /// <summary>
        /// Generate url for iframe.
        /// </summary>
        /// <param name="mediaUri">Input media uri.</param>
        /// <param name="iframeUrl">Generated url for iframe.</param>
        bool TryHandle(Uri mediaUri, out string iframeUrl);

        /// <summary>
        /// Should the generated iframe has allowfullscreen attribute.
        /// </summary>
        /// <remarks>
        /// Should be false for audio embedding.
        /// </remarks>
        bool AllowFullScreen { get; }
    }
}
