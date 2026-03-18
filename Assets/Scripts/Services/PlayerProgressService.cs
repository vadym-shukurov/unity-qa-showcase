using System;
using UnityEngine;

namespace UnityMobileQA.Services
{
    /// <summary>
    /// Handles persistent player progress (level, score, tutorial). Uses SecureStorage (AES-256).
    /// </summary>
    /// <remarks>
    /// - Validation: Rejects negative values; caps level/score to prevent overflow.
    /// - Tests: Use FakeDataFactory.CreateProgressService() for Edit Mode.
    /// - Swap: Implement IPlayerProgressService with cloud/file storage; interface unchanged.
    /// </remarks>
    public class PlayerProgressService : IPlayerProgressService
    {
        #region Storage Keys

        private const string KeyLevel = "PlayerProgress_Level";
        private const string KeyScore = "PlayerProgress_Score";
        private const string KeyTutorial = "PlayerProgress_Tutorial";

        #endregion

        #region Bounds (overflow prevention)

        private const int MaxLevel = 999999;
        private const int MaxScore = int.MaxValue - 1;

        #endregion

        #region State

        private int _currentLevel;
        private int _totalScore;
        private bool _hasCompletedTutorial;

        public int CurrentLevel => _currentLevel;
        public int TotalScore => _totalScore;
        public bool HasCompletedTutorial => _hasCompletedTutorial;

        #endregion

        #region Constructor

        public PlayerProgressService()
        {
            Load();
        }

        #endregion

        #region IPlayerProgressService

        public void SetLevel(int level)
        {
            if (level < 0) throw new ArgumentException("Level cannot be negative.");
            _currentLevel = Math.Min(level, MaxLevel);
        }

        public void AddScore(int points)
        {
            if (points < 0) throw new ArgumentException("Score cannot be negative.");
            _totalScore = (int)Math.Min((long)_totalScore + points, MaxScore);
        }

        public void SetTutorialComplete(bool complete)
        {
            _hasCompletedTutorial = complete;
        }

        public void Save()
        {
            SecureStorage.SetInt(KeyLevel, _currentLevel);
            SecureStorage.SetInt(KeyScore, _totalScore);
            SecureStorage.SetInt(KeyTutorial, _hasCompletedTutorial ? 1 : 0);
        }

        public void Load()
        {
            _currentLevel = SecureStorage.GetInt(KeyLevel, 1);
            _totalScore = SecureStorage.GetInt(KeyScore, 0);
            _hasCompletedTutorial = SecureStorage.GetInt(KeyTutorial, 0) == 1;
        }

        public void Reset()
        {
            _currentLevel = 1;
            _totalScore = 0;
            _hasCompletedTutorial = false;
            Save();
        }

        #endregion
    }
}
