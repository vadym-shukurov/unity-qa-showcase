namespace UnityMobileQA.Tests.TestUtils
{
    // PRESENTATION: "One place for magic numbers. Tests stay consistent."
    // WHY: Change reward value once, all tests use it. No scattered literals.
    // SCALE: Add constants as test scenarios grow.
    // RELEASE: Deterministic — same values across runs.
    /// <summary>
    /// Centralized test constants for consistency and maintainability.
    /// </summary>
    public static class TestConstants
    {
        public const int DefaultCurrencyReward = 50;
        public const int SampleScore = 100;
        public const int SampleLevel = 3;
        public const float SampleMusicVolume = 0.5f;
        public const float SampleSfxVolume = 0.8f;

        /// <summary>Max seconds for first-scene load (Splash). Mobile performance target.</summary>
        public const float LoadTimeMaxSeconds = 5f;

        /// <summary>Max seconds per scene load in load tests. Individual scene target.</summary>
        public const float SceneLoadMaxSeconds = 3f;

        /// <summary>Max seconds for full flow (menu → gameplay → result → menu). Load/performance target.</summary>
        public const float FullFlowLoadMaxSeconds = 15f;

        /// <summary>Max memory growth (MB) after repeated scene loads. Detects leaks.</summary>
        public const int MaxMemoryGrowthMb = 64;

        /// <summary>Max Mono heap growth (MB) after GC. Detects managed leaks.</summary>
        public const int MaxMonoHeapGrowthMb = 32;
    }
}
