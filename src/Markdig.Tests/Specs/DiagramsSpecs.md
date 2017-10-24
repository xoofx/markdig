# Extensions

Adds support for diagrams extension:

## Mermaid diagrams
 
Using a fenced code block with the `mermaid` language info will output a `<div class='mermaid'>` instead of a `pre/code` block:

```````````````````````````````` example
```mermaid
graph TD;
    A-->B;
    A-->C;
    B-->D;
    C-->D;
```
.
<div class="mermaid">graph TD;
    A-->B;
    A-->C;
    B-->D;
    C-->D;
</div>
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