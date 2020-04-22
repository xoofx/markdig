// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

namespace Markdig.Helpers
{
    /// <summary>
    /// A default object cache that expect the type {T} to provide a parameter less constructor
    /// </summary>
    /// <typeparam name="T">The type of item to cache</typeparam>
    /// <seealso cref="ObjectCache{T}" />
    public abstract class DefaultObjectCache<T> : ObjectCache<T> where T : class, new()
    {
        protected override T NewInstance()
        {
            return new T();
        }
    }
}