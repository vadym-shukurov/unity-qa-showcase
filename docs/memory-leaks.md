# Memory Leak Detection

**PRESENTATION:** "Memory leaks = crashes on low-end devices. We catch them before release."
**RELEASE CONFIDENCE:** Memory tests gate the pipeline. Telemetry trends over time.

---

## Tests

| Test | What it does | Threshold |
|------|---------------|-----------|
| **Memory_TotalAllocated_WithinReasonableBounds** | Total allocated memory at idle | ≤ 512 MB |
| **RepeatedSceneLoads_NoExcessiveMemoryGrowth** | 5× (MainMenu → Gameplay → Result) scene loads | Growth ≤ 64 MB |
| **RepeatedGameplaySessions_NoExcessiveMemoryGrowth** | 5× full gameplay sessions (score, complete, return) | Growth ≤ 64 MB |
| **MonoHeap_AfterGC_NoExcessiveGrowth** | 5× full flow, then GC; measure Mono heap | Growth ≤ 32 MB |

## How it works

- **Profiler APIs** (Editor/dev builds only): `GetTotalAllocatedMemoryLong()`, `GetMonoUsedSizeLong()`.
- **GC before measure**: `MonoHeap_AfterGC` calls `GC.Collect()` + `GC.WaitForPendingFinalizers()` to isolate managed leaks.
- **Telemetry**: Memory growth is emitted to `reports/telemetry.json` for trend analysis.

## Alerts

- **CI**: Quality gate fails on any test failure. Memory test failures trigger an explicit `::error::Memory leak detected` message.
- **Dashboard**: `reports/telemetry.json` includes `memory_growth_*` metrics; `generate-metrics.js` merges into the dashboard.

## Constants

- `TestConstants.MaxMemoryGrowthMb` = 64
- `TestConstants.MaxMonoHeapGrowthMb` = 32

Adjust for your target devices and app size.
