# Extensions

This section describes the different extensions supported:

## Footnotes Preserve Labels No BackLink

Allows footnotes that preserve the original labels in markdown:

```````````````````````````````` example
Here is a footnote reference,[^a] and another.[^somenote]

This is another reference to [^a]

[^a]: Here is the footnote.

And another reference to [^somenote]

[^somenote]: Here's one with multiple blocks.

    Subsequent paragraphs are indented to show that they
belong to the previous footnote.

    > This is a block quote
    > Inside a footnote

        { some.code }

    The whole paragraph can be indented, or just the first
    line.  In this way, multi-paragraph footnotes work like
    multi-paragraph list items.

This paragraph won't be part of the note, because it
isn't indented.
.
<p>Here is a footnote reference,<a id="fnref:1" href="#fn:a" class="footnote-ref"><sup>a</sup></a> and another.<a id="fnref:3" href="#fn:somenote" class="footnote-ref"><sup>somenote</sup></a></p>
<p>This is another reference to <a id="fnref:2" href="#fn:a" class="footnote-ref"><sup>a</sup></a></p>
<p>And another reference to <a id="fnref:4" href="#fn:somenote" class="footnote-ref"><sup>somenote</sup></a></p>
<p>This paragraph won't be part of the note, because it
isn't indented.</p>
<div class="footnotes">
<hr />
<ol>
<li id="fn:a">
<p>Here is the footnote.</p>
</li>
<li id="fn:somenote">
<p>Here's one with multiple blocks.</p>
<p>Subsequent paragraphs are indented to show that they
belong to the previous footnote.</p>
<blockquote>
<p>This is a block quote
Inside a footnote</p>
</blockquote>
<pre><code>{ some.code }
</code></pre>
<p>The whole paragraph can be indented, or just the first
line.  In this way, multi-paragraph footnotes work like
multi-paragraph list items.</p>
</li>
</ol>
</div>
````````````````````````````````

Check with multiple consecutive footnotes:

```````````````````````````````` example
Here is a footnote[^h]. And another one[^j]. And a third one[^m]. And a fourth[^4].

[^h]: Footnote 1 text

[^j]: Footnote 2 text

a

[^m]: Footnote 3 text

[^4]: Footnote 4 text
.
<p>Here is a footnote<a id="fnref:1" href="#fn:h" class="footnote-ref"><sup>h</sup></a>. And another one<a id="fnref:2" href="#fn:j" class="footnote-ref"><sup>j</sup></a>. And a third one<a id="fnref:3" href="#fn:m" class="footnote-ref"><sup>m</sup></a>. And a fourth<a id="fnref:4" href="#fn:4" class="footnote-ref"><sup>4</sup></a>.</p>
<p>a</p>
<div class="footnotes">
<hr />
<ol>
<li id="fn:h">
<p>Footnote 1 text</p></li>
<li id="fn:j">
<p>Footnote 2 text</p></li>
<li id="fn:m">
<p>Footnote 3 text</p></li>
<li id="fn:4">
<p>Footnote 4 text</p></li>
</ol>
</div>
````````````````````````````````
