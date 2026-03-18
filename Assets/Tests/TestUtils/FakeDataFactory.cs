using UnityMobileQA.Services;

namespace UnityMobileQA.Tests.TestUtils
{
    // PRESENTATION: "Maintainability — one place for test data. Reduces duplication."
    // WHY: Tests stay readable. Change factory once, all tests benefit.
    // SCALE: Add more factory methods as services grow. Keep tests DRY.
    // RELEASE: Consistent test setup = deterministic, fewer flaky tests.
    /// <summary>
    /// Factory for creating test data and mock services with predefined states.
    /// </summary>
    public static class FakeDataFactory
    {
        public static IPlayerProgressService CreateProgressService(int level = 1, int score = 0, bool tutorialComplete = false)
        {
            var service = new PlayerProgressService();
            service.Reset();
            service.SetLevel(level);
            service.AddScore(score);
            service.SetTutorialComplete(tutorialComplete);
            service.Save();
            return service;
        }

        public static ICurrencyService CreateCurrencyService(int initialBalance = 0)
        {
            var service = new CurrencyService();
            service.Reset();
            if (initialBalance > 0)
                service.Add(initialBalance);
            return service;
        }

        public static ISettingsService CreateSettingsService(float music = 1f, float sfx = 1f, bool notifications = true)
        {
            var service = new SettingsService();
            service.MusicVolume = music;
            service.SfxVolume = sfx;
            service.NotificationsEnabled = notifications;
            service.Save();
            return service;
        }
    }
}
