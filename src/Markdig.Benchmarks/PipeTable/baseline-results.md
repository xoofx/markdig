# PipeTable Baseline Results (Pre-Optimization)

## Summary Table (Server GC)

| Method                          | Mean         | Error       | StdDev      | Gen0   | Gen1   | Allocated  |
|-------------------------------- |-------------:|------------:|------------:|-------:|-------:|-----------:|
| 'PipeTable 100 rows x 5 cols'   |     542.0 us |     2.25 us |     1.88 us | 2.9297 | 0.9766 |  367.38 KB |
| 'PipeTable 500 rows x 5 cols'   |  23,018.4 us |   150.30 us |   133.24 us |      - |      - | 1818.08 KB |
| 'PipeTable 1000 rows x 5 cols'  |  89,418.0 us |   507.04 us |   474.28 us |      - |      - |  3702.7 KB |
| 'PipeTable 1500 rows x 5 cols'  | 201,593.3 us | 2,133.24 us | 1,995.44 us |      - |      - | 5660.16 KB |
| 'PipeTable 5000 rows x 5 cols'  |           NA |          NA |          NA |     NA |     NA |         NA |
| 'PipeTable 10000 rows x 5 cols' |           NA |          NA |          NA |     NA |     NA |         NA |

## Scaling Analysis

The scaling is clearly super-linear (O(n²) or worse):
- 100 → 500 (5x rows): 542µs → 23ms = **42x slowdown**
- 500 → 1000 (2x rows): 23ms → 89ms = **3.9x slowdown**
- 1000 → 1500 (1.5x rows): 89ms → 202ms = **2.3x slowdown**

## Issues

- 5000 and 10000 row benchmarks **FAIL** due to depth limit exceeded
- Deep nesting causes O(n²) backward traversal for cell boundary detection
- Memory allocation scales linearly but is excessive (~3.7 KB per row)
- Gen0/Gen1 collections drop to 0 for larger tables (server GC handles in batches)

## Environment

- BenchmarkDotNet v0.14.0
- macOS 26.2 (Darwin 25.2.0)
- Apple M2 Pro, 12 cores
- .NET 8.0.8, Arm64 RyuJIT AdvSIMD
- Server GC enabled
