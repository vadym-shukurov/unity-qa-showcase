using System.Collections;

namespace UnityMobileQA.Core
{
    /// <summary>
    /// Abstraction for scene loading. Enables mocking in tests and swapping implementations.
    /// </summary>
    public interface ISceneFlowController
    {
        /// <summary>Name of the currently active scene.</summary>
        string CurrentSceneName { get; }

        /// <summary>Loads scene synchronously.</summary>
        void LoadScene(string sceneName);

        /// <summary>Loads scene asynchronously. Yield in coroutine.</summary>
        IEnumerator LoadSceneAsync(string sceneName);
    }
}
