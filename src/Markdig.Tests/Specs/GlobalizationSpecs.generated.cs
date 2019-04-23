// Generated: 2019-04-15 05:25:26

// --------------------------------
//           Globalization
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.Globalization
{
    [TestFixture]
    public class TestExtensionsGlobalization
    {
        // # Extensions
        // 
        // This section describes the different extensions supported:
        // 
        // ## Globalization
        // Adds support for RTL content by adding `dir="rtl"` and `align="right` attributes to the appropriate html elements. Left to right text is not affected by this extension.
        // 
        // Whether a markdown block is marked as RTL or not is determined by the [first strong character](https://en.wikipedia.org/wiki/Bi-directional_text#Strong_characters) of the block.
        // 
        // **Note**: You might need to add `<meta charset="UTF-8">` to the head of the html file to be able to see the result correctly.
        // 
        // Headings and block quotes:
        [Test]
        public void ExtensionsGlobalization_Example001()
        {
            // Example 1
            // Section: Extensions / Globalization
            //
            // The following Markdown:
            //     # Fruits
            //     In botany, a [fruit](https://en.wikipedia.org/wiki/Fruit) is the seed-bearing structure in flowering plants (also known as angiosperms) formed from the ovary after flowering.
            //     
            //     > Fruits are good for health
            //     -- Anonymous
            //     
            //     # میوە
            //     [میوە](https://ckb.wikipedia.org/wiki/%D9%85%DB%8C%D9%88%DB%95) یان مێوە بەروبوومی ڕوەکیە کە ڕوەکەکان ھەڵیان ئەگرن وەک بەرگێک بۆ تۆوەکانیان، بە زۆری جیادەکرێتەوە بە شیرینی یان ترشی لە تامدا و بە بوونی بڕێکی زۆر ئاو
            //     
            //     > میوە بۆ تەندروستی باشە
            //     -- نەزانراو
            //
            // Should be rendered as:
            //     <h1 id="fruits">Fruits</h1>
            //     <p>In botany, a <a href="https://en.wikipedia.org/wiki/Fruit">fruit</a> is the seed-bearing structure in flowering plants (also known as angiosperms) formed from the ovary after flowering.</p>
            //     <blockquote>
            //     <p>Fruits are good for health
            //     -- Anonymous</p>
            //     </blockquote>
            //     <h1 id="section" dir="rtl">میوە</h1>
            //     <p dir="rtl"><a href="https://ckb.wikipedia.org/wiki/%D9%85%DB%8C%D9%88%DB%95" dir="rtl">میوە</a> یان مێوە بەروبوومی ڕوەکیە کە ڕوەکەکان ھەڵیان ئەگرن وەک بەرگێک بۆ تۆوەکانیان، بە زۆری جیادەکرێتەوە بە شیرینی یان ترشی لە تامدا و بە بوونی بڕێکی زۆر ئاو</p>
            //     <blockquote dir="rtl">
            //     <p dir="rtl">میوە بۆ تەندروستی باشە
            //     -- نەزانراو</p>
            //     </blockquote>

            Console.WriteLine("Example 1\nSection Extensions / Globalization\n");
            TestParser.TestSpec("# Fruits\nIn botany, a [fruit](https://en.wikipedia.org/wiki/Fruit) is the seed-bearing structure in flowering plants (also known as angiosperms) formed from the ovary after flowering.\n\n> Fruits are good for health\n-- Anonymous\n\n# میوە\n[میوە](https://ckb.wikipedia.org/wiki/%D9%85%DB%8C%D9%88%DB%95) یان مێوە بەروبوومی ڕوەکیە کە ڕوەکەکان ھەڵیان ئەگرن وەک بەرگێک بۆ تۆوەکانیان، بە زۆری جیادەکرێتەوە بە شیرینی یان ترشی لە تامدا و بە بوونی بڕێکی زۆر ئاو\n\n> میوە بۆ تەندروستی باشە\n-- نەزانراو", "<h1 id=\"fruits\">Fruits</h1>\n<p>In botany, a <a href=\"https://en.wikipedia.org/wiki/Fruit\">fruit</a> is the seed-bearing structure in flowering plants (also known as angiosperms) formed from the ovary after flowering.</p>\n<blockquote>\n<p>Fruits are good for health\n-- Anonymous</p>\n</blockquote>\n<h1 id=\"section\" dir=\"rtl\">میوە</h1>\n<p dir=\"rtl\"><a href=\"https://ckb.wikipedia.org/wiki/%D9%85%DB%8C%D9%88%DB%95\" dir=\"rtl\">میوە</a> یان مێوە بەروبوومی ڕوەکیە کە ڕوەکەکان ھەڵیان ئەگرن وەک بەرگێک بۆ تۆوەکانیان، بە زۆری جیادەکرێتەوە بە شیرینی یان ترشی لە تامدا و بە بوونی بڕێکی زۆر ئاو</p>\n<blockquote dir=\"rtl\">\n<p dir=\"rtl\">میوە بۆ تەندروستی باشە\n-- نەزانراو</p>\n</blockquote>", "globalization+advanced+emojis");
        }

        // Lists:
        [Test]
        public void ExtensionsGlobalization_Example002()
        {
            // Example 2
            // Section: Extensions / Globalization
            //
            // The following Markdown:
            //     ## Types of fruits
            //     - Berries
            //       - Strawberry
            //       - kiwifruit
            //     - Citrus
            //       - Orange
            //       - Lemon
            //     
            //     ## Examples of fruits :yum:
            //     1. Apple
            //     2. Banana
            //     3. Orange
            //     
            //     ## Grocery List
            //     - [X] 􏿽 Watermelon
            //     - [X] Apricot
            //     - [ ] Fig 
            //     
            //     ## نموونەی میوە :yum:
            //     1. ? سێو
            //     2. 5 مۆز 
            //     3. 􏿽 پرتەقاڵ
            //     
            //     ## جۆرەکانی میوە
            //     - توو
            //       - فڕاولە
            //       - کیوی
            //     - مزرەمەنی
            //       - پڕتەقاڵ
            //       - لیمۆ
            //     
            //     ## لیستی کڕین
            //     - [X] شووتی
            //     - [X] قەیسی
            //     - [ ] هەنجیر
            //
            // Should be rendered as:
            //     <h2 id="types-of-fruits">Types of fruits</h2>
            //     <ul>
            //     <li>Berries
            //     <ul>
            //     <li>Strawberry</li>
            //     <li>kiwifruit</li>
            //     </ul>
            //     </li>
            //     <li>Citrus
            //     <ul>
            //     <li>Orange</li>
            //     <li>Lemon</li>
            //     </ul>
            //     </li>
            //     </ul>
            //     <h2 id="examples-of-fruits">Examples of fruits 😋</h2>
            //     <ol>
            //     <li>Apple</li>
            //     <li>Banana</li>
            //     <li>Orange</li>
            //     </ol>
            //     <h2 id="grocery-list">Grocery List</h2>
            //     <ul class="contains-task-list">
            //     <li class="task-list-item"><input disabled="disabled" type="checkbox" checked="checked" /> 􏿽 Watermelon</li>
            //     <li class="task-list-item"><input disabled="disabled" type="checkbox" checked="checked" /> Apricot</li>
            //     <li class="task-list-item"><input disabled="disabled" type="checkbox" /> Fig</li>
            //     </ul>
            //     <h2 id="section" dir="rtl">نموونەی میوە 😋</h2>
            //     <ol dir="rtl">
            //     <li>? سێو</li>
            //     <li>5 مۆز</li>
            //     <li>􏿽 پرتەقاڵ</li>
            //     </ol>
            //     <h2 id="section-1" dir="rtl">جۆرەکانی میوە</h2>
            //     <ul dir="rtl">
            //     <li>توو
            //     <ul dir="rtl">
            //     <li>فڕاولە</li>
            //     <li>کیوی</li>
            //     </ul>
            //     </li>
            //     <li>مزرەمەنی
            //     <ul dir="rtl">
            //     <li>پڕتەقاڵ</li>
            //     <li>لیمۆ</li>
            //     </ul>
            //     </li>
            //     </ul>
            //     <h2 id="section-2" dir="rtl">لیستی کڕین</h2>
            //     <ul class="contains-task-list" dir="rtl">
            //     <li class="task-list-item"><input disabled="disabled" type="checkbox" checked="checked" /> شووتی</li>
            //     <li class="task-list-item"><input disabled="disabled" type="checkbox" checked="checked" /> قەیسی</li>
            //     <li class="task-list-item"><input disabled="disabled" type="checkbox" /> هەنجیر</li>
            //     </ul>

            Console.WriteLine("Example 2\nSection Extensions / Globalization\n");
            TestParser.TestSpec("## Types of fruits\n- Berries\n  - Strawberry\n  - kiwifruit\n- Citrus\n  - Orange\n  - Lemon\n\n## Examples of fruits :yum:\n1. Apple\n2. Banana\n3. Orange\n\n## Grocery List\n- [X] 􏿽 Watermelon\n- [X] Apricot\n- [ ] Fig \n\n## نموونەی میوە :yum:\n1. ? سێو\n2. 5 مۆز \n3. 􏿽 پرتەقاڵ\n\n## جۆرەکانی میوە\n- توو\n  - فڕاولە\n  - کیوی\n- مزرەمەنی\n  - پڕتەقاڵ\n  - لیمۆ\n\n## لیستی کڕین\n- [X] شووتی\n- [X] قەیسی\n- [ ] هەنجیر", "<h2 id=\"types-of-fruits\">Types of fruits</h2>\n<ul>\n<li>Berries\n<ul>\n<li>Strawberry</li>\n<li>kiwifruit</li>\n</ul>\n</li>\n<li>Citrus\n<ul>\n<li>Orange</li>\n<li>Lemon</li>\n</ul>\n</li>\n</ul>\n<h2 id=\"examples-of-fruits\">Examples of fruits 😋</h2>\n<ol>\n<li>Apple</li>\n<li>Banana</li>\n<li>Orange</li>\n</ol>\n<h2 id=\"grocery-list\">Grocery List</h2>\n<ul class=\"contains-task-list\">\n<li class=\"task-list-item\"><input disabled=\"disabled\" type=\"checkbox\" checked=\"checked\" /> 􏿽 Watermelon</li>\n<li class=\"task-list-item\"><input disabled=\"disabled\" type=\"checkbox\" checked=\"checked\" /> Apricot</li>\n<li class=\"task-list-item\"><input disabled=\"disabled\" type=\"checkbox\" /> Fig</li>\n</ul>\n<h2 id=\"section\" dir=\"rtl\">نموونەی میوە 😋</h2>\n<ol dir=\"rtl\">\n<li>? سێو</li>\n<li>5 مۆز</li>\n<li>􏿽 پرتەقاڵ</li>\n</ol>\n<h2 id=\"section-1\" dir=\"rtl\">جۆرەکانی میوە</h2>\n<ul dir=\"rtl\">\n<li>توو\n<ul dir=\"rtl\">\n<li>فڕاولە</li>\n<li>کیوی</li>\n</ul>\n</li>\n<li>مزرەمەنی\n<ul dir=\"rtl\">\n<li>پڕتەقاڵ</li>\n<li>لیمۆ</li>\n</ul>\n</li>\n</ul>\n<h2 id=\"section-2\" dir=\"rtl\">لیستی کڕین</h2>\n<ul class=\"contains-task-list\" dir=\"rtl\">\n<li class=\"task-list-item\"><input disabled=\"disabled\" type=\"checkbox\" checked=\"checked\" /> شووتی</li>\n<li class=\"task-list-item\"><input disabled=\"disabled\" type=\"checkbox\" checked=\"checked\" /> قەیسی</li>\n<li class=\"task-list-item\"><input disabled=\"disabled\" type=\"checkbox\" /> هەنجیر</li>\n</ul>", "globalization+advanced+emojis");
        }

        // Tables:
        [Test]
        public void ExtensionsGlobalization_Example003()
        {
            // Example 3
            // Section: Extensions / Globalization
            //
            // The following Markdown:
            //     Nutrition |Apple | Oranges
            //     --|-- | --
            //     Calories|52|47
            //     Sugar|10g|9g
            //     
            //      پێکهاتە |سێو | پڕتەقاڵ
            //     --|-- | --
            //     کالۆری|٥٢|٤٧
            //     شەکر| ١٠گ|٩گ
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>Nutrition</th>
            //     <th>Apple</th>
            //     <th>Oranges</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>Calories</td>
            //     <td>52</td>
            //     <td>47</td>
            //     </tr>
            //     <tr>
            //     <td>Sugar</td>
            //     <td>10g</td>
            //     <td>9g</td>
            //     </tr>
            //     </tbody>
            //     </table>
            //     <table dir="rtl" align="right">
            //     <thead>
            //     <tr>
            //     <th>پێکهاتە</th>
            //     <th>سێو</th>
            //     <th>پڕتەقاڵ</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>کالۆری</td>
            //     <td>٥٢</td>
            //     <td>٤٧</td>
            //     </tr>
            //     <tr>
            //     <td>شەکر</td>
            //     <td>١٠گ</td>
            //     <td>٩گ</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example 3\nSection Extensions / Globalization\n");
            TestParser.TestSpec("Nutrition |Apple | Oranges\n--|-- | --\nCalories|52|47\nSugar|10g|9g\n\n پێکهاتە |سێو | پڕتەقاڵ\n--|-- | --\nکالۆری|٥٢|٤٧\nشەکر| ١٠گ|٩گ", "<table>\n<thead>\n<tr>\n<th>Nutrition</th>\n<th>Apple</th>\n<th>Oranges</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>Calories</td>\n<td>52</td>\n<td>47</td>\n</tr>\n<tr>\n<td>Sugar</td>\n<td>10g</td>\n<td>9g</td>\n</tr>\n</tbody>\n</table>\n<table dir=\"rtl\" align=\"right\">\n<thead>\n<tr>\n<th>پێکهاتە</th>\n<th>سێو</th>\n<th>پڕتەقاڵ</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>کالۆری</td>\n<td>٥٢</td>\n<td>٤٧</td>\n</tr>\n<tr>\n<td>شەکر</td>\n<td>١٠گ</td>\n<td>٩گ</td>\n</tr>\n</tbody>\n</table>", "globalization+advanced+emojis");
        }
    }
}
