// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;

namespace Markdig.Syntax;

/// <summary>
/// A typed key used to store data on <see cref="IMarkdownObject"/> instances.
/// </summary>
/// <typeparam name="T">The value type associated with this key.</typeparam>
public sealed class DataKey<T>
{
    /// <summary>
    /// Gets the opaque key object used for storage.
    /// </summary>
    public object Key { get; } = new object();
}

/// <summary>
/// Typed helper methods for storing and retrieving data on <see cref="IMarkdownObject"/>.
/// </summary>
public static class MarkdownObjectDataExtensions
{
    /// <summary>
    /// Gets the data associated with the specified key.
    /// </summary>
    /// <typeparam name="T">Expected data type.</typeparam>
    /// <param name="markdownObject">The markdown object.</param>
    /// <param name="key">The key to read.</param>
    /// <returns>The stored value if it matches <typeparamref name="T"/>; otherwise <c>default</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="markdownObject"/> or <paramref name="key"/> is null.</exception>
    public static T? GetData<T>(this IMarkdownObject markdownObject, object key)
    {
        if (markdownObject is null) ThrowHelper.ArgumentNullException(nameof(markdownObject));
        if (key is null) ThrowHelper.ArgumentNullException(nameof(key));

        return markdownObject.GetData(key) is T data ? data : default;
    }

    /// <summary>
    /// Gets the data associated with the specified typed key.
    /// </summary>
    /// <typeparam name="T">Expected data type.</typeparam>
    /// <param name="markdownObject">The markdown object.</param>
    /// <param name="key">The typed key to read.</param>
    /// <returns>The stored value if it matches <typeparamref name="T"/>; otherwise <c>default</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="markdownObject"/> or <paramref name="key"/> is null.</exception>
    public static T? GetData<T>(this IMarkdownObject markdownObject, DataKey<T> key)
    {
        if (key is null) ThrowHelper.ArgumentNullException(nameof(key));
        return markdownObject.GetData<T>(key.Key);
    }

    /// <summary>
    /// Gets the data associated with the conventional type key (<c>typeof(T)</c>).
    /// </summary>
    /// <typeparam name="T">Expected data type.</typeparam>
    /// <param name="markdownObject">The markdown object.</param>
    /// <returns>The stored value if it matches <typeparamref name="T"/>; otherwise <c>default</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="markdownObject"/> is null.</exception>
    public static T? GetData<T>(this IMarkdownObject markdownObject)
    {
        if (markdownObject is null) ThrowHelper.ArgumentNullException(nameof(markdownObject));
        return markdownObject.GetData<T>(typeof(T));
    }

    /// <summary>
    /// Tries to get the data associated with the specified key.
    /// </summary>
    /// <typeparam name="T">Expected data type.</typeparam>
    /// <param name="markdownObject">The markdown object.</param>
    /// <param name="key">The key to read.</param>
    /// <param name="value">The output value if found and type-compatible.</param>
    /// <returns><c>true</c> if the value exists and matches <typeparamref name="T"/>; otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="markdownObject"/> or <paramref name="key"/> is null.</exception>
    public static bool TryGetData<T>(this IMarkdownObject markdownObject, object key, out T? value)
    {
        if (markdownObject is null) ThrowHelper.ArgumentNullException(nameof(markdownObject));
        if (key is null) ThrowHelper.ArgumentNullException(nameof(key));

        if (markdownObject.GetData(key) is T typedValue)
        {
            value = typedValue;
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Tries to get the data associated with the specified typed key.
    /// </summary>
    /// <typeparam name="T">Expected data type.</typeparam>
    /// <param name="markdownObject">The markdown object.</param>
    /// <param name="key">The typed key to read.</param>
    /// <param name="value">The output value if found and type-compatible.</param>
    /// <returns><c>true</c> if the value exists and matches <typeparamref name="T"/>; otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="markdownObject"/> or <paramref name="key"/> is null.</exception>
    public static bool TryGetData<T>(this IMarkdownObject markdownObject, DataKey<T> key, out T? value)
    {
        if (key is null) ThrowHelper.ArgumentNullException(nameof(key));
        return markdownObject.TryGetData(key.Key, out value);
    }

    /// <summary>
    /// Sets data on the conventional type key (<c>typeof(T)</c>).
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    /// <param name="markdownObject">The markdown object.</param>
    /// <param name="value">The value to store.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="markdownObject"/> is null.</exception>
    public static void SetData<T>(this IMarkdownObject markdownObject, T value)
    {
        if (markdownObject is null) ThrowHelper.ArgumentNullException(nameof(markdownObject));
        markdownObject.SetData(typeof(T), value!);
    }

    /// <summary>
    /// Sets data on the specified typed key.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    /// <param name="markdownObject">The markdown object.</param>
    /// <param name="key">The typed key to write.</param>
    /// <param name="value">The value to store.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="markdownObject"/> or <paramref name="key"/> is null.</exception>
    public static void SetData<T>(this IMarkdownObject markdownObject, DataKey<T> key, T value)
    {
        if (markdownObject is null) ThrowHelper.ArgumentNullException(nameof(markdownObject));
        if (key is null) ThrowHelper.ArgumentNullException(nameof(key));

        markdownObject.SetData(key.Key, value!);
    }
}
