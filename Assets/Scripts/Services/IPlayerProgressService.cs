namespace UnityMobileQA.Services
{
    /// <summary>
    /// Player progress persistence. Implementations: PlayerProgressService (prod), FakeDataFactory (tests).
    /// </summary>
    public interface IPlayerProgressService
    {
        int CurrentLevel { get; }
        int TotalScore { get; }
        bool HasCompletedTutorial { get; }

        void SetLevel(int level);
        void AddScore(int points);
        void SetTutorialComplete(bool complete);
        void Save();
        void Load();
        void Reset();
    }
}
