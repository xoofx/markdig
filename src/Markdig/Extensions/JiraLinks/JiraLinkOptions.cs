using System;
using System.Collections.Generic;
using System.Text;

namespace Markdig.Extensions.JiraLinks
{
    /// <summary>
    /// Available options for replacing JIRA links
    /// </summary>
    public class JiraLinkOptions
    {
        /// <summary>
        /// e.g. https://mycompany.atlassian.net/
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Should the link open in a new window when clicked
        /// </summary>
        public bool OpenInNewWindow { get; set; }

        public JiraLinkOptions()
        {
            OpenInNewWindow = true; //default
        }
    }
}
