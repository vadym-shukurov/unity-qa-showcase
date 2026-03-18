namespace UnityMobileQA.Services
{
    /// <summary>
    /// Virtual currency management. Implementations: CurrencyService (prod), FakeDataFactory (tests).
    /// </summary>
    public interface ICurrencyService
    {
        int Balance { get; }

        void Add(int amount);
        bool Spend(int amount);
        void Reset();
    }
}
