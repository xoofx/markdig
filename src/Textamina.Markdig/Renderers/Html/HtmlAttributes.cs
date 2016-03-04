using System.Collections.Generic;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Renderers.Html
{
    public class HtmlAttributes
    {
        public HtmlAttributes()
        {
        }

        public string Id { get; set; }

        public List<string> Classes { get; set; }

        public List<KeyValuePair<string, string>> Properties { get; set; }

        public void AddClass(string name)
        {
            if (Classes == null)
            {
                Classes = new List<string>();
            }
            Classes.Add(name);
        }

        public void AddProperty(string name, string value)
        {
            if (Properties == null)
            {
                Properties = new List<KeyValuePair<string, string>>();
            }
            Properties.Add(new KeyValuePair<string, string>(name, value));
        }
    }

    public static class HtmlAttributesExtensions
    {
        private static readonly object Key = typeof (HtmlAttributes);

        public static HtmlAttributes TryGetAttributes(this MarkdownObject obj)
        {
            return obj.GetData(Key) as HtmlAttributes;
        }

        public static HtmlAttributes GetAttributes(this MarkdownObject obj)
        {
            var attributes = obj.GetData(Key) as HtmlAttributes;
            if (attributes == null)
            {
                attributes = new HtmlAttributes();
                obj.SetData(Key, attributes);
            }
            return attributes;
        }
    }
}