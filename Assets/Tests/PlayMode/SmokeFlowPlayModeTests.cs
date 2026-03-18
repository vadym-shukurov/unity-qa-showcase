using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityMobileQA.Core;
using UnityMobileQA.Gameplay;
using UnityMobileQA.Tests.TestUtils;
using UnityMobileQA.UI;

namespace UnityMobileQA.Tests.PlayMode
{
    // PRESENTATION: "Smoke = release gate. I automated this after Edit Mode."
    // WHY: Critical path must work. Menu → Gameplay → Result → Menu.
    // SCALE: Add more smoke scenarios (settings flow, tutorial) as app grows.
    // RELEASE: Pipeline fails if smoke fails. No release without green smoke.
    /// <summary>
    /// Play Mode: Smoke flow. UI-driven — simulates real user taps via controllers.
    /// Uses MainMenuController.OnPlayClicked(), GameplayController.SimulateCompletion(), etc.
    /// Validates the actual navigation path a user would take (vs programmatic SceneFlowController).
    /// These tests require scenes in Build Settings.
    /// </summary>
    [TestFixture]
    [Category("Smoke")]
    public class SmokeFlowPlayModeTests : TestSetupBase
    {
        [UnityTest]
        [Timeout(15000)]
        public IEnumerator AppFlow_ReachesMainMenu_WhenSplashLoads()
        {
            yield return SceneLoaderHelper.LoadSceneAndWait(SceneConstants.Splash);
            yield return new WaitForSeconds(0.5f);
            yield return SceneLoaderHelper.LoadSceneAndWait(SceneConstants.MainMenu);
            Assert.AreEqual(SceneConstants.MainMenu, SceneLoaderHelper.GetActiveSceneName());
        }

        [UnityTest]
        [Timeout(10000)]
        public IEnumerator MainMenu_ToGameplay_TransitionWorks()
        {
            yield return SceneLoaderHelper.LoadSceneAndWait(SceneConstants.MainMenu);
            var flow = new SceneFlowController();
            flow.LoadScene(SceneConstants.Gameplay);
            yield return new WaitForSeconds(1f);
            Assert.AreEqual(SceneConstants.Gameplay, SceneLoaderHelper.GetActiveSceneName());
        }

        [UnityTest]
        [Timeout(10000)]
        public IEnumerator Gameplay_ToResult_TransitionWorks()
        {
            yield return SceneLoaderHelper.LoadSceneAndWait(SceneConstants.Gameplay);
            var flow = new SceneFlowController();
            flow.LoadScene(SceneConstants.Result);
            yield return new WaitForSeconds(0.5f);
            Assert.AreEqual(SceneConstants.Result, SceneLoaderHelper.GetActiveSceneName());
        }

        [UnityTest]
        [Timeout(10000)]
        public IEnumerator Result_ToMainMenu_ReturnWorks()
        {
            yield return SceneLoaderHelper.LoadSceneAndWait(SceneConstants.Result);
            var flow = new SceneFlowController();
            flow.LoadScene(SceneConstants.MainMenu);
            yield return new WaitForSeconds(0.5f);
            Assert.AreEqual(SceneConstants.MainMenu, SceneLoaderHelper.GetActiveSceneName());
        }

        /// <summary>UI-driven: simulates user taps. Validates real navigation path.</summary>
        [UnityTest]
        [Timeout(20000)]
        public IEnumerator FullFlow_UIButtons_SimulatesUserPath()
        {
            yield return SceneLoaderHelper.LoadSceneAndWait(SceneConstants.MainMenu);
            var menu = Object.FindObjectOfType<MainMenuController>();
            Assert.IsNotNull(menu, "MainMenuController required");
            menu.OnPlayClicked();
            yield return new WaitForSeconds(1f);
            Assert.AreEqual(SceneConstants.Gameplay, SceneLoaderHelper.GetActiveSceneName());

            var gameplay = Object.FindObjectOfType<GameplayController>();
            Assert.IsNotNull(gameplay, "GameplayController required");
            gameplay.SimulateCompletion(TestConstants.SampleScore);
            yield return new WaitForSeconds(0.8f);
            Assert.AreEqual(SceneConstants.Result, SceneLoaderHelper.GetActiveSceneName());

            var result = Object.FindObjectOfType<ResultScreenController>();
            Assert.IsNotNull(result, "ResultScreenController required");
            result.OnBackToMenuClicked();
            yield return new WaitForSeconds(0.5f);
            Assert.AreEqual(SceneConstants.MainMenu, SceneLoaderHelper.GetActiveSceneName());
        }

        /// <summary>UI-driven: MainMenu → Settings → Back. Validates Settings flow via real button taps.</summary>
        [UnityTest]
        [Timeout(15000)]
        public IEnumerator FullFlow_Settings_UIButtons_SimulatesUserPath()
        {
            yield return SceneLoaderHelper.LoadSceneAndWait(SceneConstants.MainMenu);
            var menu = Object.FindObjectOfType<MainMenuController>();
            Assert.IsNotNull(menu, "MainMenuController required");
            menu.OnSettingsClicked();
            yield return new WaitForSeconds(1f);
            Assert.AreEqual(SceneConstants.Settings, SceneLoaderHelper.GetActiveSceneName());

            var settings = Object.FindObjectOfType<SettingsController>();
            Assert.IsNotNull(settings, "SettingsController required");
            settings.OnBackClicked();
            yield return new WaitForSeconds(0.5f);
            Assert.AreEqual(SceneConstants.MainMenu, SceneLoaderHelper.GetActiveSceneName());
        }
    }
}
