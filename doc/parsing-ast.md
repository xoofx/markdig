# The Abstract Syntax Tree

If successful, the `MarkdownParser.Parse(...)` method returns the abstract syntax tree (AST) of the source text.

This will be an object of the `MarkdownDocument` type, which is in turn derived from a more general block container and is part of a larger taxonomy of classes which represent different semantic constructs of a markdown syntax tree.

This document will discuss the different types of elements within the Markdig representation of the AST.

## Structure of the AST

Within Markdig, there are two general types of node in the markdown syntax tree: `Block`, and `Inline`.  Block nodes may contain inline nodes, but the reverse is not true.  Blocks may contain other blocks, and inlines may contain other inlines.

The root of the AST is the `MarkdownDocument` which is itself derived from a container block but also contains information on the line count and starting positions within the document.  Nodes in the AST have links both to parent and children, allowing the edges in the tree to be traversed efficiently in either direction.

Different semantic constructs are represented by types derived from the `Block` and `Inline` types, which are both `abstract` themselves.  These elements are produced by `BlockParser` and `InlineParser` derived types, respectively, and so new constructs can be added with the implementation of a new block or inline parser and a new block or inline type, as well as an extension to register it in the pipeline. For more information on extending Markdig this way refer to the [Extensions/Parsers](parsing-extensions.md) document.

The AST is assembled by the static method `MarkdownParser.Parse(...)` using the collections of block and inline parsers contained in the `MarkdownPipeline`.  For more detailed information refer to the [Markdig Parsing Overview](parsing-overview.md) document.

## Block Elements

Block elements all derive from `Block` and may be one of two types:

1. `ContainerBlock`, which is a block which holds other blocks (`MarkdownDocument` is itself derived from this)
2. `LeafBlock`, which is a block that has no child blocks, but may contain inlines

Block elements in markdown refer to things like paragraphs, headings, lists, code, etc.  Most blocks may contain inlines, with the exception of things like code blocks.

### Properties of Blocks

All blocks have a reference to a parent (`Parent`) of type `ContainerBlock?`, which allows for efficient traversal up the abstract syntax tree. The parent will be `null` in the case of the root node (the `MarkdownDocument`). 

Additionally, all blocks have a reference to a parser (`Parser`) of type `BlockParser?` which refers to the instance of the parser which created this block. 

Blocks have an `IsOpen` boolean flag which is set true while they're being parsed **(is this true?)** and then closed when parsing is complete.  

Blocks are either breakable or not, specified by the `IsBreakable` flag.  **(What is the significance of breakable blocks? Can't be split? Is anything besides `FencedCodeBlock` not breakable?)**

**(Is there anything special that should be documented for the ParagraphBlock or any other specific type of blocks?)**

## Inline Elements

Inline elements may be one of two types:

1. `ContainerInline`, which contains other inlines, and whose parent may be a `LeafBlock` or another `ContainerInline` (**is this true?**)
2. `Inline`, whose parent is always a `ContainerInline` (**is this true?**)

Inlines in markdown refer to things like embellishments (italics, bold, underline, etc), links, urls, inline code, images, etc.

**(Is there anything special worth documenting about inlines or types of inlines?)**

## The SourceSpan Struct

If the pipeline was configured with `.UsePreciseSourceLocation()`, all elements in the abstract syntax tree will contain a reference to the location in the original source where they occurred.  This is done with the `SourceSpan` class, a custom Markdig `struct` which provides a start and end location.

All objects derived from `MarkdownObject` contain the `Span` property, which is of type `SourceSpan`.  
