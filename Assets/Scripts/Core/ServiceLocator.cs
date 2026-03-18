using UnityEngine;
using UnityMobileQA.Services;

namespace UnityMobileQA.Core
{
    /// <summary>
    /// Runtime service locator. Provides GameManager to UI and gameplay scenes.
    /// Attach to a GameObject in the first scene (e.g. Splash or MainMenu).
    /// </summary>
    /// <remarks>
    /// - Singleton: Only one instance; duplicates destroy themselves.
    /// - DontDestroyOnLoad: Persists across scene loads.
    /// - Tests: Do NOT use ServiceLocator; inject services directly into GameManager.
    /// </remarks>
    public class ServiceLocator : MonoBehaviour
    {
        #region Singleton

        /// <summary>Global access point. Null until first scene loads.</summary>
        public static ServiceLocator Instance { get; private set; }

        /// <summary>GameManager with all services. Created in Awake.</summary>
        public GameManager GameManager { get; private set; }

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Singleton: one instance only (handles scene reload in editor)
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Create concrete services (production implementations)
            var progress = new PlayerProgressService();
            var currency = new CurrencyService();
            var settings = new SettingsService();
            var ads = new MockAdService();
            var iap = new MockIAPService();

            GameManager = new GameManager(progress, currency, settings, ads, iap);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        /// <summary>
        /// Test only: Clears singleton so next scene load gets a fresh instance.
        /// Call from TestSetupBase.TearDown to ensure test isolation in Play Mode.
        /// </summary>
        public static void ResetForTests()
        {
            if (Instance != null)
            {
                DestroyImmediate(Instance.gameObject);
            }
        }

        #endregion
    }
}
