using NUnit.Framework;
using UnityMobileQA.Services;
using UnityMobileQA.Tests.TestUtils;

namespace UnityMobileQA.Tests.EditMode
{
    // PRESENTATION: "I automated this FIRST — progress loss is highest risk."
    // WHY: Data integrity = player churn. Fast Edit Mode, no runtime.
    // SCALE: Same pattern for other services (currency, settings). Add edge cases as bugs surface.
    // RELEASE: Passing = no progress corruption. Release gate.
    /// <summary>
    /// Edit Mode: Player progress save/load. Fast, deterministic, no runtime.
    /// </summary>
    [TestFixture]
    [Category("DataIntegrity")]
    public class PlayerProgressEditModeTests : TestSetupBase
    {
        [Test]
        public void SaveAndLoad_PersistsLevelAndScore()
        {
            var service = FakeDataFactory.CreateProgressService(TestConstants.SampleLevel, TestConstants.SampleScore, true);
            service.Save();

            var loaded = new PlayerProgressService();
            loaded.Load();

            AssertionHelpers.AssertProgressEquals(
                TestConstants.SampleLevel, TestConstants.SampleScore, true,
                loaded.CurrentLevel, loaded.TotalScore, loaded.HasCompletedTutorial);
        }

        [Test]
        public void Reset_ClearsAllProgress()
        {
            var service = FakeDataFactory.CreateProgressService(5, 500, true);
            service.Reset();

            Assert.AreEqual(1, service.CurrentLevel);
            Assert.AreEqual(0, service.TotalScore);
            Assert.IsFalse(service.HasCompletedTutorial);
        }

        [Test]
        public void AddScore_IncreasesTotalScore()
        {
            var service = FakeDataFactory.CreateProgressService(1, 0, false);
            service.AddScore(100);
            service.AddScore(50);

            Assert.AreEqual(150, service.TotalScore);
        }

        [Test]
        public void SetLevel_Negative_Throws()
        {
            var service = new PlayerProgressService();
            Assert.Throws<System.ArgumentException>(() => service.SetLevel(-1));
        }

        [Test]
        public void AddScore_Negative_Throws()
        {
            var service = FakeDataFactory.CreateProgressService(1, 0, false);
            Assert.Throws<System.ArgumentException>(() => service.AddScore(-1));
        }

        [TestCase(1, 0, false)]
        [TestCase(5, 500, true)]
        [TestCase(10, 0, false)]
        public void SaveAndLoad_Parameterized_PersistsCorrectly(int level, int score, bool tutorial)
        {
            var service = FakeDataFactory.CreateProgressService(level, score, tutorial);
            service.Save();
            var loaded = new PlayerProgressService();
            loaded.Load();
            AssertionHelpers.AssertProgressEquals(level, score, tutorial,
                loaded.CurrentLevel, loaded.TotalScore, loaded.HasCompletedTutorial);
        }
    }
}
