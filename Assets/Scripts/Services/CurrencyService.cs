using System;
using UnityEngine;

namespace UnityMobileQA.Services
{
    /// <summary>
    /// Manages virtual currency. Persists via SecureStorage (AES-256 encrypted).
    /// </summary>
    /// <remarks>
    /// - Validation: Rejects negative amounts; caps balance to prevent overflow.
    /// - Spend: Returns false if insufficient; does not throw.
    /// - Tests: Use FakeDataFactory.CreateCurrencyService() for Edit Mode.
    /// </remarks>
    public class CurrencyService : ICurrencyService
    {
        #region Constants

        private const string KeyBalance = "Currency_Balance";
        private const int MaxBalance = 999999999;

        #endregion

        #region State

        private int _balance;
        public int Balance => _balance;

        #endregion

        #region Constructor

        public CurrencyService()
        {
            Load();
        }

        #endregion

        #region ICurrencyService

        /// <summary>Adds currency. Validates amount; caps total to prevent overflow.</summary>
        public void Add(int amount)
        {
            if (amount < 0) throw new ArgumentException("Amount cannot be negative.");
            _balance = (int)Math.Min((long)_balance + amount, MaxBalance);
            Save();
        }

        /// <summary>Spends currency if sufficient. Returns false if insufficient.</summary>
        public bool Spend(int amount)
        {
            if (amount < 0) throw new ArgumentException("Amount cannot be negative.");
            if (_balance < amount) return false;
            _balance -= amount;
            Save();
            return true;
        }

        public void Reset()
        {
            _balance = 0;
            Save();
        }

        #endregion

        #region Persistence

        private void Save() => SecureStorage.SetInt(KeyBalance, _balance);
        private void Load() => _balance = SecureStorage.GetInt(KeyBalance, 0);

        #endregion
    }
}
