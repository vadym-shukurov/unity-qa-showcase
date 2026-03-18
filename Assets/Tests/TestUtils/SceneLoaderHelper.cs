using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace UnityMobileQA.Tests.TestUtils
{
    // PRESENTATION: "Reusable helper — Play Mode tests share scene load logic."
    // WHY: Avoid copy-paste. Centralized timeout, Inconclusive if scene missing.
    // SCALE: Add more helpers (wait for UI, wait for service) as tests grow.
    // RELEASE: Consistent scene setup = reliable Play Mode tests.
    /// <summary>
    /// Helper for loading scenes in Play Mode tests. Handles async loading and timeouts.
    /// </summary>
    public static class SceneLoaderHelper
    {
        public const float DefaultTimeoutSeconds = 10f;

        public static IEnumerator LoadSceneAndWait(string sceneName, float timeout = DefaultTimeoutSeconds)
        {
            if (Application.CanStreamedLevelBeLoaded(sceneName))
            {
                var op = SceneManager.LoadSceneAsync(sceneName);
                float elapsed = 0f;
                while (op != null && !op.isDone && elapsed < timeout)
                {
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                Assert.IsTrue(op != null && op.isDone, $"Scene {sceneName} failed to load within {timeout}s");
            }
            else
            {
                Assert.Inconclusive($"Scene '{sceneName}' is not in Build Settings. Add it for Play Mode tests.");
            }
        }

        public static string GetActiveSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }
    }
}
