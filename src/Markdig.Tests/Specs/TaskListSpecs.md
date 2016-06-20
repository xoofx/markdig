# Extensions

Adds support for task lists:

## TaskLists
 
A task list item consist of `[ ]` or `[x]` or `[X]` inside a list item (ordered or unordered)

```````````````````````````````` example
- [ ] Item1
- [x] Item2
- [ ] Item3
- Item4
.
<ul class="contains-task-list">
<li class="task-list-item"><input disabled="disabled" type="checkbox" /> Item1</li>
<li class="task-list-item"><input disabled="disabled" type="checkbox" checked="checked" /> Item2</li>
<li class="task-list-item"><input disabled="disabled" type="checkbox" /> Item3</li>
<li>Item4</li>
</ul>
````````````````````````````````

A task is not recognized outside a list item:

```````````````````````````````` example
[ ] This is not a task list
.
<p>[ ] This is not a task list</p>
````````````````````````````````
