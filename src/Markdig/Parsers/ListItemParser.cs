// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

namespace Markdig.Parsers
{
    /// <summary>
    /// A parser base class for a list item.
    /// </summary>
    public abstract class ListItemParser
    {
        /// <summary>
        /// Defines the characters that are used for detecting this list item.
        /// </summary>
        public char[] OpeningCharacters { get; protected set; }

        /// <summary>
        /// Tries to parse the current input as a list item for this particular instance.
        /// </summary>
        /// <param name="state">The block processor</param>
        /// <param name="pendingBulletType">The type of the current bullet type</param>
        /// <param name="result">The result of parsing</param>
        /// <returns><c>true</c> if parsing was sucessfull; <c>false</c> otherwise</returns>
        public abstract bool TryParse(BlockProcessor state, char pendingBulletType, out ListInfo result);
    }
}