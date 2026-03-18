using System;

namespace UnityMobileQA.Services
{
    /// <summary>
    /// Mock IAP service. Simulates successful purchase. Replace with Unity IAP / platform SDK for production.
    /// </summary>
    public class MockIAPService : IMockIAPService
    {
        public bool IsInitialized => true;

        public void Purchase(string productId, Action<bool> onComplete)
        {
            onComplete?.Invoke(true);
        }
    }
}
