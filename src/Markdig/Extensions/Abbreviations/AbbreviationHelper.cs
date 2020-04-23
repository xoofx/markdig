// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Collections.Generic;
using Markdig.Helpers;
using Markdig.Syntax;

namespace Markdig.Extensions.Abbreviations
{
    /// <summary>
    /// Extension methods for <see cref="Abbreviation"/>.
    /// </summary>
    public static class AbbreviationHelper
    {
        private static readonly object DocumentKey = typeof (Abbreviation);

        public static bool HasAbbreviations(this MarkdownDocument document)
        {
            return document.GetAbbreviations() != null;
        }

        public static void AddAbbreviation(this MarkdownDocument document, string label, Abbreviation abbr)
        {
            if (document == null) ThrowHelper.ArgumentNullException(nameof(document));
            if (label == null) ThrowHelper.ArgumentNullException_label();
            if (abbr == null) ThrowHelper.ArgumentNullException(nameof(abbr));

            var map = document.GetAbbreviations();
            if (map == null)
            {
                map = new Dictionary<string, Abbreviation>();
                document.SetData(DocumentKey, map);
            }
            map[label] = abbr;
        }

        public static Dictionary<string, Abbreviation> GetAbbreviations(this MarkdownDocument document)
        {
            return document.GetData(DocumentKey) as Dictionary<string, Abbreviation>;
        }
    }
}