// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;

namespace Markdig.Syntax
{
    /// <summary>
    /// Base implementation for a the Markdown syntax tree.
    /// </summary>
    public abstract class MarkdownObject : IMarkdownObject
    {
        protected MarkdownObject()
        {
            Span = SourceSpan.Empty;
        }

        /// <summary>
        /// The attached datas. Use internally a simple array instead of a Dictionary{Object,Object}
        /// as we expect less than 5~10 entries, usually typically 1 (HtmlAttributes)
        /// so it will gives faster access than a Dictionary, and lower memory occupation
        /// </summary>
        private DataEntry[] attachedDatas;
        private int count;

        /// <summary>
        /// Gets or sets the text column this instance was declared (zero-based).
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Gets or sets the text line this instance was declared (zero-based).
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// The source span
        /// </summary>
        public SourceSpan Span;

        /// <summary>
        /// Gets a string of the location in the text.
        /// </summary>
        /// <returns></returns>
        public string ToPositionText()
        {
            return $"${Line}, {Column}, {Span.Start}-{Span.End}";
        }

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
                attachedDatas = new DataEntry[1];
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    if (attachedDatas[i].Key == key)
                    {
                        attachedDatas[i].Value = value;
                        return;
                    }
                }
                if (count == attachedDatas.Length)
                {
                    var temp = new DataEntry[attachedDatas.Length + 1];
                    Array.Copy(attachedDatas, 0, temp, 0, count);
                    attachedDatas = temp;
                }
            }
            attachedDatas[count] = new DataEntry(key, value);
            count++;
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

            for (int i = 0; i < count; i++)
            {
                if (attachedDatas[i].Key == key)
                {
                    return true;
                }
            }
            return false;
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
            for (int i = 0; i < count; i++)
            {
                if (attachedDatas[i].Key == key)
                {
                    return attachedDatas[i].Value;
                }
            }
            return null;
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

            for (int i = 0; i < count; i++)
            {
                if (attachedDatas[i].Key == key)
                {
                    if (i < count - 1)
                    {
                        Array.Copy(attachedDatas, i + 1, attachedDatas, i, count - i - 1);
                    }
                    count--;
                    attachedDatas[count] = new DataEntry();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Store a Key/Value pair.
        /// </summary>
        private struct DataEntry
        {
            public DataEntry(object key, object value)
            {
                Key = key;
                Value = value;
            }

            public readonly object Key;

            public object Value;
        }
    }
}