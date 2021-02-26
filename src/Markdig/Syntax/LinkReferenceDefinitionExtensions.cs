// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;

namespace Markdig.Syntax
{
    /// <summary>
    /// Extension methods for accessing <see cref="LinkReferenceDefinition"/> attached at the document level.
    /// </summary>
    public static class LinkReferenceDefinitionExtensions
    {
        private static readonly object DocumentKey = typeof(LinkReferenceDefinitionGroup);

        public static bool ContainsLinkReferenceDefinition(this MarkdownDocument document, string label)
        {
            if (label == null) ThrowHelper.ArgumentNullException_label();
            var references = document.GetData(DocumentKey) as LinkReferenceDefinitionGroup;
            if (references == null)
            {
                return false;
            }
            return references.Links.ContainsKey(label);
        }

        public static void SetLinkReferenceDefinition(this MarkdownDocument document, string label, LinkReferenceDefinition linkReferenceDefinition)
        {
            if (label == null) ThrowHelper.ArgumentNullException_label();
            var references = document.GetLinkReferenceDefinitions();
            references.Set(label, linkReferenceDefinition);
        }

        public static bool TryGetLinkReferenceDefinition(this MarkdownDocument document, string label, out LinkReferenceDefinition linkReferenceDefinition)
        {
            if (label == null) ThrowHelper.ArgumentNullException_label();
            linkReferenceDefinition = null;
            var references = document.GetData(DocumentKey) as LinkReferenceDefinitionGroup;
            if (references == null)
            {
                return false;
            }
            return references.TryGet(label, out linkReferenceDefinition);
        }

        public static LinkReferenceDefinitionGroup GetLinkReferenceDefinitions(this MarkdownDocument document)
        {
            var references = document.GetData(DocumentKey) as LinkReferenceDefinitionGroup;
            if (references == null)
            {
                references = new LinkReferenceDefinitionGroup();
                document.SetData(DocumentKey, references);
                document.Add(references);
            }
            return references;
        }
    }
}