using UnityEngine;
using UnityMobileQA.Core;

namespace UnityMobileQA.UI
{
    /// <summary>
    /// Settings screen controller. Handles Back to Menu navigation.
    /// </summary>
    public class SettingsController : MonoBehaviour
    {
        private ISceneFlowController _sceneFlow;

        private void Awake()
        {
            _sceneFlow = new SceneFlowController();
        }

        /// <summary>Called by Back button. Returns to MainMenu.</summary>
        public void OnBackClicked()
        {
            _sceneFlow.LoadScene(SceneConstants.MainMenu);
        }
    }
}
