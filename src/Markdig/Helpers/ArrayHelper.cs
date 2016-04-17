// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
namespace Markdig.Helpers
{
    /// <summary>
    /// Helper class for defining Empty arrays.
    /// </summary>
    /// <typeparam name="T">Type of an element of the array</typeparam>
    public static class ArrayHelper<T>
    {
        /// <summary>
        /// An empty array.
        /// </summary>
        public static readonly T[] Empty = new T[0];
    }
}