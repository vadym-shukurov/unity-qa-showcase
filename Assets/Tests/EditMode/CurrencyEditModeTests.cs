using NUnit.Framework;
using UnityMobileQA.Services;
using UnityMobileQA.Tests.TestUtils;

namespace UnityMobileQA.Tests.EditMode
{
    // PRESENTATION: "Second automation — currency = monetization risk."
    // WHY: Wrong reward or lost currency = revenue + trust impact.
    // SCALE: Add IAP/ads integration tests when real SDKs in place.
    // RELEASE: Ensures rewards granted correctly after level completion.
    /// <summary>
    /// Edit Mode: Currency service. Validates reward logic and persistence.
    /// </summary>
    [TestFixture]
    [Category("DataIntegrity")]
    public class CurrencyEditModeTests : TestSetupBase
    {
        [Test]
        public void Add_IncreasesBalance()
        {
            var service = FakeDataFactory.CreateCurrencyService(0);
            service.Add(TestConstants.DefaultCurrencyReward);

            Assert.AreEqual(TestConstants.DefaultCurrencyReward, service.Balance);
        }

        [Test]
        public void Spend_WhenSufficient_ReturnsTrueAndDeducts()
        {
            var service = FakeDataFactory.CreateCurrencyService(100);
            var result = service.Spend(30);

            Assert.IsTrue(result);
            Assert.AreEqual(70, service.Balance);
        }

        [Test]
        public void Spend_WhenInsufficient_ReturnsFalseAndDoesNotDeduct()
        {
            var service = FakeDataFactory.CreateCurrencyService(20);
            var result = service.Spend(50);

            Assert.IsFalse(result);
            Assert.AreEqual(20, service.Balance);
        }

        [Test]
        public void Add_Negative_Throws()
        {
            var service = new CurrencyService();
            Assert.Throws<System.ArgumentException>(() => service.Add(-1));
        }

        [Test]
        public void Spend_Negative_Throws()
        {
            var service = FakeDataFactory.CreateCurrencyService(100);
            Assert.Throws<System.ArgumentException>(() => service.Spend(-1));
        }

        [TestCase(0, 50, 50)]
        [TestCase(100, 30, 130)]
        [TestCase(0, 0, 0)]
        public void Add_Parameterized_IncreasesBalance(int initial, int add, int expected)
        {
            var service = FakeDataFactory.CreateCurrencyService(initial);
            service.Add(add);
            Assert.AreEqual(expected, service.Balance);
        }
    }
}
