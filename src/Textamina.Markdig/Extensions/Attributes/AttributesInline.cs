using System;
using System.Collections.Generic;
using Textamina.Markdig.Renderers.Html;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Extensions.Attributes
{
    public class AttributesInline : Inline
    {
        public string Id { get; set; }

        public List<string> Classes { get; set; }

        public List<KeyValuePair<string ,string>> Properties { get; set; }

        public void CopyTo(HtmlAttributes htmlAttributes)
        {
            if (htmlAttributes == null) throw new ArgumentNullException(nameof(htmlAttributes));
            // Add html htmlAttributes to the object
            htmlAttributes.Id = Id;
            if (htmlAttributes.Classes == null)
            {
                htmlAttributes.Classes = Classes;
            }
            else if (Classes != null)
            {
                htmlAttributes.Classes.AddRange(Classes);
            }

            if (htmlAttributes.Properties == null)
            {
                htmlAttributes.Properties = Properties;
            }
            else if (Properties != null)
            {
                htmlAttributes.Properties.AddRange(Properties);
            }
        }
    }
}