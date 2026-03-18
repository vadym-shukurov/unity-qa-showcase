using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityMobileQA.Core;
using UnityMobileQA.Gameplay;
using UnityMobileQA.Tests.TestUtils;

namespace UnityMobileQA.Tests.PlayMode
{
    // PRESENTATION: "Game-specific — FPS and memory. Critical for mobile."
    // WHY: Low FPS = bad UX. Memory leaks = crashes on low-end devices.
    // SCALE: Add more perf tests (load time, GC spikes). Integrate with profiling.
    // RELEASE: Catches performance regressions before device testing.
    /// <summary>
    /// Play Mode: Game performance — FPS and memory. Game-specific validation.
    /// </summary>
    [TestFixture]
    [Category("Performance")]
    public class GamePerformancePlayModeTests : TestSetupBase
    {
        private const int FpsSampleFrames = 30;
        private const float MinAcceptableFps = 15f;
        private const long MaxMemoryMb = 512;

        [UnityTest]
        [Timeout(10000)]
        public IEnumerator FPS_StaysAboveMinimum_OverSampleFrames()
        {
            float totalFps = 0f;
            int frames = 0;

            while (frames < FpsSampleFrames)
            {
                yield return null;
                if (Time.deltaTime > 0)
                {
                    totalFps += 1f / Time.deltaTime;
                    frames++;
                }
            }

            float avgFps = totalFps / frames;
            TestTelemetry.RecordFps("fps_idle", avgFps);
            Assert.GreaterOrEqual(avgFps, MinAcceptableFps,
                $"Average FPS {avgFps:F1} below minimum {MinAcceptableFps}");
        }

        [UnityTest]
        [Timeout(10000)]
        public IEnumerator FirstScene_LoadsWithinMaxSeconds()
        {
            var start = Time.realtimeSinceStartup;

            yield return SceneLoaderHelper.LoadSceneAndWait(SceneConstants.Splash, TestConstants.LoadTimeMaxSeconds + 2f);

            var elapsed = Time.realtimeSinceStartup - start;
            TestTelemetry.RecordDuration("load_splash", elapsed);
            Assert.LessOrEqual(elapsed, TestConstants.LoadTimeMaxSeconds,
                $"First scene (Splash) took {elapsed:F2}s, max allowed {TestConstants.LoadTimeMaxSeconds}s");
        }

        [UnityTest]
        [Timeout(5000)]
        public IEnumerator Memory_TotalAllocated_WithinReasonableBounds()
        {
            yield return null;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            long allocatedBytes = Profiler.GetTotalAllocatedMemoryLong();
            var mb = allocatedBytes / (1024.0 * 1024.0);
            TestTelemetry.RecordMemoryMb("memory_total_allocated", mb);
            long maxBytes = MaxMemoryMb * 1024 * 1024;
            Assert.LessOrEqual(allocatedBytes, maxBytes,
                $"Allocated memory {allocatedBytes / 1024 / 1024}MB exceeds {MaxMemoryMb}MB");
#else
            Assert.Ignore("Profiler not available in release builds");
#endif
        }

        [UnityTest]
        [Timeout(30000)]
        public IEnumerator AllScenes_LoadWithinMaxSeconds()
        {
            foreach (var sceneName in SceneConstants.AllScenes)
            {
                var start = Time.realtimeSinceStartup;
                yield return SceneLoaderHelper.LoadSceneAndWait(sceneName, TestConstants.SceneLoadMaxSeconds + 2f);
                var elapsed = Time.realtimeSinceStartup - start;
                TestTelemetry.RecordDuration($"load_{sceneName.ToLowerInvariant()}", elapsed);
                Assert.LessOrEqual(elapsed, TestConstants.SceneLoadMaxSeconds,
                    $"Scene {sceneName} took {elapsed:F2}s, max {TestConstants.SceneLoadMaxSeconds}s");
            }
        }

        [UnityTest]
        [Timeout(25000)]
        public IEnumerator FullFlow_LoadTime_Acceptable()
        {
            var start = Time.realtimeSinceStartup;

            yield return SceneLoaderHelper.LoadSceneAndWait(SceneConstants.MainMenu);
            yield return SceneLoaderHelper.LoadSceneAndWait(SceneConstants.Gameplay);
            yield return SceneLoaderHelper.LoadSceneAndWait(SceneConstants.Result);
            yield return SceneLoaderHelper.LoadSceneAndWait(SceneConstants.MainMenu);

            var elapsed = Time.realtimeSinceStartup - start;
            TestTelemetry.RecordDuration("load_full_flow", elapsed);
            Assert.LessOrEqual(elapsed, TestConstants.FullFlowLoadMaxSeconds,
                $"Full flow load took {elapsed:F2}s, max {TestConstants.FullFlowLoadMaxSeconds}s");
        }

