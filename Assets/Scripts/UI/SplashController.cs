using UnityEngine;
using UnityEngine.SceneManagement;
using UnityMobileQA.Core;

namespace UnityMobileQA.UI
{
    /// <summary>
    /// Splash screen controller. Transitions to MainMenu after delay or on tap.
    /// </summary>
    public class SplashController : MonoBehaviour
    {
        [SerializeField] private float autoTransitionDelay = 2f;

        private float _elapsed;

        private void Awake()
        {
            AccessibilityBridge.SetRootContentDescription("UnityMobileQA");
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            if (_elapsed >= autoTransitionDelay)
            {
                SceneManager.LoadScene(SceneConstants.MainMenu);
            }
        }

        /// <summary>Called by Start button. Loads MainMenu immediately.</summary>
        public void OnStartClicked()
        {
            SceneManager.LoadScene(SceneConstants.MainMenu);
        }
    }
}
