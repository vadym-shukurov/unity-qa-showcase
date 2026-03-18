using NUnit.Framework;
using UnityMobileQA.Core;
using UnityMobileQA.Gameplay;
using UnityMobileQA.Services;
using UnityMobileQA.Tests.TestUtils;

namespace UnityMobileQA.Tests.EditMode
{
    // PRESENTATION: "GameplaySession — score flow, double-complete protection."
    // WHY: Prevents double reward exploit. Validates session state machine.
    // SCALE: Add timeout, pause/resume scenarios in real project.
    // RELEASE: Ensures score and reward are consistent.
    /// <summary>
    /// Edit Mode: GameplaySession logic. Validates score flow and completion.
    /// </summary>
    [TestFixture]
    [Category("DataIntegrity")]
    public class GameplaySessionEditModeTests : TestSetupBase
    {
        [Test]
        public void Complete_GrantsRewardAndAdvancesLevel()
        {
            var progress = FakeDataFactory.CreateProgressService(1, 0, false);
            var currency = FakeDataFactory.CreateCurrencyService(0);
            var gm = new GameManager(progress, currency, FakeDataFactory.CreateSettingsService(),
                new MockAdService(), new MockIAPService());
            gm.CurrencyRewardPerLevel = TestConstants.DefaultCurrencyReward;

            var session = new GameplaySession(gm);
            session.Start();
            session.AddScore(TestConstants.SampleScore);
            session.Complete();

            Assert.IsTrue(session.IsComplete);
            Assert.AreEqual(TestConstants.SampleScore, gm.Progress.TotalScore);
            Assert.AreEqual(2, gm.Progress.CurrentLevel);
            Assert.AreEqual(TestConstants.DefaultCurrencyReward, gm.Currency.Balance);
        }

        [Test]
        public void AddScoreAfterComplete_DoesNotIncrease()
        {
            var progress = FakeDataFactory.CreateProgressService(1, 0, false);
            var currency = FakeDataFactory.CreateCurrencyService(0);
            var gm = new GameManager(progress, currency, FakeDataFactory.CreateSettingsService(),
                new MockAdService(), new MockIAPService());

            var session = new GameplaySession(gm);
            session.Start();
            session.AddScore(50);
            session.Complete();
            session.AddScore(100);

            Assert.AreEqual(50, gm.Progress.TotalScore);
        }
    }
}
