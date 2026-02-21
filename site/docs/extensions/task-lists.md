---
title: Task lists
---

# Task lists

Enable with `.UseTaskLists()` (included in `UseAdvancedExtensions()`).

Task lists add checkbox-style list items, inspired by [GitHub task lists](https://docs.github.com/en/get-started/writing-on-github/working-with-advanced-formatting/about-task-lists).

## Syntax

Start a list item with `[ ]` (unchecked) or `[x]`/`[X]` (checked):

```markdown
- [x] Write documentation
- [x] Implement feature
- [ ] Write tests
- [ ] Release
```

- [x] Write documentation
- [x] Implement feature
- [ ] Write tests
- [ ] Release

## In ordered lists

Task lists also work with ordered list items:

```markdown
1. [x] First task
2. [ ] Second task
3. [ ] Third task
```

1. [x] First task
2. [ ] Second task
3. [ ] Third task

## HTML output

Checked items render as `<input type="checkbox" disabled checked />`, unchecked items as `<input type="checkbox" disabled />`:

```html
<ul class="contains-task-list">
  <li class="task-list-item"><input type="checkbox" disabled checked /> Write docs</li>
  <li class="task-list-item"><input type="checkbox" disabled /> Write tests</li>
</ul>
```
