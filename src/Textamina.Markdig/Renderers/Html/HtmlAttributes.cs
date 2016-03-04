// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Collections.Generic;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Renderers.Html
{
    /// <summary>
    /// Attached HTML attributes to a <see cref="MarkdownObject"/>.
    /// </summary>
    public class HtmlAttributes
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlAttributes"/> class.
        /// </summary>
        public HtmlAttributes()
        {
        }

        /// <summary>
        /// Gets or sets the HTML id/identifier. May be null.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the CSS classes attached. May be null.
        /// </summary>
        public List<string> Classes { get; set; }

        /// <summary>
        /// Gets or sets the additional properties. May be null.
        /// </summary>
        public List<KeyValuePair<string, string>> Properties { get; set; }

        /// <summary>
        /// Adds a CSS class.
        /// </summary>
        /// <param name="name">The css class name.</param>
        public void AddClass(string name)
        {
            if (Classes == null)
            {
                Classes = new List<string>();
            }
            Classes.Add(name);
        }

        /// <summary>
        /// Adds a property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddProperty(string name, string value)
        {
            if (Properties == null)
            {
                Properties = new List<KeyValuePair<string, string>>();
            }
            Properties.Add(new KeyValuePair<string, string>(name, value));
        }

        /// <summary>
        /// Copies the values from this instance to the specified <see cref="HtmlAttributes"/> instance.
        /// </summary>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void CopyTo(HtmlAttributes htmlAttributes)
        {
            if (htmlAttributes == null) throw new ArgumentNullException(nameof(htmlAttributes));
            // Add html htmlAttributes to the object
            htmlAttributes.Id = Id;
            if (htmlAttributes.Classes == null)
            {
                htmlAttributes.Classes = Classes;
            }
            else if (Classes != null)
            {
                htmlAttributes.Classes.AddRange(Classes);
            }

            if (htmlAttributes.Properties == null)
            {
                htmlAttributes.Properties = Properties;
            }
            else if (Properties != null)
            {
                htmlAttributes.Properties.AddRange(Properties);
            }
        }
    }

    /// <summary>
    /// Extensions for a <see cref="MarkdownObject"/> to allow accessing <see cref="HtmlAttributes"/>
    /// </summary>
    public static class HtmlAttributesExtensions
    {
        private static readonly object Key = typeof (HtmlAttributes);

        /// <summary>
        /// Tries the get <see cref="HtmlAttributes"/> stored on a <see cref="MarkdownObject"/>.
        /// </summary>
        /// <param name="obj">The markdown object.</param>
        /// <returns>The attached html attributes or null if not found</returns>
        public static HtmlAttributes TryGetAttributes(this MarkdownObject obj)
        {
            return obj.GetData(Key) as HtmlAttributes;
        }

        /// <summary>
        /// Gets or creates the <see cref="HtmlAttributes"/> stored on a <see cref="MarkdownObject"/>
        /// </summary>
        /// <param name="obj">The markdown object.</param>
        /// <returns>The attached html attributes</returns>
        public static HtmlAttributes GetAttributes(this MarkdownObject obj)
        {
            var attributes = obj.GetData(Key) as HtmlAttributes;
            if (attributes == null)
            {
                attributes = new HtmlAttributes();
                obj.SetData(Key, attributes);
            }
            return attributes;
        }
    }
}