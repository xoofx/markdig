# Roundtrip parser
Markdig supports parsing trivia characters and tracks the source position of these characters.
This gives the ability to parse a document and then render a slightly changed document back.
Without tracking trivia characters, the renderer must make all kinds of assumptions on newlines, tabs, whitespace characters and other document details.

To use this functionality, set the optional `trackTrivia` parameter to true when using the static `Markdown` class:
```csharp
MarkdownDocument markdownDocument = Markdown.Parse(inputMarkdown, trackTrivia: true);
```

You will get a parse tree where `Block` and `Inline` instances now have various `Trivia*` properties.
To write a document to Markdown using this tree, use the `RoundtripRenderer`:
```csharp
var sw = new StringWriter();
var rr = new RoundtripRenderer(sw);
rr.Write(markdownDocument);
var outputMarkdown = sw.ToString();
```
You should expect the `outputMarkdown` to be equal to the `inputMarkdown`.

# Demo test
For a simple test showcasing the feature, see the [`TestExample.cs`](../Markdig.Tests/RoundtripSpecs/TestExample.cs).

# Trivia
Trivia are not specified by the CommonMark standard. As such, any implementation decides for itself which tree nodes trivia are attached to.

Trivia characters are:
- newlines: `\n`, `\r`, `\r\n`
- ` ` (space), `\f` (form feed), `\v` (vertical tab)
- unescaped string characters

# Newlines
Blocks almost always end with a newline, therefore the `Block` class has it defined as a property:
```csharp
/// <summary>
/// The last newline of this block
/// </summary>
public NewLine NewLine { get; set; }
```
Consider a very simple valid Markdown document (for clarity's sake, the `\n` character is added):
```markdown
p\n
\n
p\n
```
Above document consists of 5 characters, `p`, `\n`, `\n`, `p`, `\n` in sequence.
Obviously, the two `p` characters are part of a separate paragraph block.
The `\n`  right next to each `p` is easy: we'll just attach it to either paragraph block as well.
However, it is not clear what we should do with the middle `\n`: should it be attached to the first `p` or the second `p`?
Let's look at a different example:

```markdown
\n
p\n
\n
```
Here, we only have *one* (paragraph)block, and thus must attach the first `\n` **and** last `\n` to that paragraph block. 
The `Block` class therefore has `LinesBefore` and `LinesAfter` defined:
```csharp
/// <summary>
/// Gets or sets the empty lines occurring before this block.
/// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise null.
/// </summary>
public List<StringSlice> LinesBefore { get; set; }

/// <summary>
/// Gets or sets the empty lines occurring after this block.
/// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise null.
/// </summary>
public List<StringSlice> LinesAfter { get; set; }
```

The choice where to attach the middle `\n` from the first example to is arbitrary.
When parsing, it's easier and simpler to attach it to the first occuring block, so that's what Markdig does. 

**Rule: Newlines are attached to the first occurring node**

The parse tree of the first example then becomes:
1. paragraph block `p`
   - newline: `\n` 
   - after: `\n`
2. paragraph block `p`
   - newline: `\n` 

In the second example, the parse tree is:
1. paragraph block `p`
   - newline: `\n` 
   - before: `\n`
   - after: `\n`

Stated differently: Blocks *almost always have* a newline, *often* have *trivia after* and sometimes have *trivia before*.

Keep in mind that paragraphs are a bit of a special case in Markdown.
This is also the case with trivia parsing, where the `LineBreakInline` is considered part of the paragraph block, and not part of the trivia.
Consider the following example:
```markdown
\n
text1\n
text2\n
\n
```
The first `\n` is attached to the paragraph block as *trivia before*.
The second `\n` is a LineBreakInline inline element, and not considered trivia.
The third `\n` is the newline of the paragraph block.
The fourth `\n` is attached as *trivia after*.

# Trivia before and trivia after

All trivia in a document should be attached to a node. The `Block` class defines two properties to capture this:
```csharp
/// <summary>
/// Gets or sets the trivia right before this block.
/// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
/// <see cref="StringSlice.Empty"/>.
/// </summary>
public StringSlice TriviaBefore { get; set; }

/// <summary>
/// Gets or sets trivia occurring after this block.
/// Trivia: only parsed when <see cref="MarkdownPipeline.TrackTrivia"/> is enabled, otherwise
/// <see cref="StringSlice.Empty"/>.
/// </summary>
public StringSlice TriviaAfter { get; set; }
```

Typically, this trivia occurs within the document before, in between or after blocks.
Take these examples (the *interpunct*, aka *middle dot*: `·` is used to visualize a space character):
```markdown
·*·item1
```
```markdown
··*·item1
```
```markdown
·*··item1
```
All is valid markdown that defines an unordered list with one paragraph block. The parse tree looks like this:
- ListBlock
   - ListItemBlock
     - Paragraph
       - LiteralInline "item1"

The parser assigns the trivia (spaces in above example) to the `ListItemBlock` and `ParagraphBlock` nodes respectively.

# Enclosed trivia
Trivia may occur *within* nodes. In such case, a property is defined for each part of the syntax where trivia may occur. 
Some inlines have escaped strings. These strings are set seperately on the parse tree of that inline. 

`LinkInline` and `FencedCodeBlock` are both examples where trivia is parsed within the node and the node contains properties for both escaped an unescaped strings.

# Links and LinkReferences
Links and LinkReferences have a complex parsing implementation. The codebase currrently consists of a separate set of `Parse*Trivia` methods.
These methods are duplicated from their source `Parse*` methods for simplicity's sake.
Abstracting the trivia parsing in the source methods was considered, but that would make already complex parsing logic even more complex.
Instead, the cost of maintaining a (mature) duplicated codebase was considered to be easier and less complex.

# LinkReference
While LinkReferences are parsed, the `LinkReferenceDefinitionGroup` is not added to the document.
The reason for this is to have the parse tree represent the input text as precise as possible.
Adding the `LinkReferenceDefinitionGroup` would add a node not representing input text, and as such is omitted.

# `/0` character
As per the [CommonMark 0.29 spec], the `/0` aka `U+0000` character is replaced with `/uFFFD`.
Therefore, it is not - and never will be - possible to have exactly equal output Markdown as input, whenever there is a `/0` character in the input.

**Rule: Exactly equal output Markdown given an input Markdown is only possible when the `/0` character is not present in the input Markdown**

# EmptyBlock
The spec states

> Any sequence of characters is a valid CommonMark document.

where

> A character is a Unicode code point. Although some code points (for example, combining accents) do not correspond to characters in an intuitive sense, all code points count as characters for purposes of this spec.

As such an input document containing trivia is, technically, also valid Markdown.
To support rountrip parsing for documents that contain input characters - but these input characters do not resolve to any blocks, the `EmptyBlock` is defined.

**Rule: the `EmptyBlock` is a Block representing a block of Markdown trivia where no other Block types are matched on**

# Extensions
Extensions are currently not supported. If you're a writer or maintainer of an existing extension, would you be interested in writing a pull request to have your extension support roundtrip parsing?
If you need any assistance, please reach out to @generateui. I'd be happy to help.
