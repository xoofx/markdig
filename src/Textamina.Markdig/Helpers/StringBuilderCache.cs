// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Text;

namespace Textamina.Markdig.Helpers
{
    public class StringBuilderCache : ObjectCache<StringBuilder>
    {
        /// <summary>
        /// A StringBuilder that can be used locally in a method body only.
        /// </summary>
        [ThreadStatic]
        private static StringBuilder local;

        public static StringBuilder Local()
        {
            var sb = local ?? (local = new StringBuilder());
            if (sb.Length > 0)
            {
                sb.Clear();
            }
            return sb;
        }

        protected override void Reset(StringBuilder builder)
        {
            if (builder.Length > 0)
            {
                builder.Clear();
            }
        }
    }
}