# Extensions

Adds support for diagrams extension:

## Mermaid diagrams
 
Using a fenced code block with the `mermaid` language info will output a `<pre class='mermaid'>` block (which is the default for other code block):

```````````````````````````````` example
```mermaid
graph TD;
    A-->B;
    A-->C;
    B-->D;
    C-->D;
```
.
<pre class="mermaid">graph TD;
    A-->B;
    A-->C;
    B-->D;
    C-->D;
</pre>
````````````````````````````````

## nomnoml diagrams

Using a fenced code block with the `nomnoml` language info will output a `<div class='nomnoml'>` instead of a `pre/code` block:

```````````````````````````````` example
```nomnoml
[example|
  propertyA: Int
  propertyB: string
|
  methodA()
  methodB()
|
  [subA]--[subB]
  [subA]-:>[sub C]
]
```
.
<div class="nomnoml">[example|
  propertyA: Int
  propertyB: string
|
  methodA()
  methodB()
|
  [subA]--[subB]
  [subA]-:>[sub C]
]
</div>
````````````````````````````````

TODO: Add other text diagram languages