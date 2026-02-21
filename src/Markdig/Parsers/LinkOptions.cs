// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdig.Parsers;

/// <summary>
/// Represents the LinkOptions type.
/// </summary>
public class LinkOptions
{
    /// <summary>
    /// Should the link open in a new window when clicked (false by default)
    /// </summary>
    public bool OpenInNewWindow { get; set; }
}
