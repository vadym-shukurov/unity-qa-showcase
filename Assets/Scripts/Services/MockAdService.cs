using System;

namespace UnityMobileQA.Services
{
    /// <summary>
    /// Mock ad service. Always reports ready and success. Replace with real SDK (AdMob, Unity Ads) for production.
    /// </summary>
    public class MockAdService : IMockAdService
    {
        public bool IsAdReady => true;

        public void ShowRewardedAd(Action<bool> onComplete)
        {
            onComplete?.Invoke(true);
        }
    }
}
