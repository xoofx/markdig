// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Collections.Generic;

namespace Markdig.Syntax
{
    /// <summary>
    /// Extension methods for accessing <see cref="LinkReferenceDefinition"/> attached at the document level.
    /// </summary>
    public static class LinkReferenceDefinitionExtensions
    {
        private static readonly object DocumentKey = typeof(LinkReferenceDefinition);

        public static bool ContainsLinkReferenceDefinition(this MarkdownDocument document, string label)
        {
            if (label == null) throw new ArgumentNullException(nameof(label));
            var references = document.GetData(DocumentKey) as Dictionary<string, LinkReferenceDefinition>;
            if (references == null)
            {
                return false;
            }
            return references.ContainsKey(label);
        }

        public static void SetLinkReferenceDefinition(this MarkdownDocument document, string label, LinkReferenceDefinition linkReferenceDefinition)
        {
            if (label == null) throw new ArgumentNullException(nameof(label));
            var references = document.GetLinkReferenceDefinitions();
            references[label] = linkReferenceDefinition;
        }

        public static bool RemoveLinkReferenceDefinition(this MarkdownDocument document, string label)
        {
            if (label == null) throw new ArgumentNullException(nameof(label));
            var references = document.GetData(DocumentKey) as Dictionary<string, LinkReferenceDefinition>;
            if (references == null)
            {
                return false;
            }
            return references.Remove(label);
        }

        public static bool TryGetLinkReferenceDefinition(this MarkdownDocument document, string label, out LinkReferenceDefinition linkReferenceDefinition)
        {
            if (label == null) throw new ArgumentNullException(nameof(label));
            linkReferenceDefinition = null;
            var references = document.GetData(DocumentKey) as Dictionary<string, LinkReferenceDefinition>;
            if (references == null)
            {
                return false;
            }
            return references.TryGetValue(label, out linkReferenceDefinition);
        }

        public static Dictionary<string, LinkReferenceDefinition> GetLinkReferenceDefinitions(this MarkdownDocument document)
        {
            var references = document.GetData(DocumentKey) as Dictionary<string, LinkReferenceDefinition>;
            if (references == null)
            {
                references = new Dictionary<string, LinkReferenceDefinition>(StringComparer.OrdinalIgnoreCase);
                document.SetData(DocumentKey, references);
            }
            return references;
        }
    }
}