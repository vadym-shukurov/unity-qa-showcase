using System;
using UnityMobileQA.Services;

namespace UnityMobileQA.Core
{
    /// <summary>
    /// Central game state coordinator. Orchestrates session flow, progress, and rewards.
    /// Designed for testability: all dependencies injected via constructor (no static access).
    /// </summary>
    /// <remarks>
    /// Best practice: Use interfaces for all services so tests can inject mocks.
    /// See GameManagerEditModeTests for examples.
    /// </remarks>
    public class GameManager
    {
        #region Services (injected)

        /// <summary>Player progress (level, score, tutorial). Persisted across sessions.</summary>
        public IPlayerProgressService Progress { get; }

        /// <summary>Virtual currency. Used for rewards and IAP.</summary>
        public ICurrencyService Currency { get; }

        /// <summary>User preferences (volume, notifications).</summary>
        public ISettingsService Settings { get; }

        /// <summary>Ad integration (mocked in demo).</summary>
        public IMockAdService Ads { get; }

        /// <summary>In-app purchase (mocked in demo).</summary>
        public IMockIAPService IAP { get; }

        #endregion

        #region Session State

        /// <summary>Score accumulated in the current gameplay session. Reset on StartSession/ReturnToMenu.</summary>
        public int SessionScore { get; private set; }

        /// <summary>Currency granted when a level is completed. Configurable per level if needed.</summary>
        public int CurrencyRewardPerLevel { get; set; } = 50;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a GameManager with injected services. All services required (null = ArgumentNullException).
        /// </summary>
        public GameManager(
            IPlayerProgressService progress,
            ICurrencyService currency,
            ISettingsService settings,
            IMockAdService ads,
            IMockIAPService iap)
        {
            Progress = progress ?? throw new ArgumentNullException(nameof(progress));
            Currency = currency ?? throw new ArgumentNullException(nameof(currency));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Ads = ads ?? throw new ArgumentNullException(nameof(ads));
            IAP = iap ?? throw new ArgumentNullException(nameof(iap));
        }

        #endregion

        #region Session Lifecycle

        /// <summary>Starts a new gameplay session. Resets session score.</summary>
        public void StartSession()
        {
            SessionScore = 0;
        }

        /// <summary>Adds points to the current session. Ignores negative values.</summary>
        /// <param name="points">Points to add. Must be &gt;= 0.</param>
        public void AddSessionScore(int points)
        {
            if (points < 0) return;
            SessionScore += points;
        }

        /// <summary>
        /// Completes the current session: persists score, advances level, grants currency reward.
        /// Call when player finishes a level.
        /// </summary>
        public void CompleteSession()
        {
            Progress.AddScore(SessionScore);
            Progress.SetLevel(Progress.CurrentLevel + 1);
            Currency.Add(CurrencyRewardPerLevel);
            Progress.Save();
        }

        /// <summary>Resets session state when returning to menu (e.g. from result screen).</summary>
        public void ReturnToMenu()
        {
            SessionScore = 0;
        }

        #endregion
    }
}
