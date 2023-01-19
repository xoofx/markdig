// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Text;
using System.Text.RegularExpressions;

namespace Testamina.Markdig.Benchmarks;

public class TextRegexHelper
{
    private readonly Dictionary<string, string> replacers;
    private readonly Regex regex;

    public TextRegexHelper(Dictionary<string, string> replacers)
    {
        this.replacers = replacers;
        var builder = new StringBuilder();

        // (?<1>:value:?)|(?<1>:noo:?)
        foreach (var replace in replacers)
        {
            var matchStr = Regex.Escape(replace.Key);
            if (builder.Length > 0)
            {
                builder.Append('|');
            }
            builder.Append("(?<1>").Append(matchStr).Append("?)");
        }

        regex = new Regex(builder.ToString());
    }

    public bool TryMatch(string text, int offset, out string matchText, out string replaceText)
    {
        replaceText = null;
        matchText = null;
        var result = regex.Match(text, offset);
        if (!result.Success)
        {
            return false;
        }

        matchText = result.Groups[1].Value;
        replaceText = replacers[matchText];
        return true;
    }
}

/*
public class TestMatchPerf
{
    private readonly TextMatchHelper matcher;

    public TestMatchPerf()
    {
        var replacers = new Dictionary<string, string>();
        for (int i = 0; i < 1000; i++)
        {
            replacers.Add($":z{i}:", i.ToString());
        }
        replacers.Add(":abc:", "yes");

        matcher = new TextMatchHelper(new HashSet<string>(replacers.Keys));
    }

    [Benchmark]
    public void TestMatch()
    {

        for (int i = 0; i < 1000; i++)
        {
            string matchText;
            //var text = ":z150: this is a long string";
            var text = ":z1:";
            matcher.TryMatch(text, 0, text.Length, out matchText);
        }
    }
}
*/