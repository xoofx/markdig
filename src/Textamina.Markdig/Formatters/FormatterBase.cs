using System;
using System.Collections.Generic;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Formatters
{
    public abstract class FormatterBase<TProcessor, TState> where TProcessor : FormatterBase<TProcessor, TState>
    {
        private readonly List<FormatterHandlerBase<TProcessor, TState>> handlers;
        private readonly Dictionary<Type, FormatterHandlerBase<TProcessor, TState>> handlersPerType;

        public FormatterBase()
        {
            handlers = new List<FormatterHandlerBase<TProcessor, TState>>();
            handlersPerType = new Dictionary<Type, FormatterHandlerBase<TProcessor, TState>>();
        }

        public void VisitContainer(TState state, ContainerBlock containerBlock)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            if (containerBlock == null)
            {
                return;
            }

            foreach (var block in containerBlock.Children)
            {
                Visit(state, block);
            }
        }

        public void VisitContainer(TState state, ContainerInline containerInline)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            if (containerInline == null)
            {
                return;
            }

            var inline = containerInline.FirstChild;
            while (inline != null)
            {
                Visit(state, inline);
                inline = inline.NextSibling;
            }
        }

        public void Visit(TState state, object obj)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            if (obj == null)
            {
                return;
            }

            FormatterHandlerBase<TProcessor, TState> formatterHandler;
            if (!handlersPerType.TryGetValue(obj.GetType(), out formatterHandler))
            {
                foreach (var testFormatter in handlers)
                {
                    if (testFormatter.Accept((TProcessor)this, state, obj.GetType()))
                    {
                        handlersPerType[obj.GetType()] = testFormatter;
                        break;
                    }
                }
            }
            if (formatterHandler != null)
            {
                formatterHandler.Visit((TProcessor)this, state, obj);
            }
        }
    }
}