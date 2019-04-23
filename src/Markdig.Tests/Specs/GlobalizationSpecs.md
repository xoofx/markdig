# Extensions

This section describes the different extensions supported:

## Globalization
Adds support for RTL content by adding `dir="rtl"` and `align="right` attributes to the appropriate html elements. Left to right text is not affected by this extension.

Whether a markdown block is marked as RTL or not is determined by the [first strong character](https://en.wikipedia.org/wiki/Bi-directional_text#Strong_characters) of the block.

**Note**: You might need to add `<meta charset="UTF-8">` to the head of the html file to be able to see the result correctly.

Headings and block quotes:
```````````````````````````````` example
# Fruits
In botany, a [fruit](https://en.wikipedia.org/wiki/Fruit) is the seed-bearing structure in flowering plants (also known as angiosperms) formed from the ovary after flowering.

> Fruits are good for health
-- Anonymous

# میوە
[میوە](https://ckb.wikipedia.org/wiki/%D9%85%DB%8C%D9%88%DB%95) یان مێوە بەروبوومی ڕوەکیە کە ڕوەکەکان ھەڵیان ئەگرن وەک بەرگێک بۆ تۆوەکانیان، بە زۆری جیادەکرێتەوە بە شیرینی یان ترشی لە تامدا و بە بوونی بڕێکی زۆر ئاو

> میوە بۆ تەندروستی باشە
-- نەزانراو
.
<h1 id="fruits">Fruits</h1>
<p>In botany, a <a href="https://en.wikipedia.org/wiki/Fruit">fruit</a> is the seed-bearing structure in flowering plants (also known as angiosperms) formed from the ovary after flowering.</p>
<blockquote>
<p>Fruits are good for health
-- Anonymous</p>
</blockquote>
<h1 id="section" dir="rtl">میوە</h1>
<p dir="rtl"><a href="https://ckb.wikipedia.org/wiki/%D9%85%DB%8C%D9%88%DB%95" dir="rtl">میوە</a> یان مێوە بەروبوومی ڕوەکیە کە ڕوەکەکان ھەڵیان ئەگرن وەک بەرگێک بۆ تۆوەکانیان، بە زۆری جیادەکرێتەوە بە شیرینی یان ترشی لە تامدا و بە بوونی بڕێکی زۆر ئاو</p>
<blockquote dir="rtl">
<p dir="rtl">میوە بۆ تەندروستی باشە
-- نەزانراو</p>
</blockquote>
````````````````````````````````

Lists:
```````````````````````````````` example
## Types of fruits
- Berries
  - Strawberry
  - kiwifruit
- Citrus
  - Orange
  - Lemon

## Examples of fruits :yum:
1. Apple
2. Banana
3. Orange

## Grocery List
- [X] 􏿽 Watermelon
- [X] Apricot
- [ ] Fig 

## نموونەی میوە :yum:
1. ? سێو
2. 5 مۆز 
3. 􏿽 پرتەقاڵ

## جۆرەکانی میوە
- توو
  - فڕاولە
  - کیوی
- مزرەمەنی
  - پڕتەقاڵ
  - لیمۆ

## لیستی کڕین
- [X] شووتی
- [X] قەیسی
- [ ] هەنجیر
.
<h2 id="types-of-fruits">Types of fruits</h2>
<ul>
<li>Berries
<ul>
<li>Strawberry</li>
<li>kiwifruit</li>
</ul>
</li>
<li>Citrus
<ul>
<li>Orange</li>
<li>Lemon</li>
</ul>
</li>
</ul>
<h2 id="examples-of-fruits">Examples of fruits 😋</h2>
<ol>
<li>Apple</li>
<li>Banana</li>
<li>Orange</li>
</ol>
<h2 id="grocery-list">Grocery List</h2>
<ul class="contains-task-list">
<li class="task-list-item"><input disabled="disabled" type="checkbox" checked="checked" /> 􏿽 Watermelon</li>
<li class="task-list-item"><input disabled="disabled" type="checkbox" checked="checked" /> Apricot</li>
<li class="task-list-item"><input disabled="disabled" type="checkbox" /> Fig</li>
</ul>
<h2 id="section" dir="rtl">نموونەی میوە 😋</h2>
<ol dir="rtl">
<li>? سێو</li>
<li>5 مۆز</li>
<li>􏿽 پرتەقاڵ</li>
</ol>
<h2 id="section-1" dir="rtl">جۆرەکانی میوە</h2>
<ul dir="rtl">
<li>توو
<ul dir="rtl">
<li>فڕاولە</li>
<li>کیوی</li>
</ul>
</li>
<li>مزرەمەنی
<ul dir="rtl">
<li>پڕتەقاڵ</li>
<li>لیمۆ</li>
</ul>
</li>
</ul>
<h2 id="section-2" dir="rtl">لیستی کڕین</h2>
<ul class="contains-task-list" dir="rtl">
<li class="task-list-item"><input disabled="disabled" type="checkbox" checked="checked" /> شووتی</li>
<li class="task-list-item"><input disabled="disabled" type="checkbox" checked="checked" /> قەیسی</li>
<li class="task-list-item"><input disabled="disabled" type="checkbox" /> هەنجیر</li>
</ul>
````````````````````````````````

Tables:

```````````````````````````````` example
Nutrition |Apple | Oranges
--|-- | --
Calories|52|47
Sugar|10g|9g

 پێکهاتە |سێو | پڕتەقاڵ
--|-- | --
کالۆری|٥٢|٤٧
شەکر| ١٠گ|٩گ
.
<table>
<thead>
<tr>
<th>Nutrition</th>
<th>Apple</th>
<th>Oranges</th>
</tr>
</thead>
<tbody>
<tr>
<td>Calories</td>
<td>52</td>
<td>47</td>
</tr>
<tr>
<td>Sugar</td>
<td>10g</td>
<td>9g</td>
</tr>
</tbody>
</table>
<table dir="rtl" align="right">
<thead>
<tr>
<th>پێکهاتە</th>
<th>سێو</th>
<th>پڕتەقاڵ</th>
</tr>
</thead>
<tbody>
<tr>
<td>کالۆری</td>
<td>٥٢</td>
<td>٤٧</td>
</tr>
<tr>
<td>شەکر</td>
<td>١٠گ</td>
<td>٩گ</td>
</tr>
</tbody>
</table>
````````````````````````````````