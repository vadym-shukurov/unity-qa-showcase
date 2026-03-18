using UnityEngine;
using UnityMobileQA.Core;
using UnityMobileQA.Services;

namespace UnityMobileQA.UI
{
    /// <summary>
    /// Result screen controller. Handles Back to Menu button.
    /// Resets GameManager session state before loading MainMenu.
    /// </summary>
    public class ResultScreenController : MonoBehaviour
    {
        private ISceneFlowController _sceneFlow;
        private GameManager _gameManager;

        private void Awake()
        {
            _sceneFlow = new SceneFlowController();
            _gameManager = ServiceLocator.Instance?.GameManager;
        }

        /// <summary>Called by Back to Menu button. Resets session, loads MainMenu.</summary>
        public void OnBackToMenuClicked()
        {
            _gameManager?.ReturnToMenu(); // Clear session score before next play
            _sceneFlow.LoadScene(SceneConstants.MainMenu);
        }
    }
}
