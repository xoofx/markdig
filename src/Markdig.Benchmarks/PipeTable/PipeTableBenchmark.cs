// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using Markdig;

namespace Testamina.Markdig.Benchmarks.PipeTable;

/// <summary>
/// Benchmark for pipe table parsing performance, especially for large tables.
/// Tests the performance of PipeTableParser with varying table sizes.
/// </summary>
[MemoryDiagnoser]
[GcServer(true)] // Use server GC to get more comprehensive GC stats
public class PipeTableBenchmark
{
    private string _100Rows = null!;
    private string _500Rows = null!;
    private string _1000Rows = null!;
    private string _1500Rows = null!;
    private string _5000Rows = null!;
    private string _10000Rows = null!;
    private MarkdownPipeline _pipeline = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Pipeline with pipe tables enabled (part of advanced extensions)
        _pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        // Generate tables of various sizes
        // Note: Before optimization, 5000+ rows hit depth limit due to nested tree structure.
        // After optimization, these should work.
        _100Rows = PipeTableGenerator.Generate(rows: 100, columns: 5);
        _500Rows = PipeTableGenerator.Generate(rows: 500, columns: 5);
        _1000Rows = PipeTableGenerator.Generate(rows: 1000, columns: 5);
        _1500Rows = PipeTableGenerator.Generate(rows: 1500, columns: 5);
        _5000Rows = PipeTableGenerator.Generate(rows: 5000, columns: 5);
        _10000Rows = PipeTableGenerator.Generate(rows: 10000, columns: 5);
    }

    [Benchmark(Description = "PipeTable 100 rows x 5 cols")]
    public string Parse100Rows()
    {
        return Markdown.ToHtml(_100Rows, _pipeline);
    }

    [Benchmark(Description = "PipeTable 500 rows x 5 cols")]
    public string Parse500Rows()
    {
        return Markdown.ToHtml(_500Rows, _pipeline);
    }

    [Benchmark(Description = "PipeTable 1000 rows x 5 cols")]
    public string Parse1000Rows()
    {
        return Markdown.ToHtml(_1000Rows, _pipeline);
    }

    [Benchmark(Description = "PipeTable 1500 rows x 5 cols")]
    public string Parse1500Rows()
    {
        return Markdown.ToHtml(_1500Rows, _pipeline);
    }

    [Benchmark(Description = "PipeTable 5000 rows x 5 cols")]
    public string Parse5000Rows()
    {
        return Markdown.ToHtml(_5000Rows, _pipeline);
    }

    [Benchmark(Description = "PipeTable 10000 rows x 5 cols")]
    public string Parse10000Rows()
    {
        return Markdown.ToHtml(_10000Rows, _pipeline);
    }
}
