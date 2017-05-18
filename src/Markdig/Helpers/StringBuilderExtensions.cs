// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Text;

namespace Markdig.Helpers
{
    /// <summary>
    /// Extensions for StringBuilder with <see cref="StringSlice"/>
    /// </summary>
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Appends the specified slice to this <see cref="StringBuilder"/> instance.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="slice">The slice.</param>
        public static StringBuilder Append(this StringBuilder builder, StringSlice slice)
        {
            return builder.Append(slice.Text, slice.Start, slice.Length);
        }
    }
}