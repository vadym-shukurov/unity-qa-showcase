using NUnit.Framework;

namespace UnityMobileQA.Tests.TestUtils
{
    // PRESENTATION: "Domain assertions — readable tests, consistent error messages."
    // WHY: Assert.AreEqual(x, y) vs AssertProgressEquals(...) — latter explains what failed.
    // SCALE: Add more helpers as domain grows. Keeps tests maintainable.
    // RELEASE: Clear failure messages = faster debugging.
    /// <summary>
    /// Custom assertion helpers for domain-specific checks.
    /// </summary>
    public static class AssertionHelpers
    {
        public static void AssertProgressEquals(int expectedLevel, int expectedScore, bool expectedTutorial,
            int actualLevel, int actualScore, bool actualTutorial)
        {
            Assert.AreEqual(expectedLevel, actualLevel, "Level mismatch");
            Assert.AreEqual(expectedScore, actualScore, "Score mismatch");
            Assert.AreEqual(expectedTutorial, actualTutorial, "Tutorial completion mismatch");
        }

        public static void AssertCurrencyIncreased(int initialBalance, int expectedGain, int finalBalance)
        {
            Assert.AreEqual(initialBalance + expectedGain, finalBalance,
                $"Currency should have increased by {expectedGain}");
        }
    }
}
