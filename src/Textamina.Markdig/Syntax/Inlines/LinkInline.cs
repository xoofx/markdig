// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
namespace Textamina.Markdig.Syntax.Inlines
{
    public class LinkInline : ContainerInline
    {
        public LinkInline()
        {
        }

        public LinkInline(string url, string title)
        {
            Url = url;
            Title = title;
        }

        public string Url { get; set; }

        public string Title { get; set; }

        public bool IsImage { get; set; }

        public override string ToString()
        {
            return (IsImage ? "<img src=\"" : "<a href=\"") + Url + "\" title=\"" + Title + "\">";
        }
    }
}
