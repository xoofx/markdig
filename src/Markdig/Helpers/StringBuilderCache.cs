// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Text;

namespace Markdig.Helpers
{
    /// <summary>
    /// An implementation of <see cref="ObjectCache{T}"/> for <see cref="StringBuilder"/>
    /// </summary>
    /// <seealso cref="Markdig.Helpers.ObjectCache{StringBuilder}" />
    public class StringBuilderCache : DefaultObjectCache<StringBuilder>
    {
        /// <summary>
        /// A StringBuilder that can be used locally in a method body only.
        /// </summary>
        [ThreadStatic]
        private static StringBuilder local;

        /// <summary>
        /// Provides a string builder that can only be used locally in a method. This StringBuilder MUST not be stored.
        /// </summary>
        /// <returns></returns>
        public static StringBuilder Local()
        {
            var sb = local ?? (local = new StringBuilder());
            if (sb.Length > 0)
            {
                sb.Length = 0;
            }
            return sb;
        }

        protected override void Reset(StringBuilder instance)
        {
            if (instance.Length > 0)
            {
                instance.Length = 0;
            }
        }
    }
}