// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
namespace Markdig.Syntax
{
    /// <summary>
    /// Base interface for a the Markdown syntax tree
    /// </summary>
    public interface IMarkdownObject
    {
        /// <summary>
        /// Stores a key/value pair for this instance.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.ArgumentNullException">if key is null</exception>
        void SetData(object key, object value);

        /// <summary>
        /// Determines whether this instance contains the specified key data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if a data with the key is stored</returns>
        /// <exception cref="System.ArgumentNullException">if key is null</exception>
        bool ContainsData(object key);

        /// <summary>
        /// Gets the associated data for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The associated data or null if none</returns>
        /// <exception cref="System.ArgumentNullException">if key is null</exception>
        object GetData(object key);

        /// <summary>
        /// Removes the associated data for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the data was removed; <c>false</c> otherwise</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        bool RemoveData(object key);
    }
}