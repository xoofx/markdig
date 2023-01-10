// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

namespace Markdig.Helpers
{
    /// <summary>
    /// Helper class for defining Empty arrays.
    /// </summary>
    public static class ArrayHelper
    {
        /// <summary>
        /// An empty array.
        /// </summary>
        public static T[] Empty<T>() => System.Array.Empty<T>();
    }
}