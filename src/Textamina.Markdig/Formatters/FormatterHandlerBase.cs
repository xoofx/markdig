using System;

namespace Textamina.Markdig.Formatters
{

    public abstract class FormatterHandlerBase<TVisitor, TState> where TVisitor : FormatterBase<TVisitor, TState>
    {
        public abstract bool Accept(TVisitor visitor, TState state, Type type);

        public abstract void Visit(TVisitor visitor, TState state, object objectToVisit);
    }
}