// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

namespace Markdig.Extensions.HtmlTagFilter;

/// <summary>
/// Options for filtering HTML tags via whitelist or blacklist.
/// </summary>
public class HtmlTagFilterOptions
{
    /// <summary>
    /// Gets or sets the whitelist of allowed HTML tags. If non-empty, only these tags are allowed.
    /// When both <see cref="AllowedTags"/> and <see cref="BlockedTags"/> are set, the whitelist takes precedence.
    /// </summary>
    public HashSet<string> AllowedTags { get; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets or sets the blacklist of blocked HTML tags. If non-empty, these tags are blocked.
    /// This is only used when <see cref="AllowedTags"/> is null or empty.
    /// </summary>
    public HashSet<string> BlockedTags { get; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Determines whether the specified tag name is allowed based on the configured whitelist/blacklist.
    /// </summary>
    /// <param name="tagName">The HTML tag name to check (without angle brackets).</param>
    /// <returns>True if the tag is allowed; false if it should be blocked.</returns>
    public bool IsTagAllowed(string tagName)
    {
        if (string.IsNullOrEmpty(tagName))
        {
            return false;
        }

        // Whitelist takes precedence: if defined, tag must be in whitelist
        if (AllowedTags != null && AllowedTags.Count > 0)
        {
            return AllowedTags.Contains(tagName);
        }

        // Blacklist: if defined, tag must NOT be in blacklist
        if (BlockedTags != null && BlockedTags.Count > 0)
        {
            return !BlockedTags.Contains(tagName);
        }

        // No filters configured: allow all
        return true;
    }
}
