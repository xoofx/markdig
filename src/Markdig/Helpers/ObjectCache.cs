// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Collections.Concurrent;

namespace Markdig.Helpers;

/// <summary>
/// A simple object recycling system.
/// </summary>
/// <typeparam name="T">Type of the object to cache</typeparam>
public abstract class ObjectCache<T> where T : class
{
    private readonly ConcurrentQueue<T> _builders;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectCache{T}"/> class.
    /// </summary>
    protected ObjectCache()
    {
        _builders = new ConcurrentQueue<T>();
    }

    /// <summary>
    /// Clears this cache.
    /// </summary>
    public void Clear()
    {
        _builders.Clear();
    }

    /// <summary>
    /// Gets a new instance.
    /// </summary>
    /// <returns></returns>
    public T Get()
    {
        if (_builders.TryDequeue(out T? instance))
        {
            return instance;
        }

        return NewInstance();
    }

    /// <summary>
    /// Releases the specified instance.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <exception cref="ArgumentNullException">if instance is null</exception>
    public void Release(T instance)
    {
        if (instance is null) ThrowHelper.ArgumentNullException(nameof(instance));
        Reset(instance);
        _builders.Enqueue(instance);
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