// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Collections.Generic;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// Base implementation for a the Markdown syntax tree.
    /// </summary>
    public abstract class MarkdownObject : IMarkdownObject
    {
        /// <summary>
        /// The attached datas.
        /// </summary>
        private Dictionary<object, object> attachedDatas;

        /// <summary>
        /// Stores a key/value pair for this instance.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.ArgumentNullException">if key is null</exception>
        public void SetData(object key, object value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (attachedDatas == null)
            {
                attachedDatas = new Dictionary<object, object>();
            }
            attachedDatas[key] = value;
        }

        /// <summary>
        /// Determines whether this instance contains the specified key data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if a data with the key is stored</returns>
        /// <exception cref="System.ArgumentNullException">if key is null</exception>
        public bool ContainsData(object key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (attachedDatas == null)
            {
                return false;
            }
            return attachedDatas.ContainsKey(key);
        }

        /// <summary>
        /// Gets the associated data for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The associated data or null if none</returns>
        /// <exception cref="System.ArgumentNullException">if key is null</exception>
        public object GetData(object key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (attachedDatas == null)
            {
                return null;
            }
            object value;
            attachedDatas.TryGetValue(key, out value);
            return value;
        }

        /// <summary>
        /// Removes the associated data for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the data was removed; <c>false</c> otherwise</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public bool RemoveData(object key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (attachedDatas == null)
            {
                return true;
            }
            return attachedDatas.Remove(key);
        }
    }
}