using System;
using System.Collections.Generic;

namespace Textamina.Markdig.Helpers
{
    public class ObjectCache<T> where T : class, new()
    {
        private readonly Stack<T> builders;

        public ObjectCache()
        {
            builders = new Stack<T>();
        }

        public T Get()
        {
            lock (builders)
            {
                if (builders.Count > 0)
                {
                    return builders.Pop();
                }
            }

            return new T();
        }

        public void Release(T builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            Reset(builder);
            lock (builders)
            {
                builders.Push(builder);
            }
        }

        protected virtual void Reset(T builder)
        {
        }
    }
}