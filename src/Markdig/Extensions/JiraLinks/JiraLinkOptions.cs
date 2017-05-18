// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Text;

namespace Markdig.Extensions.JiraLinks
{
    /// <summary>
    /// Available options for replacing JIRA links
    /// </summary>
    public class JiraLinkOptions
    {
        /// <summary>
        /// The base Url (e.g. `https://mycompany.atlassian.net`)
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// The base path after the base url (default is `/browse`)
        /// </summary>
        public string BasePath { get; set; }

        /// <summary>
        /// Should the link open in a new window when clicked
        /// </summary>
        public bool OpenInNewWindow { get; set; }

        public JiraLinkOptions(string baseUrl)
        {
            OpenInNewWindow = true; //default
            BaseUrl = baseUrl;
            BasePath = "/browse";
        }

        /// <summary>
        /// Gets the full url composed of the <see cref="BaseUrl"/> and <see cref="BasePath"/> with no trailing `/`
        /// </summary>
        public virtual string GetUrl()
        {
            var url = new StringBuilder();
            var baseUrl = BaseUrl;
            if (baseUrl != null)
            {
                url.Append(baseUrl.TrimEnd('/'));
            }

            url.Append("/");

            if (BasePath != null)
            {
                url.Append(BasePath.Trim('/'));
            }

            return url.ToString();
        }
    }
}