        [UnityTest]
        [Timeout(45000)]
        [Retry(2)]
        public IEnumerator RepeatedSceneLoads_NoExcessiveMemoryGrowth()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            const int loadCycles = 5;
            long initialBytes = 0;

            for (int i = 0; i < loadCycles; i++)
            {
                yield return SceneLoaderHelper.LoadSceneAndWait(SceneConstants.MainMenu);
                yield return SceneLoaderHelper.LoadSceneAndWait(SceneConstants.Gameplay);
                yield return SceneLoaderHelper.LoadSceneAndWait(SceneConstants.Result);
                yield return null;

                var current = Profiler.GetTotalAllocatedMemoryLong();
                if (i == 0) initialBytes = current;
                var growthMb = (current - initialBytes) / (1024.0 * 1024.0);
                TestTelemetry.RecordMemoryMb("memory_growth_scene_loads", growthMb);
                Assert.LessOrEqual(growthMb, TestConstants.MaxMemoryGrowthMb,
                    $"Memory grew {growthMb:F1}MB after {i + 1} cycles (max {TestConstants.MaxMemoryGrowthMb}MB)");
            }
#else
            Assert.Ignore("Profiler not available in release builds");
#endif
        }

        [UnityTest]
        [Timeout(15000)]
        public IEnumerator FPS_DuringSceneLoad_StaysAboveMinimum()
        {
            float totalFps = 0f;
            int frames = 0;
            var loadOp = SceneManager.LoadSceneAsync(SceneConstants.Gameplay);

            while (loadOp != null && !loadOp.isDone && frames < 60)
            {
                yield return null;
                if (Time.deltaTime > 0)
                {
                    totalFps += 1f / Time.deltaTime;
                    frames++;
                }
            }

            if (frames > 0)
            {
                float avgFps = totalFps / frames;
                TestTelemetry.RecordFps("fps_during_load", avgFps);
                Assert.GreaterOrEqual(avgFps, MinAcceptableFps,
                    $"FPS during scene load {avgFps:F1} below minimum {MinAcceptableFps}");
            }
        }

        [UnityTest]
        [Timeout(60000)]
        [Retry(2)]
        public IEnumerator MonoHeap_AfterGC_NoExcessiveGrowth()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            yield return SceneLoaderHelper.LoadSceneAndWait(SceneConstants.MainMenu);
            yield return null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            yield return null;
            long baselineMono = Profiler.GetMonoUsedSizeLong();

            const int cycles = 5;
            for (int i = 0; i < cycles; i++)
            {
                yield return SceneLoaderHelper.LoadSceneAndWait(SceneConstants.Gameplay);
                yield return SceneLoaderHelper.LoadSceneAndWait(SceneConstants.Result);
                yield return SceneLoaderHelper.LoadSceneAndWait(SceneConstants.MainMenu);
                yield return null;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            yield return null;
            long afterMono = Profiler.GetMonoUsedSizeLong();
            var growthMb = (afterMono - baselineMono) / (1024.0 * 1024.0);
            TestTelemetry.RecordMemoryMb("memory_mono_growth_after_gc", growthMb);
            Assert.LessOrEqual(growthMb, TestConstants.MaxMonoHeapGrowthMb,
                $"Mono heap grew {growthMb:F1}MB after GC (max {TestConstants.MaxMonoHeapGrowthMb}MB)");
#else
            Assert.Ignore("Profiler not available in release builds");
#endif
        }

        [UnityTest]
        [Timeout(60000)]
        [Retry(2)]
        public IEnumerator RepeatedGameplaySessions_NoExcessiveMemoryGrowth()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            yield return SceneLoaderHelper.LoadSceneAndWait(SceneConstants.MainMenu);
            yield return null;
            long initialBytes = Profiler.GetTotalAllocatedMemoryLong();

            const int sessions = 5;
            for (int i = 0; i < sessions; i++)
            {
                yield return SceneLoaderHelper.LoadSceneAndWait(SceneConstants.Gameplay);
                yield return new WaitForSeconds(0.3f);

                var controller = UnityEngine.Object.FindObjectOfType<GameplayController>();
                if (controller != null)
                    controller.SimulateCompletion(TestConstants.SampleScore);

                yield return new WaitForSeconds(0.5f);
                yield return SceneLoaderHelper.LoadSceneAndWait(SceneConstants.MainMenu);
                yield return null;

                var current = Profiler.GetTotalAllocatedMemoryLong();
                var growthMb = (current - initialBytes) / (1024 * 1024);
                TestTelemetry.RecordMemoryMb($"memory_growth_after_session_{i + 1}", growthMb);
                Assert.LessOrEqual(growthMb, TestConstants.MaxMemoryGrowthMb,
                    $"Memory grew {growthMb}MB after {i + 1} gameplay sessions (max {TestConstants.MaxMemoryGrowthMb}MB)");
            }
#else
            Assert.Ignore("Profiler not available in release builds");
#endif
        }
    }
}
