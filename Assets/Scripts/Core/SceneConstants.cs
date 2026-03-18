namespace UnityMobileQA.Core
{
    /// <summary>
    /// Centralized scene names. Use these instead of string literals.
    /// Benefits: refactor-safe, no typos, single source of truth for tests.
    /// </summary>
    /// <remarks>
    /// Scene names must match Build Settings (File → Build Settings).
    /// AllScenes used by load/performance tests.
    /// </remarks>
    public static class SceneConstants
    {
        public const string Splash = "Splash";
        public const string MainMenu = "MainMenu";
        public const string Settings = "Settings";
        public const string Gameplay = "Gameplay";
        public const string Result = "Result";

        /// <summary>All scenes in app flow order. Use for iteration (e.g. load tests).</summary>
        public static readonly string[] AllScenes = { Splash, MainMenu, Settings, Gameplay, Result };
    }
}
