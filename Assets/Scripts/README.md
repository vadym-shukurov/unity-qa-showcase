# Scripts Architecture

Quick reference for navigating and maintaining the core codebase.

---

## Structure

```
Scripts/
├── Core/           # Game state, scene flow, service locator
├── Gameplay/       # Session logic, gameplay scene controller
├── UI/             # Scene controllers (MainMenu, Result)
└── Services/      # Persistence, currency, settings, mocks
```

---

## Data Flow

```
ServiceLocator (Awake)
    └── Creates: PlayerProgressService, CurrencyService, SettingsService, MockAd, MockIAP
    └── Creates: GameManager(progress, currency, settings, ads, iap)
    └── DontDestroyOnLoad → persists across scenes

MainMenuController.OnPlayClicked()
    └── SceneFlowController.LoadScene(Gameplay)

GameplayController.SimulateCompletion(score)
    └── GameplaySession.AddScore(score)
    └── GameplaySession.Complete() → GameManager.CompleteSession()
    └── SceneFlowController.LoadScene(Result)

ResultScreenController.OnBackToMenuClicked()
    └── GameManager.ReturnToMenu()
    └── SceneFlowController.LoadScene(MainMenu)
```

---

## Key Conventions

| Convention | Where |
|------------|-------|
| **Interfaces** | All services (IPlayerProgressService, etc.) for testability |
| **Constructor injection** | GameManager gets services via ctor; no static access |
| **Region grouping** | #region for Dependencies, State, Lifecycle, Public API |
| **SceneConstants** | Use constants, not string literals, for scene names |
| **SecureStorage** | All persistent data (progress, currency, settings) |

---

## Testability

- **Edit Mode**: Inject fakes via `FakeDataFactory.CreateProgressService()`, etc.
- **Play Mode**: ServiceLocator provides real services; tests use SceneLoaderHelper.
- **Test isolation**: `ServiceLocator.ResetForTests()` called in `TestSetupBase.TearDown`; no shared state between Play Mode tests.
- **No static state** in GameManager; ServiceLocator is the only singleton.

---

## Adding a New Service

1. Create `IYourService` interface
2. Create `YourService` implementation
3. Add to `ServiceLocator.Awake()` and `GameManager` ctor
4. Add `FakeDataFactory.CreateYourService()` for tests
