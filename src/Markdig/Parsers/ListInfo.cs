// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

namespace Markdig.Parsers
{
    /// <summary>
    /// Defines list information returned when trying to parse a list item with <see cref="ListItemParser.TryParse"/>
    /// </summary>
    public struct ListInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListInfo"/> struct.
        /// </summary>
        /// <param name="bulletType">Type of the bullet (e.g: '1', 'a', 'A', 'i', 'I').</param>
        public ListInfo(char bulletType)
        {
            BulletType = bulletType;
            OrderedStart = null;
            OrderedDelimiter = (char)0;
            DefaultOrderedStart = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListInfo"/> struct.
        /// </summary>
        /// <param name="bulletType">Type of the bullet (e.g: '1', 'a', 'A', 'i', 'I')</param>
        /// <param name="orderedStart">The string used as a starting sequence for an ordered list.</param>
        /// <param name="orderedDelimiter">The ordered delimiter found when parsing this list (e.g: the character `)` after `1)`)</param>
        /// <param name="defaultOrderedStart">The default string used as a starting sequence for the ordered list (e.g: '1' for an numbered ordered list)</param>
        public ListInfo(char bulletType, string orderedStart, char orderedDelimiter, string defaultOrderedStart)
        {
            BulletType = bulletType;
            OrderedStart = orderedStart;
            OrderedDelimiter = orderedDelimiter;
            DefaultOrderedStart = defaultOrderedStart;
        }

        /// <summary>
        /// Gets or sets the type of the bullet (e.g: '1', 'a', 'A', 'i', 'I').
        /// </summary>
        public char BulletType { get; set; }

        /// <summary>
        /// Gets or sets the string used as a starting sequence for an ordered list
        /// </summary>
        public string OrderedStart { get; set; }

        /// <summary>
        /// Gets or sets the ordered delimiter found when parsing this list (e.g: the character `)` after `1)`)
        /// </summary>
        public char OrderedDelimiter { get; set; }

        /// <summary>
        /// Gets or sets default string used as a starting sequence for the ordered list (e.g: '1' for an numbered ordered list)
        /// </summary>
        public string DefaultOrderedStart { get; set; }
    }
}