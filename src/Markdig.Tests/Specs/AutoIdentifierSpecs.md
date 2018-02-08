# Extensions

This section describes the auto identifier extension

## Heading Auto Identifiers

Allows to automatically creates an identifier for a heading:

```````````````````````````````` example
# This is a heading
.
<h1 id="this-is-a-heading">This is a heading</h1>
````````````````````````````````

Only punctuation `-`, `_` and `.` is kept, all other non letter characters are discarded.
Consecutive same character `-`, `_` or `.` are rendered into a single one
Characters `-`, `_` and `.` at the end of the string are also discarded.

```````````````````````````````` example
# This - is a &@! heading _ with . and ! -
.
<h1 id="this-is-a-heading_with.and">This - is a &amp;@! heading _ with . and ! -</h1>
````````````````````````````````

Formatting (emphasis) are also discarded:

```````````````````````````````` example
# This is a *heading*
.
<h1 id="this-is-a-heading">This is a <em>heading</em></h1>
````````````````````````````````

Links are also removed:

```````````````````````````````` example
# This is a [heading](/url)
.
<h1 id="this-is-a-heading">This is a <a href="/url">heading</a></h1>
````````````````````````````````

If multiple heading have the same text, -1, -2...-n will be postfix to the header id.

```````````````````````````````` example
# This is a heading
# This is a heading
.
<h1 id="this-is-a-heading">This is a heading</h1>
<h1 id="this-is-a-heading-1">This is a heading</h1>
````````````````````````````````

The heading Id will start on the first letter character of the heading, all previous characters will be discarded:

```````````````````````````````` example
# 1.0 This is a heading
.
<h1 id="this-is-a-heading">1.0 This is a heading</h1>
````````````````````````````````

If the heading is all stripped by the previous rules, the id `section` will be used instead:

```````````````````````````````` example
# 1.0 & ^ % *
# 1.0 & ^ % *
.
<h1 id="section">1.0 &amp; ^ % *</h1>
<h1 id="section-1">1.0 &amp; ^ % *</h1>
````````````````````````````````

When the options "AutoLink" is setup, it is possible to link to an existing heading by using the 
exact same Label text as the heading:

```````````````````````````````` example
# This is a heading
[This is a heading]
.
<h1 id="this-is-a-heading">This is a heading</h1>
<p><a href="#this-is-a-heading">This is a heading</a></p>
````````````````````````````````

Links before the heading are also working:

```````````````````````````````` example
[This is a heading]
# This is a heading
.
<p><a href="#this-is-a-heading">This is a heading</a></p>
<h1 id="this-is-a-heading">This is a heading</h1>
````````````````````````````````

The text of the link can be changed:

```````````````````````````````` example
[With a new text][This is a heading]
# This is a heading
.
<p><a href="#this-is-a-heading">With a new text</a></p>
<h1 id="this-is-a-heading">This is a heading</h1>
````````````````````````````````

An autoidentifier should not conflict with an existing link:

```````````````````````````````` example
![scenario image][scenario]
## Scenario
[scenario]: ./scenario.png
.
<p><img src="./scenario.png" alt="scenario image" /></p>
<h2 id="scenario">Scenario</h2>
````````````````````````````````
