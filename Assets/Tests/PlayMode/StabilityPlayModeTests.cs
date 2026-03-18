using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityMobileQA.Core;
using UnityMobileQA.Services;
using UnityMobileQA.Tests.TestUtils;

namespace UnityMobileQA.Tests.PlayMode
{
    // PRESENTATION: "Stability — repeated save/load, scene reload. Flaky awareness."
    // WHY: Real bugs surface under repetition. Data consistency under stress.
    // SCALE: Add memory leaks, long session tests. Quarantine flaky, don't ignore.
    // RELEASE: Confidence that app won't corrupt data under normal use.
    /// <summary>
    /// Play Mode: Stability-oriented tests. Repeated operations, consistency checks.
    /// Demonstrates awareness of flaky tests and data integrity.
    /// </summary>
    [TestFixture]
    [Category("Regression")]
    public class StabilityPlayModeTests : TestSetupBase
    {
        private const int RepeatCount = 5;

        [UnityTest]
        [Timeout(15000)]
        [Retry(3)]
        public IEnumerator RepeatedSceneReload_NoFatalErrors()
        {
            for (int i = 0; i < RepeatCount; i++)
            {
                yield return SceneLoaderHelper.LoadSceneAndWait(SceneConstants.MainMenu);
                yield return new WaitForSeconds(0.2f);
            }
            Assert.Pass($"Completed {RepeatCount} scene reloads without error");
        }

        [UnityTest]
        public IEnumerator RepeatedSaveLoad_ProgressConsistent()
        {
            var service = FakeDataFactory.CreateProgressService(3, 250, true);
            for (int i = 0; i < RepeatCount; i++)
            {
                service.Save();
                service.Load();
                Assert.AreEqual(3, service.CurrentLevel);
                Assert.AreEqual(250, service.TotalScore);
                Assert.IsTrue(service.HasCompletedTutorial);
                yield return null;
            }
        }

        [UnityTest]
        public IEnumerator RepeatedCurrencyAdd_ConsistentBalance()
        {
            var service = FakeDataFactory.CreateCurrencyService(0);
            for (int i = 0; i < RepeatCount; i++)
            {
                service.Add(TestConstants.DefaultCurrencyReward);
            }
            Assert.AreEqual(RepeatCount * TestConstants.DefaultCurrencyReward, service.Balance);
            yield return null;
        }
    }
}
