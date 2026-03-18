using NUnit.Framework;
using UnityMobileQA.Core;
using UnityMobileQA.Services;
using UnityMobileQA.Tests.TestUtils;

namespace UnityMobileQA.Tests.EditMode
{
    // PRESENTATION: "Core business logic — session completion, reward grant."
    // WHY: GameManager orchestrates progress + currency. Single point of failure.
    // SCALE: Add more session scenarios (quit mid-game, crash recovery) as needed.
    // RELEASE: Catches reward bugs before Play Mode. Fast feedback.
    /// <summary>
    /// Edit Mode: GameManager session completion and currency reward.
    /// Validates core business logic without runtime.
    /// </summary>
    [TestFixture]
    [Category("DataIntegrity")]
    public class GameManagerEditModeTests : TestSetupBase
    {
        private GameManager CreateGameManager()
        {
            var progress = FakeDataFactory.CreateProgressService(1, 0, false);
            var currency = FakeDataFactory.CreateCurrencyService(0);
            var settings = FakeDataFactory.CreateSettingsService();
            return new GameManager(progress, currency, settings, new MockAdService(), new MockIAPService());
        }

        [Test]
        public void CompleteSession_GrantsCurrencyReward()
        {
            var gm = CreateGameManager();
            gm.CurrencyRewardPerLevel = TestConstants.DefaultCurrencyReward;
            gm.AddSessionScore(TestConstants.SampleScore);
            var initialBalance = gm.Currency.Balance;

            gm.CompleteSession();

            AssertionHelpers.AssertCurrencyIncreased(initialBalance, TestConstants.DefaultCurrencyReward, gm.Currency.Balance);
        }

        [Test]
        public void CompleteSession_IncrementsLevelAndScore()
        {
            var gm = CreateGameManager();
            gm.AddSessionScore(100);

            gm.CompleteSession();

            Assert.AreEqual(2, gm.Progress.CurrentLevel);
            Assert.AreEqual(100, gm.Progress.TotalScore);
        }

        [Test]
        public void NullProgress_Throws()
        {
            var currency = FakeDataFactory.CreateCurrencyService();
            var settings = FakeDataFactory.CreateSettingsService();
            Assert.Throws<System.ArgumentNullException>(() =>
                new GameManager(null, currency, settings, new MockAdService(), new MockIAPService()));
        }

        [TestCase(50, 100, 150)]
        [TestCase(25, 200, 225)]
        [TestCase(100, 0, 100)]
        public void CompleteSession_GrantsConfiguredReward(int rewardPerLevel, int initialBalance, int expectedAfter)
        {
            var progress = FakeDataFactory.CreateProgressService(1, 0, false);
            var currency = FakeDataFactory.CreateCurrencyService(initialBalance);
            var gm = new GameManager(progress, currency, FakeDataFactory.CreateSettingsService(), new MockAdService(), new MockIAPService());
            gm.CurrencyRewardPerLevel = rewardPerLevel;
            gm.AddSessionScore(50);
            gm.CompleteSession();
            Assert.AreEqual(expectedAfter, gm.Currency.Balance);
        }

        [Test]
        public void AddSessionScore_Negative_IgnoresValue()
        {
            var gm = CreateGameManager();
            gm.AddSessionScore(-100);
            Assert.AreEqual(0, gm.SessionScore);
        }
    }
}
