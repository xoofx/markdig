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
#if NET452
        public static T[] Empty<T>() => EmptyArray<T>.Value;

        private static class EmptyArray<T>
        {
            public static readonly T[] Value = new T[0];
        }
#else
        public static T[] Empty<T>() => System.Array.Empty<T>();
#endif
    }
}