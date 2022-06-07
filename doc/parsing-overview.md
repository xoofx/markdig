# Markdig Parsing

Markdig provides efficient, regex-free parsing of markdown documents directly into an abstract syntax tree (AST). The AST is a representation of the markdown document's semantic constructs, which can be manipulated and explored programmatically.

* This document contains a general overview of the parsing system and components and their use
* The [Abstract Syntax Tree](parsing-ast.md) document contains a discussion of how Markdig represents the product of the parsing operation
* The [Extensions/Parsers](parsing-extensions.md) document explores extensions and block/inline parsers within the context of extending Markdig's parsing capabilities

## Introduction

Markdig's parsing machinery consists of two main components at its surface: the `Markdown.Parse(...)` method and the `MarkdownPipeline` type.  The parsed document is represented by a `MarkdownDocument` object, which is a tree of objects derived from `MarkdownObject`, including block and inline elements. 

The `Markdown` static class is the main entrypoint to the Markdig API. It contains the `Parse(...)` method, the main algorithm for parsing a markdown document. The `Parse(...)` method in turn uses a `MarkdownPipeline`, which is a sealed internal class which maintains some configuration information and the collections of parsers and extensions.  The `MarkdownPipeline` determines how the parser behaves and what its capabilities are.  The `MarkdownPipeline` can be modified with built-in as well as user developed extensions.

### Glossary of Relevant Types

The following is a table of some of the types relevant to parsing and mentioned in the related documentation. For an exhaustive list refer to API documentation (coming soon).

