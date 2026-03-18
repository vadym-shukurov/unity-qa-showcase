namespace UnityMobileQA.Services
{
    /// <summary>
    /// Mock in-app purchase service interface. In production, wraps store SDK.
    /// </summary>
    public interface IMockIAPService
    {
        bool IsInitialized { get; }
        void Purchase(string productId, System.Action<bool> onComplete);
    }
}
