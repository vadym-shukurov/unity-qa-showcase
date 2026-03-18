using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityMobileQA.Core
{
    /// <summary>
    /// Handles scene loading. Wraps Unity SceneManager for testability and consistency.
    /// Use SceneConstants for scene names (avoids typos, centralizes refs).
    /// </summary>
    public class SceneFlowController : ISceneFlowController
    {
        #region ISceneFlowController

        /// <summary>Name of the currently active scene.</summary>
        public string CurrentSceneName => SceneManager.GetActiveScene().name;

        /// <summary>Loads a scene synchronously. Blocks until load completes.</summary>
        /// <param name="sceneName">Scene name (must be in Build Settings). Use SceneConstants.</param>
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>Loads a scene asynchronously. Yield in coroutine until done.</summary>
        /// <param name="sceneName">Scene name. Use SceneConstants.</param>
        public IEnumerator LoadSceneAsync(string sceneName)
        {
            var op = SceneManager.LoadSceneAsync(sceneName);
            while (op != null && !op.isDone)
            {
                yield return null;
            }
        }

        #endregion
    }
}
