using NUnit.Framework;
using UnityEngine;
using UnityMobileQA.Services;
using UnityMobileQA.Tests.TestUtils;

namespace UnityMobileQA.Tests.EditMode
{
    /// <summary>
    /// Edit Mode: SecureStorage encryption and round-trip. Verifies data protection.
    /// </summary>
    [TestFixture]
    [Category("DataIntegrity")]
    [Category("BuildConfig")]
    public class SecureStorageEditModeTests : TestSetupBase
    {
        private const string TestKey = "SecureStorageTest_Key";

        [Test]
        public void SetInt_GetInt_RoundTripsCorrectly()
        {
            SecureStorage.SetInt(TestKey, 42);
            var result = SecureStorage.GetInt(TestKey, -1);
            Assert.AreEqual(42, result);
        }

        [Test]
        public void SetFloat_GetFloat_RoundTripsCorrectly()
        {
            SecureStorage.SetFloat(TestKey, 0.75f);
            var result = SecureStorage.GetFloat(TestKey, -1f);
            Assert.AreEqual(0.75f, result, 0.001f);
        }

        [Test]
        public void GetInt_ReturnsDefault_WhenKeyMissing()
        {
            var result = SecureStorage.GetInt("NonExistentKey_123", 99);
            Assert.AreEqual(99, result);
        }

        [Test]
        public void GetFloat_ReturnsDefault_WhenKeyMissing()
        {
            var result = SecureStorage.GetFloat("NonExistentKey_456", 1.5f);
            Assert.AreEqual(1.5f, result, 0.001f);
        }

        [Test]
        public void StoredValue_IsEncrypted_NotPlaintext()
        {
            SecureStorage.SetInt(TestKey, 12345);
            var raw = PlayerPrefs.GetString("Secure_" + TestKey, "");
            Assert.IsNotEmpty(raw);
            Assert.AreNotEqual("12345", raw, "Raw storage must not contain plaintext");
            Assert.DoesNotThrow(() => System.Convert.FromBase64String(raw), "Stored value should be valid Base64");
        }

        [Test]
        public void CorruptData_ReturnsDefault_Gracefully()
        {
            PlayerPrefs.SetString("Secure_" + TestKey, "not-valid-base64!!!");
            PlayerPrefs.Save();
            var result = SecureStorage.GetInt(TestKey, 999);
            Assert.AreEqual(999, result);
        }

        [Test]
        public void SetInt_NullOrEmptyKey_Throws()
        {
            Assert.Throws<ArgumentException>(() => SecureStorage.SetInt(null, 1));
            Assert.Throws<ArgumentException>(() => SecureStorage.SetInt("", 1));
        }

        [Test]
        public void SetFloat_NullOrEmptyKey_Throws()
        {
            Assert.Throws<ArgumentException>(() => SecureStorage.SetFloat(null, 1f));
            Assert.Throws<ArgumentException>(() => SecureStorage.SetFloat("", 1f));
        }
    }
}
