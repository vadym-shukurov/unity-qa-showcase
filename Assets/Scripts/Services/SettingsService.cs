using UnityEngine;

namespace UnityMobileQA.Services
{
    /// <summary>
    /// Persists user settings (volume, notifications). Values clamped 0–1 to prevent corruption.
    /// </summary>
    /// <remarks>
    /// - Volume: Clamped 0–1; NaN/Infinity replaced with 1f.
    /// - Tests: Use FakeDataFactory.CreateSettingsService() for Edit Mode.
    /// </remarks>
    public class SettingsService : ISettingsService
    {
        #region Storage Keys

        private const string KeyMusic = "Settings_MusicVolume";
        private const string KeySfx = "Settings_SfxVolume";
        private const string KeyNotifications = "Settings_Notifications";

        #endregion

        #region State (backing fields for validation)

        private float _musicVolume = 1f;
        private float _sfxVolume = 1f;
        private bool _notificationsEnabled = true;

        public float MusicVolume { get => _musicVolume; set => _musicVolume = Clamp01(value); }
        public float SfxVolume { get => _sfxVolume; set => _sfxVolume = Clamp01(value); }
        public bool NotificationsEnabled { get => _notificationsEnabled; set => _notificationsEnabled = value; }

        #endregion

        #region Constructor

        public SettingsService()
        {
            Load();
        }

        #endregion

        #region ISettingsService

        public void Save()
        {
            SecureStorage.SetFloat(KeyMusic, _musicVolume);
            SecureStorage.SetFloat(KeySfx, _sfxVolume);
            SecureStorage.SetInt(KeyNotifications, _notificationsEnabled ? 1 : 0);
        }

        public void Load()
        {
            _musicVolume = Clamp01(SecureStorage.GetFloat(KeyMusic, 1f));
            _sfxVolume = Clamp01(SecureStorage.GetFloat(KeySfx, 1f));
            _notificationsEnabled = SecureStorage.GetInt(KeyNotifications, 1) == 1;
        }

        #endregion

        #region Helpers

        /// <summary>Clamps volume to 0–1. Handles NaN/Infinity.</summary>
        private static float Clamp01(float value)
        {
            if (float.IsNaN(value) || float.IsInfinity(value)) return 1f;
            return Mathf.Clamp(value, 0f, 1f);
        }

        #endregion
    }
}
