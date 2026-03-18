namespace UnityMobileQA.Services
{
    /// <summary>
    /// Mock ad service interface for testability. In production, wraps real ad SDK.
    /// </summary>
    public interface IMockAdService
    {
        bool IsAdReady { get; }
        void ShowRewardedAd(System.Action<bool> onComplete);
    }
}
