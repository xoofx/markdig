// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

using Markdig.Helpers;

namespace Markdig.Syntax;

/// <summary>
/// Extension methods for accessing <see cref="LinkReferenceDefinition"/> attached at the document level.
/// </summary>
public static class LinkReferenceDefinitionExtensions
{
    private static readonly object DocumentKey = typeof(LinkReferenceDefinitionGroup);

    /// <summary>
    /// Performs the contains link reference definition operation.
    /// </summary>
    public static bool ContainsLinkReferenceDefinition(this MarkdownDocument document, string label)
    {
        if (label is null) ThrowHelper.ArgumentNullException_label();
        var references = document.GetData(DocumentKey) as LinkReferenceDefinitionGroup;
        if (references is null)
        {
            return false;
        }
        return references.Links.ContainsKey(label);
    }

    /// <summary>
    /// Sets link reference definition.
    /// </summary>
    public static void SetLinkReferenceDefinition(this MarkdownDocument document, string label, LinkReferenceDefinition linkReferenceDefinition, bool addGroup)
    {
        if (label is null) ThrowHelper.ArgumentNullException_label();
        var references = document.GetLinkReferenceDefinitions(addGroup);
        references.Set(label, linkReferenceDefinition);
    }

    /// <summary>
    /// Attempts to get link reference definition.
    /// </summary>
    public static bool TryGetLinkReferenceDefinition(this MarkdownDocument document, string label, [NotNullWhen(true)] out LinkReferenceDefinition? linkReferenceDefinition)
    {
        if (label is null) ThrowHelper.ArgumentNullException_label();
        linkReferenceDefinition = null;
        var references = document.GetData(DocumentKey) as LinkReferenceDefinitionGroup;
        if (references is null)
        {
            return false;
        }
        return references.TryGet(label, out linkReferenceDefinition);
    }

    /// <summary>
    /// Gets link reference definitions.
    /// </summary>
    public static LinkReferenceDefinitionGroup GetLinkReferenceDefinitions(this MarkdownDocument document, bool addGroup)
    {
        var references = document.GetData(DocumentKey) as LinkReferenceDefinitionGroup;
        if (references is null)
        {
            references = new LinkReferenceDefinitionGroup();
            document.SetData(DocumentKey, references);
            // don't add the LinkReferenceDefinitionGroup when tracking trivia
            if (addGroup)
            {
                document.Add(references);
            }
        }
        return references;
    }
}
