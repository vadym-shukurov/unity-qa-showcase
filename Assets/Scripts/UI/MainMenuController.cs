using UnityEngine;
using UnityMobileQA.Core;

namespace UnityMobileQA.UI
{
    /// <summary>
    /// Main menu UI controller. Handles Play and Settings button navigation.
    /// Wire button OnClick to OnPlayClicked / OnSettingsClicked.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        private ISceneFlowController _sceneFlow;

        private void Awake()
        {
            _sceneFlow = new SceneFlowController();
        }

        /// <summary>Called by Play button. Loads Gameplay scene.</summary>
        public void OnPlayClicked()
        {
            _sceneFlow.LoadScene(SceneConstants.Gameplay);
        }

        /// <summary>Called by Settings button. Loads Settings scene.</summary>
        public void OnSettingsClicked()
        {
            _sceneFlow.LoadScene(SceneConstants.Settings);
        }
    }
}
