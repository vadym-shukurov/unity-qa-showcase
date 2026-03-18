using System;
using System.Security.Cryptography;
using UnityEngine;

namespace UnityMobileQA.Services
{
    /// <summary>
    /// AES-256 encrypted key-value storage over PlayerPrefs.
    /// Protects progress, currency, settings from backup extraction and casual file access.
    /// </summary>
    /// <remarks>
    /// - Key derivation: PBKDF2 (100k iterations) from device + app identifier.
    /// - For hardware-backed keys (Android Keystore, iOS Keychain), replace with platform API.
    /// - Decrypt returns empty/default on corruption; does not throw.
    /// </remarks>
    public static class SecureStorage
    {
        #region Constants

        private const string KeyPrefix = "Secure_";
        private const int KeySize = 32;      // AES-256
        private const int IvSize = 16;       // AES block size
        private const int Iterations = 100000;

        #endregion

        #region Key Derivation

        private static byte[] GetSalt()
        {
            var appSalt = $"{Application.identifier}_UnityMobileQA_Salt";
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(appSalt));
            var salt = new byte[16];
            Buffer.BlockCopy(hash, 0, salt, 0, 16);
            return salt;
        }

        private static byte[] GetKey()
        {
            var salt = GetSalt();
            // deviceUniqueIdentifier: deprecated on iOS; may return fixed/empty value. For production,
            // prefer Android Keystore / iOS Keychain for hardware-backed key storage.
            var deviceId = string.IsNullOrEmpty(SystemInfo.deviceUniqueIdentifier) ? "fallback" : SystemInfo.deviceUniqueIdentifier;
            var password = $"{Application.productName}_{Application.identifier}_{deviceId}";
            using var derive = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            return derive.GetBytes(KeySize);
        }

        #endregion

        #region Encrypt / Decrypt

        private static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;
            using var aes = Aes.Create();
            aes.Key = GetKey();
            aes.GenerateIV();
            using var encryptor = aes.CreateEncryptor();
            var plain = System.Text.Encoding.UTF8.GetBytes(plainText);
            var cipher = encryptor.TransformFinalBlock(plain, 0, plain.Length);
            var result = new byte[IvSize + cipher.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, IvSize);
            Buffer.BlockCopy(cipher, 0, result, IvSize, cipher.Length);
            return Convert.ToBase64String(result);
        }

        private static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return string.Empty;
            try
            {
                var data = Convert.FromBase64String(cipherText);
                if (data.Length < IvSize) return string.Empty;
                using var aes = Aes.Create();
                aes.Key = GetKey();
                var iv = new byte[IvSize];
                Buffer.BlockCopy(data, 0, iv, 0, IvSize);
                aes.IV = iv;
                using var decryptor = aes.CreateDecryptor();
                var cipher = new byte[data.Length - IvSize];
                Buffer.BlockCopy(data, IvSize, cipher, 0, cipher.Length);
                var plain = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
                return System.Text.Encoding.UTF8.GetString(plain);
            }
            catch
            {
                return string.Empty; // Corrupt data: fail gracefully
            }
        }

        #endregion

        #region Public API

        private static void ValidateKey(string key)
        {
            if (string.IsNullOrEmpty(key) || key.Length > 128)
                throw new ArgumentException("Key must be non-empty and ≤128 chars.");
        }

        public static void SetInt(string key, int value)
        {
            ValidateKey(key);
            PlayerPrefs.SetString(KeyPrefix + key, Encrypt(value.ToString()));
            PlayerPrefs.Save();
        }

        public static int GetInt(string key, int defaultValue)
        {
            if (string.IsNullOrEmpty(key)) return defaultValue;
            var raw = PlayerPrefs.GetString(KeyPrefix + key, null);
            if (string.IsNullOrEmpty(raw)) return defaultValue;
            var dec = Decrypt(raw);
            return int.TryParse(dec, out var v) ? v : defaultValue;
        }

        public static void SetFloat(string key, float value)
        {
            ValidateKey(key);
            PlayerPrefs.SetString(KeyPrefix + key, Encrypt(value.ToString("R")));
            PlayerPrefs.Save();
        }

        public static float GetFloat(string key, float defaultValue)
        {
            if (string.IsNullOrEmpty(key)) return defaultValue;
            var raw = PlayerPrefs.GetString(KeyPrefix + key, null);
            if (string.IsNullOrEmpty(raw)) return defaultValue;
            var dec = Decrypt(raw);
            return float.TryParse(dec, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : defaultValue;
        }

        #endregion
    }
}
