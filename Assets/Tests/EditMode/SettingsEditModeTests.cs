using NUnit.Framework;
using UnityMobileQA.Services;
using UnityMobileQA.Tests.TestUtils;

namespace UnityMobileQA.Tests.EditMode
{
    // PRESENTATION: "Third — settings persistence. UX risk if lost."
    // WHY: Users expect settings to stick. Low complexity, high value.
    // SCALE: Add platform-specific settings (notifications, language) when needed.
    // RELEASE: Prevents "settings reset" support tickets.
    /// <summary>
    /// Edit Mode: Settings persistence. Critical for user experience.
    /// </summary>
    [TestFixture]
    [Category("DataIntegrity")]
    public class SettingsEditModeTests : TestSetupBase
    {
        [Test]
        public void SaveAndLoad_PersistsVolumeAndNotifications()
        {
            var service = FakeDataFactory.CreateSettingsService(
                TestConstants.SampleMusicVolume, TestConstants.SampleSfxVolume, false);
            service.Save();

            var loaded = new SettingsService();
            loaded.Load();

            Assert.AreEqual(TestConstants.SampleMusicVolume, loaded.MusicVolume, 0.001f);
            Assert.AreEqual(TestConstants.SampleSfxVolume, loaded.SfxVolume, 0.001f);
            Assert.IsFalse(loaded.NotificationsEnabled);
        }

        [Test]
        public void ChangeSettings_SaveLoad_RestoresCorrectly()
        {
            var service = new SettingsService();
            service.MusicVolume = 0.2f;
            service.SfxVolume = 0.9f;
            service.NotificationsEnabled = false;
            service.Save();

            var loaded = new SettingsService();
            loaded.Load();

            Assert.AreEqual(0.2f, loaded.MusicVolume, 0.001f);
            Assert.AreEqual(0.9f, loaded.SfxVolume, 0.001f);
            Assert.IsFalse(loaded.NotificationsEnabled);
        }

        [TestCase(0f, 1f, true)]
        [TestCase(0.5f, 0.5f, false)]
        [TestCase(1f, 0f, true)]
        public void SaveAndLoad_Parameterized_PersistsCorrectly(float music, float sfx, bool notifications)
        {
            var service = FakeDataFactory.CreateSettingsService(music, sfx, notifications);
            service.Save();
            var loaded = new SettingsService();
            loaded.Load();
            Assert.AreEqual(music, loaded.MusicVolume, 0.01f);
            Assert.AreEqual(sfx, loaded.SfxVolume, 0.01f);
            Assert.AreEqual(notifications, loaded.NotificationsEnabled);
        }
    }
}
