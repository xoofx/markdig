// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

namespace Markdig.Parsers;

/// <summary>
/// The default parser used to parse unordered list item (-, +, *)
/// </summary>
/// <seealso cref="ListItemParser" />
public class UnorderedListItemParser : ListItemParser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnorderedListItemParser"/> class.
    /// </summary>
    public UnorderedListItemParser()
    {
        OpeningCharacters = ['-', '+', '*'];
    }

    public override bool TryParse(BlockProcessor state, char pendingBulletType, out ListInfo result)
    {
        result = new ListInfo(state.CurrentChar);
        state.NextChar();
        return true;
    }
}