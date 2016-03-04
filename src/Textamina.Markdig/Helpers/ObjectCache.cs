// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Collections.Generic;

namespace Textamina.Markdig.Helpers
{
    /// <summary>
    /// A simple object recycling system.
    /// </summary>
    /// <typeparam name="T">Type of the object to cache</typeparam>
    public class ObjectCache<T> where T : class, new()
    {
        private readonly Stack<T> builders;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectCache{T}"/> class.
        /// </summary>
        public ObjectCache()
        {
            builders = new Stack<T>();
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

            return new T();
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
        /// Resets the specified instance when <see cref="Release"/> is called before storing back to this cache.
        /// </summary>
        /// <param name="instance">The instance.</param>
        protected virtual void Reset(T instance)
        {
        }
    }
}