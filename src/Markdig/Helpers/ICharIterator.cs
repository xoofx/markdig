// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

namespace Markdig.Helpers
{
    /// <summary>
    /// Provides a common interface for iterating characters 
    /// over a <see cref="StringSlice"/> or <see cref="StringLineGroup"/>.
    /// </summary>
    public interface ICharIterator
    {
        /// <summary>
        /// Gets the current start character position.
        /// </summary>
        int Start { get; }

        /// <summary>
        /// Gets the current character.
        /// </summary>
        char CurrentChar { get; }

        /// <summary>
        /// Gets the end character position.
        /// </summary>
        int End { get; }

        /// <summary>
        /// Goes to the next character, incrementing the <see cref="Start"/> position.
        /// </summary>
        /// <returns>The next character. `\0` is end of the iteration.</returns>
        char NextChar();

        /// <summary>
        /// Peeks at the next character, without incrementing the <see cref="Start"/> position.
        /// </summary>
        /// <returns>The next character. `\0` is end of the iteration.</returns>
        char PeekChar();

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Trims whitespaces at the beginning of this slice starting from <see cref="Start"/> position.
        /// </summary>
        /// <returns><c>true</c> if it has reaches the end of the iterator</returns>
        bool TrimStart();
    }
}