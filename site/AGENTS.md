# Markdig Documentation Site — Agent Instructions

This folder contains the [Lunet](https://lunet.io)-based documentation site for Markdig.

Single source of truth for the overall project: read and follow `../AGENTS.md`.

## Orientation

- `config.scriban` — Site configuration (theme, metadata, API generation)
- `menu.yml` — Top-level navigation
- `readme.md` — Landing/home page
- `docs/` — All documentation content
  - `docs/getting-started.md` — Installation & first steps
  - `docs/usage.md` — Parse/render, pipeline architecture
  - `docs/commonmark.md` — CommonMark syntax reference
  - `docs/extensions/` — Extension reference (one page per extension or group)
  - `docs/advanced/` — Developer guide (AST, parsers, renderers, performance)
- `img/` — Site images (logo, banner)
- `specs/` — Versioned API specification documents (historical reference)

## Build & Serve

```sh
# Prerequisites: install lunet as a .NET global tool
dotnet tool install -g lunet

# From this directory (site/)
lunet build          # production build → .lunet/build/www/
lunet serve          # dev server with live reload at http://localhost:4000
```

## Content Conventions

- Pages use Markdown with YAML front matter (`title` required).
- Navigation is defined in `menu.yml` files (one per section/folder).
- The site uses `UseAdvancedExtensions()` so all Markdig extensions render natively — extension docs can demonstrate syntax live.
- Because the site is processed by Scriban via Lunet, any literal `{{` or `}}` in Markdown must be escaped as `{{ "{{" }}` and `{{ "}}" }}`.
- Keep code examples short, correct, and copy-pasteable.
- When adding a new page, add a corresponding entry in the relevant `menu.yml`.

## What to Update

When library behavior changes:
- Update the relevant extension or feature page in `docs/`.
- If a new extension is added, create a page under `docs/extensions/` and add it to `docs/extensions/menu.yml`.
- If public API changes affect developer guide content, update pages in `docs/advanced/`.
