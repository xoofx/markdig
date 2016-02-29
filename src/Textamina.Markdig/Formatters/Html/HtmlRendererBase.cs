using System;
using System.Reflection;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Formatters.Html
{
    public abstract class HtmlRendererBase<TWriter, TObject> : FormatterHandlerBase<HtmlFormatter, HtmlWriter>
        where TWriter : HtmlWriter
    {
        public override bool Accept(HtmlFormatter visitor, HtmlWriter state, Type type)
        {
            return typeof(TObject).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }

        public override void Visit(HtmlFormatter visitor, HtmlWriter state, object obj)
        {
            var typedObj = (TObject)obj;
            Write(visitor, state, typedObj);
        }

        protected abstract void Write(HtmlFormatter visitor, HtmlWriter writer, TObject obj);

        protected void WriteLeafInline(HtmlFormatter visitor, HtmlWriter writer, LeafBlock leafBlock)
        {
            var inline = (Inline)leafBlock.Inline;
            if (inline != null)
            {
                while (inline != null)
                {
                    visitor.Visit(writer, inline);
                    inline = inline.NextSibling;
                }
            }
        }

        protected void WriteLeafRawLines(TWriter writer, LeafBlock leafBlock, bool writeEndOfLines, bool escape)
        {
            if (leafBlock.Lines != null)
            {
                var lines = leafBlock.Lines;
                for (int i = 0; i < lines.Count; i++)
                {
                    if (!writeEndOfLines && i > 0)
                    {
                        writer.WriteLine();
                    }
                    var line = lines.Lines[i];
                    if (escape)
                    {
                        HtmlHelper.EscapeHtml(line.ToString(), writer);
                    }
                    else
                    {
                        writer.Write(line.ToString());
                    }
                    if (writeEndOfLines)
                    {
                        writer.WriteLine();
                    }
                }
            }
        }
    }

    public abstract class HtmlRendererBase<TObject> : HtmlRendererBase<HtmlWriter, TObject>
    {
    }
}