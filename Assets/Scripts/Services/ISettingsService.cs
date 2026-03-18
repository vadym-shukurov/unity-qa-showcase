namespace UnityMobileQA.Services
{
    /// <summary>
    /// User settings persistence. Implementations: SettingsService (prod), FakeDataFactory (tests).
    /// </summary>
    public interface ISettingsService
    {
        float MusicVolume { get; set; }
        float SfxVolume { get; set; }
        bool NotificationsEnabled { get; set; }

        void Save();
        void Load();
    }
}
