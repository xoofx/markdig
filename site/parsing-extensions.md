# Extensions and Parsers

Markdig was [implemented in such a way](http://xoofx.github.io/blog/2016/06/13/implementing-a-markdown-processor-for-dotnet/) as to be extremely pluggable, with even basic behaviors being mutable and extendable.

The basic mechanism for extension of Markdig is the `IMarkdownExtension` interface, which allows any implementing class to be registered with the pipeline builder and thus to directly modify the collections of `BlockParser` and `InlineParser` objects which end up in the pipeline.

This document discusses the `IMarkdownExtension` interface, the `BlockParser` abstract base class, and the `InlineParser` abstract base class, which together are the foundation of extending Markdig's parsing machinery.

## Creating Extensions

Extensions can vary from very simple to very complicated. 

A simple extension, for example, might simply find a parser already in the pipeline and modify a setting on it.  An example of this is the `SoftlineBreakAsHardlineExtension`, which locates the `LineBreakInlineParser` and modifies a single boolean flag on it.

A complex extension, on the other hand, might add an entire taxonomy of new `Block` and `Inline` types, as well as several related parsers and renderers, and require being added to the the pipeline in a specific order in relation to other extensions which are already configured. The `FootnoteExtension` and `PipeTableExtension` are examples of more complex extensions.

For extensions that don't require order considerations, the implementation of the extension itself is adequate, and the extension can be added to the pipeline with the generic `Use<TExtension>()` method on the pipeline builder. For extensions which do require order considerations, it is best to create an extension method on the `MarkdownPipelineBuilder` to perform the registration.  See the following two sections for further information.

### Implementation of an Extension

The [IMarkdownExtension.cs](https://github.com/xoofx/markdig/blob/master/src/Markdig/IMarkdownExtension.cs) interface specifies two methods which must be implemented.

The first, which takes only the pipeline builder as an argument, is called when the `Build()` method on the pipeline builder is invoked, and should set up any modifications to the parsers or parser collections. These parsers will then be used by the main parsing algorithm to process the source text.

```csharp
void Setup(MarkdownPipelineBuilder pipeline);
```

The second, which takes the pipeline itself and a renderer, is used to set up a rendering component in order to convert any special `MarkdownObject` types associated with the extension into an output.  This is not relevant for parsing, but is necessary for rendering.

```csharp
void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer);
```

The extension can then be registered to the pipeline builder using the `Use<TExtension>()` method.  A skeleton example is given below:

```csharp
public class MySpecialBlockParser : BlockParser 
{
    // ...
}

public class MyExtension : IMarkdownExtension 
{
    void Setup(MarkdownPipelineBuilder pipeline)
    {
        pipeline.BlockParsers.AddIfNotAlready<MySpecialBlockParser>();
    }

    void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) { }
}
```

```csharp
var builder = new MarkdownPipelineBuilder()
    .Use<MyExtension>();
```

### Pipeline Builder Extension Methods

For extensions which require specific ordering and/or need to perform multiple operations to register with the builder, it's recommended to create an extension method.

```csharp
public static class MyExtensionMethods 
{
    public static MarkdownPipelineBuilder UseMyExtension(this MarkdownPipelineBuilder pipeline)
    {
        // Directly access or modify pipeline.Extensions here, with the ability to
        // search for other extensions, insert before or after, remove other extensions,
        // or modify their settings.

        // ...

        return pipeline;
    }
}

```

### Simple Extension Example

An example of a simple extension which does not add any new parsers, but instead creates a new, horrific emphasis tag, marked by triple percentage signs.  This example is based on [CitationExtension.cs](https://github.com/xoofx/markdig/blob/master/src/Markdig/Extensions/Citations/CitationExtension.cs)

```csharp
/// <summary> 
/// An extension which applies to text of the form %%%text%%%
/// </summary>
public class BlinkExtension : IMarkdownExtension
{
    // This setup method will be run when the pipeline builder's `Build()` method is invoked. As this
    // is a simple, self-contained extension we won't be adding anything new, but rather finding an 
    // existing parser already in the pipeline and adding some settings to it.
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        // We check the pipeline builder's inline parser collection and see if we can find a parser
        // registered of the type EmphasisInlineParser. This is the parser which nominally handles
        // bold and italic emphasis, but we know from its documentation that it is a general parser
        // that can have new characters added to it.
        var parser = pipeline.InlineParsers.FindExact<EmphasisInlineParser>();

        // If we find the parser and it doesn't already have the % character registered, we add
        // a descriptor for 3 consecutive % signs. This is specific to the EmphasisInlineParser and
        // is just used here as an example.
        if (parser is not null && !parser.HasEmphasisChar('%'))
        {
            parser.EmphasisDescriptors.Add(new EmphasisDescriptor('%', 3, 3, false));
        }
    }

    // This method is called by the pipeline before rendering, which is a separate operation from 
    // parsing. This implementation is just here for the purpose of the example, in which we 
    // daisy-chain a delegate specific to the EmphasisInlineRenderer to cause an unconscionable tag 
    // to be inserted into the HTML output wherever a %%% annotated span was placed in the source.
    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is not HtmlRenderer) return;

        var emphasisRenderer = renderer.ObjectRenderers.FindExact<EmphasisInlineRenderer>();
        if (emphasisRenderer is null) return;

        var previousTag = emphasisRenderer.GetTag;
        emphasisRenderer.GetTag = inline =>
            (inline.DelimiterCount == 3 && inline.DelimiterChar == '%' ? "blink" : null)
            ?? previousTag(inline);
    }
}
```
