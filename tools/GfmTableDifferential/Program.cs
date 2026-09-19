using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Markdig;
using Markdig.Extensions.Tables;

// No download or native build is performed here. Both are explicit prerequisites.
const string revision = "499789b49373bfa045d0e7547e5ee63444c77bca";
if (args.Length is not (3 or 4))
{
    Console.Error.WriteLine("Usage: GfmTableDifferential <cmark-gfm executable> <upstream checkout> <report directory> [snapshot.json]");
    return 2;
}
var exe = Path.GetFullPath(args[0]);
var upstream = Path.GetFullPath(args[1]);
var root = Path.GetFullPath(args[2]);
var gitInfo = new ProcessStartInfo("git") { RedirectStandardOutput = true };
foreach (var arg in new[] { "-C", upstream, "rev-parse", "HEAD" }) gitInfo.ArgumentList.Add(arg);
using (var git = Process.Start(gitInfo)!)
{
    var actualRevision = git.StandardOutput.ReadToEnd().Trim();
    git.WaitForExit();
    if (git.ExitCode != 0 || actualRevision != revision) throw new Exception($"Expected upstream revision {revision}; got {actualRevision}");
}
Directory.CreateDirectory(root);
var pipeline = new MarkdownPipelineBuilder().UsePipeTables(new PipeTableOptions { UseGfmRules = true }).Build();
var trivia = new MarkdownPipelineBuilder().UsePipeTables(new PipeTableOptions { UseGfmRules = true }).EnableTrackTrivia().Build();
var cases = new Dictionary<string, string>();
void Add(string group, string md) => cases.TryAdd(md, group);
string Normalize(string html) => Regex.Replace(Regex.Replace(html.Replace("\r\n", "\n"), " align=\"(left|center|right)\"", " style=\"text-align: $1;\""), @"<li>\n?(?=<(?:h[1-6]|table|blockquote|pre|ul|ol|hr|p)[ >])", "<li>\n").Trim();
string Oracle(string input)
{
    using var process = new Process { StartInfo = new ProcessStartInfo(exe, "--unsafe -e table") { RedirectStandardInput = true, RedirectStandardOutput = true, RedirectStandardError = true, StandardInputEncoding = new UTF8Encoding(false), StandardOutputEncoding = Encoding.UTF8, CreateNoWindow = true } };
    process.Start();
    var stdout = process.StandardOutput.ReadToEndAsync();
    process.StandardInput.Write(input);
    process.StandardInput.Close();
    if (!process.WaitForExit(10000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("cmark-gfm did not finish within ten seconds.");
    }
    if (process.ExitCode != 0) throw new Exception(process.StandardError.ReadToEnd());
    return Normalize(stdout.GetAwaiter().GetResult());
}
foreach (var file in new[] { "spec.txt", "extensions.txt" })
{
    var source = File.ReadAllText(Path.Combine(upstream, "test", file)).Replace("\r\n", "\n");
    if (file == "extensions.txt") source = source[..source.IndexOf("## Strikethroughs")];
    int index = 0;
    foreach (Match m in Regex.Matches(source, @"(?ms)^`{32} example([^\n]*)\n(.*?)\n\.\n(.*?)\n`{32}"))
    {
        index++;
        if (m.Groups[1].Value.Trim() is not ("" or "table")) continue;
        Add("upstream-" + file + "-" + index, m.Groups[2].Value.Replace("\u2192", "\t") + "\n");
    }
}
string[] headers = ["a", "|a", "a|", "|a|", "a|b", "|a|b|", "a|b|c", "", "|", "||", "|||", "| a || b |", "`a|b`", @"a\|b", "[a|b](u)", "<i x='a|b'>", "[a]: /url", "a &vert; b"];
string[] separators = ["-", "--", "---", "----", "-|-", "--|--", "---|---", "|---|---|", "|---|", "---|", "|---", ":-", "-:", ":-:", "|:-|", "|:-:|---:|", "---|---|---", "|", "||", "| |", "|:|", "|---||", "||---|", "|---| |", "|- -|", "|: -|", "|- :|", "|--x|", "|\t-\t|", "|\v-\v|", "|\f-\f|", "|\u00a0-\u00a0|", "- | -", " - | - ", "--- | ---", "---\t|\t---"];
foreach (var h in headers) foreach (var s in separators)
{
    Add("header-separator", h + "\n" + s + "\nx|y|z\n");
    Add("header-only", h + "\n" + s);
    Add("preceding-text", "intro\n" + h + "\n" + s + "\nx|y\n");
}
string[] rows = ["plain", "a|b", "|a|b|", "a|b|c|d", "", " ", "|", "||", "|||", "| |", "  ", "\t", "# h", "## h", "---", "-", "--", "- - -", "***", "___", "===", "=", "> quote", "- item", "* item", "+ item", "1. item", "2. item", "-", "1.", "2.", "```", "~~~x", "<div>", "<span>", "<!-- html -->", "<?x?>", "<![CDATA[x]]>", "[a]: /url", "[a]: /url \"title\"", "    indented", "\tindented", "    |a|b|", "\f", "\v", "\u00a0", "\u2003", "|\u00a0x\u00a0|y|", "|\vx\v|y|", "|a\\|b|", "a  ", "a\\", "[a]", "[a](url)"];
string[] prefixes = ["", "> ", "- ", "1. ", "> - ", "- > ", "> > "];
string Wrap(string md, string prefix)
{
    var cont = prefix.Replace("- ", "  ").Replace("1. ", "   ");
    return prefix + md.Replace("\n", "\n" + cont);
}
foreach (var r in rows)
{
    foreach (var p in prefixes)
    {
        Add("body-blocks", Wrap("a|b\n--|--\n" + r + "\ntail|end", p));
        Add("body-end", Wrap("a|b\n--|--\nx|y", p) + "\n" + r + "\ntail|end");
    }
    foreach (var h in new[] { "a", "a|b", "|a|b|" })
        Add("multiline-paragraph", r + "\n" + h + "\n--|--\nx|y");
}
string[] contents = ["a|b", @"a\|b", "&#124;", "&vert;", "`a|b`", "`a\\|b`", "[a|b](url)", "[a\\|b](url)", "[a](u\\|v)", "[a](u \"x\\|y\")", "[a\\|b]", "![a\\|b](u)", "<https://a/\\|>", "<i title='a\\|b'>", "<i title='a|b'>", "**a|b**", "[a]", "[]", "\u00a0x\u00a0", "a\tb", "a\\", "[a]: /url", "` x `", "` `", "`a", "b`", "[a", "b](u)"];
foreach (var c in contents) foreach (var other in contents)
{
    Add("inline", "a|b\n--|--\n|" + c + "|" + other + "|\n\n[a]: /url\n[a|b]: /target");
    Add("inline-header", "|" + c + "|" + other + "|\n--|--\nx|y");
}
for (int n = 0; n <= 8; n++) foreach (var context in new[] { "{0}", "`{0}`", "**{0}**", "[x]({0})", "<i x='{0}'>", "<https://a/{0}>" })
{
    var c = string.Format(context, "a" + new string('\\', n) + "|b");
    Add("backslash", "|a|b|\n|-|-|\n|" + c + "|x|");
}
for (int hi = 0; hi < 6; hi++) for (int si = 0; si < 6; si++) for (int bi = 0; bi < 6; bi++)
    Add("indent", new string(' ', hi) + "a|b\n" + new string(' ', si) + "--|--\n" + new string(' ', bi) + "x|y");
foreach (var r in new[] { "--|--", "---|", "x|y", "---", "x", "", "|", "||", "- | -" })
    foreach (var s in separators)
        Add("retry", "a|b|c\n" + r + "\na|b\n" + s + "\nx|y");
foreach (var space in new[] { "", " ", "\t", "\v", "\f", "\u00a0", "\u2003" })
foreach (var marker in new[] { "-", ":-", "-:", ":-:", "", ":", "- -", ": -", "- :" })
foreach (var prefix in new[] { "", "|", " " })
foreach (var suffix in new[] { "", "|", " " })
{
    Add("separator-whitespace", "a\n" + prefix + space + marker + space + suffix + "\nx");
    Add("cell-whitespace", prefix + space + "x" + space + suffix + "\n|---|\n" + prefix + space + "y" + space + suffix);
}
foreach (var p in prefixes)
foreach (var r in rows)
{
    Add("body-after-row", Wrap("a|b\n--|--\nx|y\n" + r + "\ntail|end", p));
    Add("header-only-list", Wrap("intro\na|b\n--|--\n" + r, p));
}
foreach (var content in contents)
{
    Add("inline-focused", "a|b\n--|--\n|" + content + "|x|\n\n[a]: /url\n[a|b]: /target");
    Add("paragraph-escape", content + "\na|b\n--|--\nx|y\n\n[a]: /url\n[a|b]: /target");
}
// Reproducible bounded combinations exercise parser recovery and container boundaries.
var random = new Random(955);
for (int i = 0; i < 4000; i++)
{
    string Pick(string[] values) => values[random.Next(values.Length)];
    Add("random", Wrap(Pick(headers) + "\n" + Pick(separators) + "\n" + Pick(rows) + "\n" + Pick(rows) + "\n" + Pick(rows), Pick(prefixes)));
}
var cachePath = Path.Combine(root, $"oracle-{revision}.json");
var cache = File.Exists(cachePath) ? JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(cachePath))! : new();
var missing = cases.Keys.Where(c => !cache.ContainsKey(c)).ToArray();
Console.WriteLine($"Cases: {cases.Count}; uncached oracle runs: {missing.Length}");
Parallel.ForEach(missing, new ParallelOptions { MaxDegreeOfParallelism = 4 }, c => { var html = Oracle(c); lock(cache) cache[c] = html; });
File.WriteAllText(cachePath, JsonSerializer.Serialize(cache));
File.WriteAllText(Path.Combine(root, "corpus.json"), JsonSerializer.Serialize(cases.Select(c => new { group = c.Value, input = c.Key, expected = Normalize(cache[c.Key]) }), new JsonSerializerOptions { WriteIndented = true }));
var mismatches = new List<object>();
var counts = new Dictionary<string, int>();
int htmlDifferences = 0, triviaTableDifferences = 0, outsideTableDifferences = 0, coreOnlyDifferences = 0;
foreach (var (input, group) in cases)
{
    var expected = Normalize(cache[input]);
    string actual;
    try { actual = Normalize(Markdown.ToHtml(input, pipeline)); } catch (Exception ex) { actual = ex.ToString(); }
    var tracked = Normalize(Markdown.ToHtml(input, trivia));
    if (expected != actual)
    {
        htmlDifferences++;
        if (Tables(expected) == Tables(actual) && expected.Contains("<table>")) outsideTableDifferences++;
        if (!expected.Contains("<table>") && actual == Normalize(Markdown.ToHtml(input))) coreOnlyDifferences++;
    }
    if (Tables(tracked) != Tables(expected)) triviaTableDifferences++;
    if (expected != actual || tracked != actual)
    {
        counts[group] = counts.GetValueOrDefault(group) + 1;
        mismatches.Add(new { group, input, expected, actual, tracked = tracked == actual ? null : tracked });
    }
}
File.WriteAllText(Path.Combine(root, "mismatches.json"), JsonSerializer.Serialize(mismatches, new JsonSerializerOptions { WriteIndented = true }));
Console.WriteLine($"Mismatches: {mismatches.Count}");
foreach (var pair in counts) Console.WriteLine($"{pair.Key}: {pair.Value}");
string Tables(string html) => string.Join("\n", Regex.Matches(html, @"(?s)<table>.*?</table>").Select(m => m.Value));
var tableDifferences = cases.Where(c => Tables(cache[c.Key]) != Tables(Normalize(Markdown.ToHtml(c.Key, pipeline)))).Select(c => new { group = c.Value, input = c.Key, expected = cache[c.Key], actual = Normalize(Markdown.ToHtml(c.Key, pipeline)) }).ToArray();
File.WriteAllText(Path.Combine(root, "table-mismatches.json"), JsonSerializer.Serialize(tableDifferences, new JsonSerializerOptions { WriteIndented = true }));
Console.WriteLine($"Table subtree differences: {tableDifferences.Length}");
var normalizationDifferences = cases.Where(c => c.Value is "backslash" or "inline-focused" or "inline").Select(c => new { input = c.Key, expected = Normalize(Markdown.ToHtml(c.Key, pipeline)), normalized = Markdown.Normalize(c.Key, pipeline: pipeline) }).Where(c => c.expected != Normalize(Markdown.ToHtml(c.normalized, pipeline))).ToArray();
File.WriteAllText(Path.Combine(root, "normalization-mismatches.json"), JsonSerializer.Serialize(normalizationDifferences, new JsonSerializerOptions { WriteIndented = true }));
Console.WriteLine($"Inline normalization differences: {normalizationDifferences.Length}");
Console.WriteLine($"Full HTML differences: {htmlDifferences}; of these, {outsideTableDifferences} have identical table subtrees but different surrounding HTML.");
Console.WriteLine($"Trivia-mode table subtree differences: {triviaTableDifferences}");
Console.WriteLine($"Differences without tables reproduced with Markdig's default pipeline: {coreOnlyDifferences}");
if (args.Length == 4)
{
    // A fixed selection by category, never selected by whether Markdig passes.
    // Table-only cases retain native expectations even for known core differences.
    var snapshots = cases.Where(c => c.Value is "header-only" or "backslash" or "inline-focused"
        or "indent" or "cell-whitespace" or "retry" or "body-blocks" or "body-after-row"
        or "header-only-list" or "paragraph-escape" or "separator-whitespace")
        .Select(c => new
        {
            group = c.Value, input = c.Key, expected = Normalize(cache[c.Key]),
            tableOnly = c.Value is "separator-whitespace" or "body-after-row" or "header-only-list"
                || (c.Value == "body-blocks" && c.Key.Split('\n')[2].TrimStart(' ', '>') is "---" or "- - -" or "***" or "___")
        });
    File.WriteAllText(args[3], JsonSerializer.Serialize(snapshots, new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    }));
}
return tableDifferences.Length == 0 && triviaTableDifferences == 0 && normalizationDifferences.Length == 0 ? 0 : 1;
