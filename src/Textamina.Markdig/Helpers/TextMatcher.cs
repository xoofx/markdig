// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Textamina.Markdig.Helpers
{
    /// <summary>
    /// Match a text against a list of ASCII string using internally a tree to speedup the lookup
    /// </summary>
    public class TextMatchHelper
    {
        private readonly CharNode root;
        private readonly ListCache listCache;
        private readonly DictionaryCache dictCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextMatchHelper"/> class.
        /// </summary>
        /// <param name="matches">The matches to match against.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public TextMatchHelper(HashSet<string> matches)
        {
            if (matches == null) throw new ArgumentNullException(nameof(matches));
            var list = new List<string>(matches);
            root = new CharNode();
            dictCache = new DictionaryCache();
            listCache = new ListCache();
            BuildMap(ref root, 0, list);
            listCache.Clear();
            dictCache.Clear();
        }

        /// <summary>
        /// Tries to match in the text, at offset position, the list of string matches registered to this instance.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="match">The match string if the match was successfull.</param>
        /// <returns>
        ///   <c>true</c> if the match was successfull; <c>false</c> otherwise
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public bool TryMatch(string text, int offset, int length, out string match)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            // TODO(lazy): we should check offset and length for a better exception experience in case of wrong usage
            var node = root;
            match = null;
            while (length > 0)
            {
                var c = text[offset];
                var nextIndex = c - node.MinChar;
                if (nextIndex < 0)
                {
                    return false;
                }
                var nextNodes = node.NextNodes;
                if (nextNodes  == null || nextIndex >= nextNodes.Length)
                {
                    return false;
                }

                node = nextNodes[nextIndex];
                if (node == null)
                {
                    return false;
                }
                if (node.Content != null)
                {
                    match = node.Content;
                    return true;
                }

                offset++;
                length--;
            }
            return false;
        }

        private void BuildMap(ref CharNode node, int index, List<string> list)
        {
            // TODO(lazy): This code for building the nodes is not very efficient in terms of memory usage and could be optimized (using structs and indices)
            // At least, we are using a cache for the temporary objects build (List<string> and Dictionary<char, CharNode>)
            var charSet = dictCache.Get();
            int minChar = int.MaxValue;
            int maxChar = 0;
            for (int i = 0; i < list.Count; i++)
            {
                var str = list[i];
                var c = str[index];
                // Make sure that we don't get something to match that is too large
                if (c > 127)
                {
                    throw new InvalidOperationException($"The string [{str}] contains a non ASCII character `{c}`");
                }

                CharNode nextNode;
                if (!charSet.TryGetValue(c, out nextNode))
                {
                    nextNode = new CharNode();
                    charSet.Add(c, nextNode);
                }

                // We have found a string for this node
                if (index + 1 == str.Length)
                {
                    nextNode.Content = str;
                }
                else
                {
                    if (nextNode.NextList == null)
                    {
                        nextNode.NextList = listCache.Get();
                    }
                    nextNode.NextList.Add(str);
                }

                if (c < minChar)
                {
                    minChar = c;
                }
                if (c > maxChar)
                {
                    maxChar = c;
                }
            }
            node.MinChar = minChar;
            var chars = new CharNode[maxChar - minChar + 1];
            node.NextNodes = chars;
            foreach (var charList in charSet)
            {
                var nodeIndex = charList.Key - minChar;
                chars[nodeIndex] = charList.Value;
                if (charList.Value.NextList != null)
                {
                    BuildMap(ref chars[charList.Key - minChar], index + 1, charList.Value.NextList);
                    listCache.Release(charList.Value.NextList);
                    charList.Value.NextList = null;
                }
            }
            dictCache.Release(charSet);
        }

        private class ListCache : DefaultObjectCache<List<string>>
        {
            protected override void Reset(List<string> instance)
            {
                instance.Clear();
            }
        }

        private class DictionaryCache : DefaultObjectCache<Dictionary<char, CharNode>>
        {
            protected override void Reset(Dictionary<char, CharNode> instance)
            {
                instance.Clear();
            }
        }

        private class CharNode
        {
            public CharNode[] NextNodes;

            public int MinChar;

            public List<string> NextList;

            public string Content { get; set; }
        }
    }
}