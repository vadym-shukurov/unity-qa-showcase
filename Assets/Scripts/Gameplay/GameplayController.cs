using UnityEngine;
using UnityMobileQA.Core;

namespace UnityMobileQA.Gameplay
{
    /// <summary>
    /// Gameplay scene controller. Handles session completion and navigation to result screen.
    /// Requires ServiceLocator in scene (from MainMenu bootstrap).
    /// </summary>
    /// <remarks>
    /// SimulateCompletion: Used by tests and demo. In production, trigger from game logic (e.g. level end).
    /// </remarks>
    public class GameplayController : MonoBehaviour
    {
        #region Dependencies

        private ISceneFlowController _sceneFlow;
        private GameplaySession _session;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            _sceneFlow = new SceneFlowController();
            var gm = Core.ServiceLocator.Instance?.GameManager;
            if (gm != null)
                _session = new GameplaySession(gm);
            // If ServiceLocator not ready (e.g. direct scene load), _session stays null
        }

        #endregion

        #region Public API

        /// <summary>
        /// Completes the session with given score, then loads Result scene.
        /// No-op if ServiceLocator/GameManager not available.
        /// </summary>
        /// <param name="score">Score to record for this session.</param>
        public void SimulateCompletion(int score)
        {
            if (_session == null) return;
            _session.AddScore(score);
            _session.Complete();
            _sceneFlow.LoadScene(SceneConstants.Result);
        }

        #endregion
    }
}
