# The Abstract Syntax Tree

If successful, the `Markdown.Parse(...)` method returns the abstract syntax tree (AST) of the source text.

This will be an object of the `MarkdownDocument` type, which is in turn derived from a more general block container and is part of a larger taxonomy of classes which represent different semantic constructs of a markdown syntax tree.

This document will discuss the different types of elements within the Markdig representation of the AST.

## Structure of the AST

Within Markdig, there are two general types of node in the markdown syntax tree: `Block`, and `Inline`.  Block nodes may contain inline nodes, but the reverse is not true.  Blocks may contain other blocks, and inlines may contain other inlines.

The root of the AST is the `MarkdownDocument` which is itself derived from a container block but also contains information on the line count and starting positions within the document.  Nodes in the AST have links both to parent and children, allowing the edges in the tree to be traversed efficiently in either direction.

Different semantic constructs are represented by types derived from the `Block` and `Inline` types, which are both `abstract` themselves.  These elements are produced by `BlockParser` and `InlineParser` derived types, respectively, and so new constructs can be added with the implementation of a new block or inline parser and a new block or inline type, as well as an extension to register it in the pipeline. For more information on extending Markdig this way refer to the [Extensions/Parsers](parsing-extensions.md) document.

The AST is assembled by the static method `Markdown.Parse(...)` using the collections of block and inline parsers contained in the `MarkdownPipeline`.  For more detailed information refer to the [Markdig Parsing Overview](parsing-overview.md) document.

### Quick Examples: Descendants API

The easiest way to traverse the abstract syntax tree is with a group of extension methods that have the name `Descendants`.  Several different overloads exist to allow it to search for both `Block` and `Inline` elements, starting from any node in the tree.

The `Descendants` methods return `IEnumerable<MarkdownObject>` or `IEnumerable<T>` as their results.  Internally they are using `yield return` to perform edge traversals lazily.

#### Depth-First Like Traversal of All Elements

```csharp
MarkdownDocument result = Markdown.Parse(sourceText, pipeline);

// Iterate through all MarkdownObjects in a depth-first order
foreach (var item in result.Descendants())
{
    Console.WriteLine(item.GetType());

    // You can use pattern matching to isolate elements of certain type,
    // otherwise you can use the filtering mechanism demonstrated in the
    // next section
    if (item is ListItemBlock listItem) 
    {
        // ...
    }
}
```

#### Filtering of Specific Child Types

Filtering can be performed using the `Descendants<T>()` method, in which T is required to be derived from `MarkdownObject`.

```csharp
MarkdownDocument result = Markdown.Parse(sourceText, pipeline);

// Iterate through all ListItem blocks
foreach (var item in result.Descendants<ListItemBlock>())
{
    // ...
}

// Iterate through all image links
foreach (var item in result.Descendants<LinkInline>().Where(x => x.IsImage)) 
{
    // ...
}
```

#### Combined Hierarchies

The `Descendants` method can be used on any `MarkdownObject`, not just the root node, so complex hierarchies can be queried.

```csharp
MarkdownDocument result = Markdown.Parse(sourceText, pipeline);

// Find all Emphasis inlines which descend from a ListItem block
var items = document.Descendants<ListItemBlock>()
    .Select(block => block.Descendants<EmphasisInline>());

// Find all Emphasis inlines whose direct parent block is a ListItem
var other = document.Descendants<EmphasisInline>()
    .Where(inline => inline.ParentBlock is ListItemBlock);
```

## Block Elements

Block elements all derive from `Block` and may be one of two types:

1. `ContainerBlock`, which is a block which holds other blocks (`MarkdownDocument` is itself derived from this)
2. `LeafBlock`, which is a block that has no child blocks, but may contain inlines

Block elements in markdown refer to things like paragraphs, headings, lists, code, etc.  Most blocks may contain inlines, with the exception of things like code blocks.

### Properties of Blocks

The following are properties of `Block` objects which warrant elaboration. For a full list of properties see the generated API documentation (coming soon).

#### Block Parent
All blocks have a reference to a parent (`Parent`) of type `ContainerBlock?`, which allows for efficient traversal up the abstract syntax tree. The parent will be `null` in the case of the root node (the `MarkdownDocument`). 

#### Parser

All blocks have a reference to a parser (`Parser`) of type `BlockParser?` which refers to the instance of the parser which created this block. 

#### IsOpen Flag

Blocks have an `IsOpen` boolean flag which is set true while they're being parsed and then closed when parsing is complete.  

Blocks are created by `BlockParser` objects which are managed by an instance of a `BlockProcessor` object. During the parsing algorithm the `BlockProcessor` maintains a list of all currently open `Block` objects as it steps through the source line by line. The `IsOpen` flag indicates to the `BlockProcessor` that the block should remain open as the next line begins.  If the `IsOpen` flag is not directly set by the `BlockParser` on each line, the `BlockProcessor` will consider the `Block` fully parsed and will no longer call its `BlockParser` on it.

#### IsBreakable Flag

Blocks are either breakable or not, specified by the `IsBreakable` flag.  If a block is non-breakable it indicates to the parser that the close condition of any parent container do not apply so long as the non-breakable child block is still open.

The only built-in example of this is the `FencedCodeBlock`, which, if existing as the child of a container block of some sort, will prevent that container from being closed before the `FencedCodeBlock` is closed, since any characters inside the `FencedCodeBlock` are considered to be valid code and not the container's close condition.

#### RemoveAfterProcessInlines



## Inline Elements

Inlines in markdown refer to things like embellishments (italics, bold, underline, etc), links, urls, inline code, images, etc.

Inline elements may be one of two types:

1. `Inline`, whose parent is always a `ContainerInline` 
2. `ContainerInline`, derived from `Inline`, which contains other inlines.  `ContainerInline` also has a `ParentBlock` property of type `LeafBlock?`


**(Is there anything special worth documenting about inlines or types of inlines?)**

## The SourceSpan Struct

If the pipeline was configured with `.UsePreciseSourceLocation()`, all elements in the abstract syntax tree will contain a reference to the location in the original source where they occurred.  This is done with the `SourceSpan` type, a custom Markdig `struct` which provides a start and end location.

All objects derived from `MarkdownObject` contain the `Span` property, which is of type `SourceSpan`.  

