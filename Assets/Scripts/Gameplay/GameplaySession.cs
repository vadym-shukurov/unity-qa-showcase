using System;
using UnityEngine;
using UnityMobileQA.Core;
using UnityMobileQA.Services;

namespace UnityMobileQA.Gameplay
{
    /// <summary>
    /// Represents a single gameplay session. Wraps GameManager session lifecycle.
    /// Prevents double-complete (AddScore after Complete has no effect).
    /// </summary>
    /// <remarks>
    /// Flow: Start() → AddScore() (optional) → Complete(). Complete persists score and grants reward.
    /// </remarks>
    public class GameplaySession
    {
        #region Dependencies

        private readonly GameManager _gameManager;

        #endregion

        #region State

        private bool _isComplete;

        /// <summary>True after Complete() called. AddScore has no effect when complete.</summary>
        public bool IsComplete => _isComplete;

        /// <summary>Current session score (from GameManager).</summary>
        public int Score => _gameManager.SessionScore;

        #endregion

        #region Constructor

        public GameplaySession(GameManager gameManager)
        {
            _gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
        }

        #endregion

        #region Session Lifecycle

        /// <summary>Starts a new session. Resets GameManager session score.</summary>
        public void Start()
        {
            _gameManager.StartSession();
            _isComplete = false;
        }

        /// <summary>Adds points to session. No-op if already completed.</summary>
        public void AddScore(int points)
        {
            if (!_isComplete)
                _gameManager.AddSessionScore(points);
        }

        /// <summary>Completes session: persists score, advances level, grants currency. Idempotent.</summary>
        public void Complete()
        {
            if (_isComplete) return;
            _isComplete = true;
            _gameManager.CompleteSession();
        }

        #endregion
    }
}
