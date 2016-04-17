// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Collections.Generic;

namespace Markdig.Helpers
{
    /// <summary>
    /// A simple object recycling system.
    /// </summary>
    /// <typeparam name="T">Type of the object to cache</typeparam>
    public abstract class ObjectCache<T> where T : class
    {
        private readonly Stack<T> builders;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectCache{T}"/> class.
        /// </summary>
        protected ObjectCache()
        {
            builders = new Stack<T>(4);
        }

        /// <summary>
        /// Clears this cache.
        /// </summary>
        public void Clear()
        {
            lock (builders)
            {
                builders.Clear();
            }
        }

        /// <summary>
        /// Gets a new instance.
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            lock (builders)
            {
                if (builders.Count > 0)
                {
                    return builders.Pop();
                }
            }

            return NewInstance();
        }

        /// <summary>
        /// Releases the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <exception cref="System.ArgumentNullException">if instance is null</exception>
        public void Release(T instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            Reset(instance);
            lock (builders)
            {
                builders.Push(instance);
            }
        }

        /// <summary>
        /// Creates a new instance of {T}
        /// </summary>
        /// <returns>A new instance of {T}</returns>
        protected abstract T NewInstance();

        /// <summary>
        /// Resets the specified instance when <see cref="Release"/> is called before storing back to this cache.
        /// </summary>
        /// <param name="instance">The instance.</param>
        protected abstract void Reset(T instance);
    }
}