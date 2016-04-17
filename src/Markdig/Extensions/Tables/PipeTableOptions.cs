// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
namespace Markdig.Extensions.Tables
{
    /// <summary>
    /// Options for the extension <see cref="PipeTableExtension"/>
    /// </summary>
    public class PipeTableOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipeTableOptions"/> class.
        /// </summary>
        public PipeTableOptions()
        {
            RequireHeaderSeparator = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to require header separator. <c>true</c> by default (Kramdown is using <c>false</c>)
        /// </summary>
        public bool RequireHeaderSeparator { get; set; }
    }
}