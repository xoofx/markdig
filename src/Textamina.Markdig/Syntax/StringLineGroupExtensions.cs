// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// Extensions methods for <see cref="StringLineGroup"/>
    /// </summary>
    public static class StringLineGroupExtensions
    {
        /// <summary>
        /// Trims each lines of the specified <see cref="StringLineGroup"/>.
        /// </summary>
        /// <param name="slices">The slices.</param>
        public static void Trim(this StringLineGroup slices)
        {
            for (int i = 0; i < slices.Count; i++)
            {
                slices.Lines[i].Slice.Trim();
            }
        }
    }
}