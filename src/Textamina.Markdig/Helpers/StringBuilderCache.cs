using System;
using System.Collections.Generic;
using System.Text;

namespace Textamina.Markdig.Helpers
{
    public class StringBuilderCache
    {
        /// <summary>
        /// A StringBuilder that can be used locally in a method body only.
        /// </summary>
        [ThreadStatic]
        private static readonly StringBuilder local = new StringBuilder();

        public static StringBuilder Local()
        {
            var sb = local;
            if (sb.Length > 0)
            {
                sb.Clear();
            }
            return sb;
        }

        private readonly Stack<StringBuilder> builders;

        public StringBuilderCache()
        {
            builders = new Stack<StringBuilder>();
        }

        public StringBuilder Get()
        {
            if (builders.Count > 0)
            {
                return builders.Pop();
            }

            return new StringBuilder();
        }

        public void Release(StringBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (builder.Length > 0)
            {
                builder.Clear();
            }
            builders.Push(builder);
        }
    }
}