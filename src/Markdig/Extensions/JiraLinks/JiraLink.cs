// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Diagnostics;
using Markdig.Helpers;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.JiraLinks
{
    /// <summary>
    /// Model for a JIRA link item
    /// </summary>
    [DebuggerDisplay("{ProjectKey}-{Issue}")]
    public class JiraLink : LinkInline
    {
        public JiraLink()
        {
            IsClosed = true;
        }

        /// <summary>
        /// JIRA Project Key
        /// </summary>
        public StringSlice ProjectKey { get; set; }

        /// <summary>
        /// JIRA Issue Number
        /// </summary>
        public StringSlice Issue { get; set; }
    }
}