|Type|Description|
|-|-|
|`Markdown`|Static class with the entry point to the parsing algorithm via the `Parse(...)` method|
|`MarkdownPipeline`|Configuration object for the parser, contains collections of block and inline parsers and registered extensions|
|`MarkdownPipelineBuilder`|Responsible for constructing the `MarkdownPipeline`, used by client code to configure pipeline options and behaviors|
|`IMarkdownExtension`|Interface for [Extensions](#extensions-imarkdownextension) which alter the behavior of the pipeline, this is the standard mechanism for extending Markdig|
|`BlockParser`|Base type for an individual parsing component meant to identify `Block` elements in the markdown source|
|`InlineParser`|Base type for an individual parsing component meant to identify `Inline` elements within a `Block`|
|`Block`|A node in the AST representing a markdown block element, can either be a `ContainerBlock` or a `LeafBlock`|
|`Inline`|A node in the AST representing a markdown inline element|
|`MarkdownDocument`|The root node of the AST produced by the parser, derived from `ContainerBlock`|
|`MarkdownObject`|The base type of all `Block` and `Inline` derived objects (as well as `HtmlAttributes`)|

### Simple Examples

*The following are simple examples of parsing to help get you started, see the following sections for an in-depth explanation of the different parts of Markdig's parsing mechanisms*

The `MarkdownPipeline` dictate how the parser will behave.  The `Markdown.Parse(...)` method will construct a default pipeline if none is provided.  A default pipeline will be CommonMark compliant but nothing else.

```csharp
var markdownText = File.ReadAllText("sample.md");

// No pipeline provided means a default pipeline will be used
var document = Markdown.Parse(markdownText);
```

Pipelines can be created and configured manually, however this must be done using a `MarkdownPipelineBuilder` object, which then is configured through a fluent interface composed of extension methods.  

```csharp
var markdownText = File.ReadAllText("sample.md");

// Markdig's "UseAdvancedExtensions" option includes many common extensions beyond
// CommonMark, such as citations, figures, footnotes, grid tables, mathematics
// task lists, diagrams, and more.
var pipeline = new MarkdownPipelineBuilder()
    .UseAdvancedExtensions()
    .Build();

var document = Markdown.Parse(markdownText, pipeline);
```

Extensions can also be added individually:

```csharp
var markdownText = File.ReadAllText("sample.md");

var pipeline = new MarkdownPipelineBuilder()
    .UseCitations()
    .UseFootnotes()
    .UseMyCustomExtension()
    .Build();

var document = Markdown.Parse(markdownText, pipeline);
```

## Markdown.Parse and the MarkdownPipeline

As metioned in the [Introduction](#introduction), Markdig's parsing machinery involves two surface components: the `Markdown.Parse(...)` method, and the `MarkdownPipeline` type.  The main parsing algorithm (not to be confused with individual `BlockParser` and `InlineParser` components) lives in the `Markdown.Parse(...)` static method. The `MarkdownPipeline` is responsible for configuring the behavior of the parser.

These two components are covered in further detail in the following sections.

### The MarkdownPipeline

The `MarkdownPipeline` is a sealed internal class which dictates what features the parsing algorithm has.  The pipeline must be created by using a `MarkdownPipelineBuilder` as shown in the examples above.

The `MarkdownPipeline` holds configuration information and collections of extensions and parsers.  Parsers fall into one of two categories:

* Block Parsers (`BlockParser`)
* Inline Parsers (`InlineParser`)

Extensions are classes implementing `IMarkdownExtension` which are allowed to add to the list of parsers, or modify existing parsers and/or renderers. They are invoked to perform their mutations on the pipeline when the pipeline is built by the `MarkdownPipelineBuilder`.

Lastly, the `MarkdownPipeline` contains a few extra elements:

* A configuration setting determining whether or not trivial elements, referred to as *trivia*, (whitespace, extra heading characters, unescaped strings, etc) are to be tracked
* A configuration setting determining whether or not nodes in the resultant abstract syntax tree will refer to their precise original locations in the source
* An optional delegate which will be invoked when the document has been processed.
* An optional `TextWriter` which will get debug logging from the parser

### The Markdown.Parse Method

`Markdown.Parse` is a static method which contains the overall parsing algorithm but not the actual parsing components, which instead are contained within the pipeline.

The `Markdown.Parse(...)` method takes a string containing raw markdown and returns a `MarkdownDocument`, which is the root node in the abstract syntax tree.  The `Parse(...)` method optionally takes a pre-configured `MarkdownPipeline`, but if none is given will create a default pipeline which has minimal features.

Within the `Parse(...)` method, the following sequence of operations occur:

1. The block parsers contained in the pipeline are invoked on the raw markdown text, creating the initial tree of block elements
2. If the pipeline is configured to track markdown trivia (trivial/non-contributing elements), the blocks are expanded to absorb neighboring trivia
3. The inline parsers contained in the pipeline are now invoked on the blocks, populating the inline elements of the abstract syntax tree
4. If a delegate has been configured for when the document has completed processing, it is now invoked
5. The abstract syntax tree (`MarkdownDocument` object) is returned

## The Pipeline Builder and Extensions

The `MarkdownPipeline` determines the behavior and capabilities of the parser, and *extensions* added via the `MarkdownPipelineBuilder` determine the configuration of the pipeline.  

This section discusses the pipeline builder and the concept of *extensions* in more detail.

### Extensions (IMarkdownExtension)

***Note**: This section discusses how to consume extensions by adding them to pipeline. For a discussion on how to implement an extension, refer to the [Extensions/Parsers](parsing-extensions.md) document.*

Extensions are the primary mechanism for modifying the parsers in the pipeline.

An extension is any class which implements the `IMarkdownExtension` interface found in [IMarkdownExtension.cs](https://github.com/xoofx/markdig/blob/master/src/Markdig/IMarkdownExtension.cs). This interface consists solely of two `Setup(...)` overloads, which both take a `MarkdownPipelineBuilder` as the first argument.

When the `MarkdownPipelineBuilder.Build()` method is invoked as the final stage in pipeline construction, the builder runs through the list of registered extensions in order and calls the `Setup(...)` method on each of them.  The extension then has full access to modify both the parser collections themselves (by adding new parsers to it), or to find and modify existing parsers.  

Because of this, *some* extensions may need to be ordered in relation to others, for instance if they modify a parser that gets added by a different extension. The `OrderedList<T>` class contains convenience methods to this end, which aid in finding other extensions by type and then being able to added an item before or after them.

### The MarkdownPipelineBuilder

Because the `MarkdownPipeline` is a sealed internal class, it cannot (and *should* not be attempted to) be created directly.  Rather, the `MarkdownPipelineBuilder` manages the requisite construction of the pipeline after the configuration has been provided by the client code.

As discussed in the [section above](#the-markdownpipeline), the `MarkdownPipeline` primarily consists of a collection of block parsers and a collection of inline parsers, which are provided to the `Markdown.Parse(...)` method and thus determine its features and behavior.  Both the collections and some of the parsers themselves are mutable, and the mechanism of mutation is the `Setup(...)` method of the `IMarkdownExtension` interface.  This is covered in more detail in the section on [Extensions](#extensions-imarkdownextension).

#### The Fluent Interface

A collection of extension methods in the [MarkdownExtensions.cs](https://github.com/xoofx/markdig/blob/master/src/Markdig/MarkdownExtensions.cs) source file provides a convenient fluent API for the configuration of the pipeline builder.  This should be considered the standard way of configuring the builder.

##### Configuration Options

There are several extension methods which apply configurations to the builder which change settings in the pipeline outside of the use of typical extensions. 

|Method|Description|
|-|-|
|`.ConfigureNewLine(...)`|Takes a string which will serve as the newline delimiter during parsing|
|`.DisableHeadings()`|Disables the parsing of ATX and Setex headings|
|`.DisableHtml()`|Disables the parsing of HTML elements|
|`.EnableTrackTrivia()`|Enables the tracking of trivia (trivial elements like whitespace)|
|`.UsePreciseSourceLocation()`|Maps syntax objects to their precise location in the original source, such as would be required for syntax highlighting|

```csharp
var builder = new MarkdownPipelineBuilder()
    .ConfigureNewLine("\r\n")
    .DisableHeadings()
    .DisableHtml()
    .EnableTrackTrivia()
    .UsePreciseSourceLocation();

var pipeline = builder.Build();
```

##### Adding Extensions

All extensions which ship with Markdig can be added through a dedicated fluent method, while user code which implements the `IMarkdownExtension` interface can be added with one of the `Use()` methods, or via a custom extension method implemented in the client code.

Refer to [MarkdownExtensions.cs](https://github.com/xoofx/markdig/blob/master/src/Markdig/MarkdownExtensions.cs) for a full list of extension methods:

```csharp
var builder = new MarkdownPipelineBuilder()
    .UseFootnotes()
    .UseFigures();
```

For custom/user-provided extensions, the `Use<TExtension>(...)` methods allow either a type to be directly added or an already constructed instance to be put into the extension container. Internally they will prevent two of the same type of extension from being added to the container.

```csharp
public class MyExtension : IMarkdownExtension 
{
    // ...
}

// Only works if MyExtension has an empty constructor (aka new())
var builder = new MarkdownPipelineBuilder()
    .Use<MyExtension>();
```

Alternatively:

```csharp
public class MyExtension : IMarkdownExtension 
{
    public MyExtension(object someConfigurationObject) { /* ... */ }
    // ...
}

var instance = new MyExtension(configData);

var builder = new MarkdownPipelineBuilder()
    .Use(instance);
```

##### Adding Extensions with the Configure Method

The `MarkdownPipelineBuilder` has one additional method for the configuration of extensions worth mentioning: the `Configure(...)` method, which takes a `string?` of `+` delimited tokens specifying which extensions should be dynamically configured.  This is a convenience method for the configuration of pipelines whose extensions are only known at runtime.

Refer to [MarkdownExtensions.cs's `Configure(...)`](https://github.com/xoofx/markdig/blob/983187eace6ba02ee16d1443c387267ad6e78f58/src/Markdig/MarkdownExtensions.cs#L538) code for the full list of extensions.


```csharp
var builder = new MarkdownPipelineBuilder()
    .Configure("common+footnotes+figures");

var pipeline = builder.Build();
```

#### Manual Configuration

Internally, the fluent interface wraps manual operations on the three primary collections:

* `MarkdownPipelineBuilder.BlockParsers` - this is an `OrderedList<BlockParser>` of the block parsers
* `MarkdownPipelineBuilder.InlineParsers` - this is an `OrderedList<InlineParser>` of the inline element parsers
* `MarkdownPipelineBuilder.Extensions` - this is an `OrderedList<IMarkdownExtension>` of the extensions

All three collections are `OrderedList<T>`, which is a collection type custom to Markdig which contains special methods for finding and inserting derived types.  With the builder created, manual configuration can be performed by accessing these collections and their elements and modifying them as necessary.  

***Warning**: be aware that it should not be necessary to directly modify either the `BlockParsers` or the `InlineParsers` collections directly during the pipeline configuration. Rather, these can and should be modified whenever possible through the `Setup(...)` method of extensions, which will be deferred until the pipeline is actually built and will allow for ordering such that operations dependent on other operations can be accounted for.*

## Block and Inline Parsers

Let's dive deeper into the parsing system.  With a configured pipeline, the `Markdown.Parse` method will run through two two conceptual passes to produce the abstract syntax tree.

1. First, `BlockProcessor.ProcessLine` is called on the file's lines, one by one, trying to identify block elements in the source
2. Next, an `InlineProcessor` is created or borrowed and run on each block to identify inline elements.

These two conceptual operations dictate Markdig's two types of parsers, both of which derive from `ParserBase<TProcessor>`. 

Block parsers, derived from `BlockParser`, identify block elements from lines in the source text and push them onto the abstract syntax tree.  Inline parsers, derived from `InlineParser`, identify inline elements from `LeafBlock` elements and push them into an attached container: the `ContainerInline? LeafBlock.Inline` property.  

Both inline and block parsers are regex-free, and instead work on finding opening characters and then making fast read-only views into the source text.

### Block Parser

**(The contents of this section I am very unsure of, this is from my reading of the code but I could use some guidance here)**

**(Does `CanInterrupt` specifically refer to interrupting a paragraph block?)**

In order to be added to the parsing pipeline, all block parsers must be derived from `BlockParser`.

Internally, the main parsing algorithm will be stepping through the source text, using the `HasOpeningCharacter(char c)` method of the block parser collection to pre-identify parsers which *could* be opening a block at a given position in the text based on the active character.  Thus any derived implementation needs to set the value of the `char[]? OpeningCharacter` property with the initial characters that might begin the block.

If a parser can potentially open a block at a place in the source text it should expect to have the `TryOpen(BlockProcessor processor)` method called.  This is a virtual method that must be implemented on any derived class.  The `BlockProcessor` argument is a reference to an object which stores the current state of parsing and the position in the source.

**(What are the rules concerning how the `BlockState` return type should work for `TryOpen`? I see examples returning `None`, `Continue`, `BreakDiscard`, `ContinueDiscard`.  How does the return value change the algorithm behavior?)**

**(Should a new block always be pushed into `processor.NewBlocks` in the `TryOpen` method?)**

As the main parsing algorithm moves forward, it will then call `TryContinue(...)` on blocks that were opened in `TryOpen(..)`. 

**(Is this where/how you close a block? Is there anything that needs to be done to perform that beyond `block.UpdateSpanEnd` and returning `BlockState.Break`?)**


### Inline Parsers

Inline parsers extract inline markdown elements from the source, but their starting point is the text of each individual `LeafBlock` produced by the block parsing process.  To understand the role of each inline parser it is necessary to first understand the inline parsing process as a whole.

#### The Inline Parsing Process

After the block parsing process has occurred, the abstract syntax tree of the document has been populated only with block elements, starting from the root `MarkdownDocument` node and ending with the individual `LeafBlock` derived block elements, most of which will be `ParagraphBlocks`, but also include things like `CodeBlocks`, `HeadingBlocks`, `FigureCaptions`, and so on.

At this point, the parsing machinery will iterate through each `LeafBlock` one by one, creating and assigning its `LeafBlock.Inline` property with an empty `ContainerInline`, and then sweeping through the `LeafBlock`'s text running the inline parsers.  This occurs by the following process:

Starting at the first character of the text it will run through all of its `InlineParser` objects which have that character as a possible opening character for the type of inline they extract.  The parsers will run in order (as such ordering is the *only* way which conflicts between parsers are resolved, and thus is important to the overall behavior of the parsing system) and the `Match(...)` method will be called on each candidate parser, in order, until one of them returns `true`.

The `Match(...)` method will be passed a slice of the text beginning at the *specific character* being processed and running until the end of the `LeafBlock`'s complete text.  If the parser can create an `Inline` element it will do so and return `true`, otherwise it will return `false`.  The parser will store the created `Inline` object in the processor's `InlineProcessor.Inline` property, which as passed into the `Match(...)` method as an argument.  The parser will also advance the start of the working `StringSlice` by the characters consumed in the match.

* If the parser has created an inline element and returned `true`, that element is pushed into the deepest open `ContainerInline`
* If `false` was returned, a default `LiteralInlineParser` will run instead:
    * If the `InlineProcessor.Inline` property already has an existing `LiteralInline` in it, these characters will be added to the existing `LiteralInline`, effectively growing it
    * If no `LiteralInline` exists in the `InlineProcessor.Inline` property, a new one will be created containing the consumed characters and pushed into the deepest open `ContainerInline`

After that, the working text of the `LeafBlock` has been conceptually shortened by the advancing start of the working `StringSlice`, moving the starting character forward.  If there is still text remaining, the process repeats from the new starting character until all of the text is consumed.

At this point, when all of the source text from the `LeafBlock` has been consumed, a post-processing step occurs.  `InlineParser` objects in the pipeline which also implement `IPostInlineProcessor` are invoked on the `LeafBlock`'s root `ContainerInline`.  This, for example, is the mechanism by which the unstructured output of the `EmphasisInlineParser` is then restructured into cleanly nested `EmphasisInline` and `LiteralInline` elements.


#### Responsibilities of an Inline Parser

Like the block parsers, an inline parser must provide an array of opening characters with the `char[]? OpeningCharacter` property.

However, inline parsers only require one other method, the `Match(InlineProcessor processor, ref StringSlice slice)` method, which is expected to determine if a match for the related inline is located at the starting character of the slice.

Within the `Match` method a parser should:

1. Determine if a match begins at the starting character of the `slice` argument
2. If no match exists, the method should return `false` and not advance the `Start` property of the `slice` argument
3. If a match does exist, perform the following actions: 
    * Instantiate the appropriate `Inline` derived class and assign it to the processor argument with `processor.Inline = myInlineObject`
    * Advance the `Start` property of the `slice` argument by the number of characters contained in the match, for example by using the `NextChar()`, `SkipChar()`, or other helper methods of the `StringSlice` class
    * Return `true`

While parsing, the `InlineProcessor` performing the processing, which is available to the `Match` function through the `processor` argument, contains a number of properties which can be used to access the current state of parsing.  For example, the `processor.Inline` property is the mechanism for returning a new inline element, but before assignment it contains the last created inline, which in turn can be accessed for its parents.

Additionally, in the case of inlines which can be expected to contain other inlines, a possible strategy is to inject an inline element derived from `DelimiterInline` when the opening delimiter is detected, then to replace the opening delimiter with the final desired element when the closing delimiter is found.  This is the strategy used by the `LinkInlineParser`, for example.  In such cases the tools described in the next section, such as the `ReplaceBy` method, can be used.  Note that if this method is used the post-processing should be invoked on the `InlineProcessor` in order to finalize any emphasis elements.  For example, in the following code adapted from the `LinkInlineParser`:

```csharp
var parent = processor.Inline?.FirstParentOfType<MyDelimiterInline>();
if (parent is null) return;

var myInline = new MySpecialInline { /* set span and other parameters here */ };

// Replace the delimiter inline with the final inline type, adopting all of its children
parent.ReplaceBy(myInline);

// Notifies processor as we are creating an inline locally
processor.Inline = myInline;

// Process emphasis delimiters
processor.PostProcessInlines(0, myInline, null, false);
```

#### Inline Post-Processing

The purpose of post-processing inlines is typically to re-structure inline elements after the initial parsing is complete and the entire structure of the inline elements within a parent container is now available in a way it was not during the parsing process.  Generally this consists of removing, replacing, and re-ordering `Inline` elements.

To this end, the `Inline` abstract base class contains several helper methods intended to allow manipulation of inline elements during the post-processing phase.

|Method|Purpose|
|-|-|
|`InsertAfter(...)`|Takes a new inline as an argument and inserts it into the same parent container after this instance|
|`InsertBefore(...)`|Takes a new inline as an argument and inserts it into the same parent container before this instance|
|`Remove()`|Removes this inline from its parent container|
|`ReplaceBy(...)`|Removes this instance and replaces it with a new inline specified in the argument. Has an option to move all of the original inline's children into the new inline.|

Additionally, the `PreviousSibling` and `NextSibling` properties can be used to determine the siblings of an inline element within its parent container.  The `FirstParentOfType<T>()` method can be used to search for a parent element, which is often useful when searching for `DelimiterInline` derived elements, which are implemented as containers.

