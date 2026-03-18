using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityMobileQA.Core;
using UnityMobileQA.Services;
using UnityMobileQA.Tests.TestUtils;

namespace UnityMobileQA.Tests.PlayMode
{
    // PRESENTATION: "Regression — full flow. Catches integration breaks."
    // WHY: Edit Mode tests logic; Play Mode tests flow. Both needed.
    // SCALE: Add Settings in/out, tutorial skip, etc. as features grow.
    // RELEASE: Ensures no regression in happy path.
    /// <summary>
    /// Play Mode: Regression flow. API-driven — uses SceneFlowController.LoadScene() directly.
    /// Validates scene transitions and persistence without simulating UI taps.
    /// Complements SmokeFlowPlayModeTests (UI-driven); both cover the same path differently.
    /// </summary>
    [TestFixture]
    [Category("Regression")]
    public class RegressionFlowPlayModeTests : TestSetupBase
    {
        [UnityTest]
        [Timeout(15000)]
        public IEnumerator FullFlow_MenuToGameplayToResultToMenu_CompletesWithoutError()
        {
            yield return SceneLoaderHelper.LoadSceneAndWait(SceneConstants.MainMenu);
            Assert.AreEqual(SceneConstants.MainMenu, SceneLoaderHelper.GetActiveSceneName());

            var flow = new SceneFlowController();
            flow.LoadScene(SceneConstants.Gameplay);
            yield return new WaitForSeconds(0.5f);
            Assert.AreEqual(SceneConstants.Gameplay, SceneLoaderHelper.GetActiveSceneName());

            flow.LoadScene(SceneConstants.Result);
            yield return new WaitForSeconds(0.5f);
            Assert.AreEqual(SceneConstants.Result, SceneLoaderHelper.GetActiveSceneName());

            flow.LoadScene(SceneConstants.MainMenu);
            yield return new WaitForSeconds(0.5f);
            Assert.AreEqual(SceneConstants.MainMenu, SceneLoaderHelper.GetActiveSceneName());
        }

        [UnityTest]
        [Timeout(10000)]
        public IEnumerator MainMenu_ToSettings_AndBack_Works()
        {
            yield return SceneLoaderHelper.LoadSceneAndWait(SceneConstants.MainMenu);
            var flow = new SceneFlowController();

            flow.LoadScene(SceneConstants.Settings);
            yield return new WaitForSeconds(0.5f);
            Assert.AreEqual(SceneConstants.Settings, SceneLoaderHelper.GetActiveSceneName());

            flow.LoadScene(SceneConstants.MainMenu);
            yield return new WaitForSeconds(0.5f);
            Assert.AreEqual(SceneConstants.MainMenu, SceneLoaderHelper.GetActiveSceneName());
        }

        [UnityTest]
        [Timeout(10000)]
        public IEnumerator Settings_ChangeAndReturn_PersistsCorrectly()
        {
            yield return SceneLoaderHelper.LoadSceneAndWait(SceneConstants.MainMenu);
            var flow = new SceneFlowController();
            flow.LoadScene(SceneConstants.Settings);
            yield return new WaitForSeconds(0.5f);
            Assert.AreEqual(SceneConstants.Settings, SceneLoaderHelper.GetActiveSceneName());

            var settings = FakeDataFactory.CreateSettingsService(0.3f, 0.7f, false);
            settings.Save();

            flow.LoadScene(SceneConstants.MainMenu);
            yield return new WaitForSeconds(0.5f);
            var loaded = new SettingsService();
            loaded.Load();
            Assert.AreEqual(0.3f, loaded.MusicVolume, 0.01f);
            Assert.AreEqual(0.7f, loaded.SfxVolume, 0.01f);
            Assert.IsFalse(loaded.NotificationsEnabled);
        }
    }
}
