# Setup Guide

*Use this when demoing: "Here's how to run the tests."*

---

## Auto-Setup (Recommended)
On first open, the project **automatically creates** the 5 required scenes with controllers and visible UI (Canvas, centered buttons). Includes: ServiceLocator, MainMenuController, GameplayController, ResultScreenController, SplashController. No manual setup needed for Play Mode tests or device full-flow.

## IL2CPP (for Unity-specific tests)
Player Settings → Other Settings → Scripting Backend → **IL2CPP** (Android, iOS). Required for `UnitySpecificEditModeTests`. **Validate**: Tools → Unity Mobile QA → Validate Build Security.

## Unity Version
- **Recommended**: Unity 2022.3 LTS
- Targets: Android, iOS

## Opening the Project
1. Open Unity Hub
2. Add project from disk: select the `Unity-Mobile-QA` folder
3. Open with Unity 2022.3 or later

## Scenes for Play Mode Tests

Play Mode tests require these scenes in **Build Settings** (File → Build Settings):

| Scene Name | Purpose |
|------------|---------|
| Splash | Start/splash screen |
| MainMenu | Main menu |
| Settings | Settings screen |
| Gameplay | Gameplay scene |
| Result | Result/reward screen |

### Creating Scenes
1. Create new scene: File → New Scene
2. Save as `Assets/Scenes/<SceneName>.unity` (e.g. `MainMenu.unity`)
3. Add each scene to Build Settings: File → Build Settings → Add Open Scenes
4. Ensure scene names match `SceneConstants` (Splash, MainMenu, Settings, Gameplay, Result)

### Minimal Scene Setup
Each scene can be empty (just a camera and directional light). The tests validate scene loading and transitions, not visual content.

## Running Tests
- **Edit Mode**: Window → General → Test Runner → Edit Mode → Run All
- **Play Mode**: Test Runner → Play Mode → Run All
- **Smoke only**: In Test Runner, filter by `Category = Smoke` for fast release gate

Edit Mode tests run without scenes. Play Mode tests will report `Inconclusive` if scenes are missing.

## Command-Line (Proof of Execution)
**Mac/Linux:**
```bash
chmod +x scripts/run-tests.sh
./scripts/run-tests.sh all
```
**Windows:**
```batch
scripts\run-tests.bat all
```
Output: `reports/editmode-results.xml`, `reports/playmode-results.xml` (JUnit-style).

**Cross-platform:** Scripts auto-detect Unity (Hub on Mac/Windows, `/opt/Unity` on Linux). Override with `UNITY_PATH` if needed.

## CI Setup (GitHub Actions)
1. Get Unity license: use [unity-request-activation-file](https://github.com/marketplace/actions/unity-request-activation-file) action
2. Add `UNITY_LICENSE` as repository secret
3. Push to trigger pipeline — game-ci runs Edit/Play Mode tests, Android build, metrics dashboard
4. Metrics: `scripts/generate-metrics.js` produces `reports/metrics.json` and `reports/dashboard.html`
5. **Device farm:** When APK is built, `device-farm.yml` runs Appium smoke on an Android emulator (reactivecircus/android-emulator-runner).
6. **Secrets/env**: Copy `.env.example` to `.env` for JIRA_TOKEN, APK_PATH (Appium). Never commit `.env`.
7. **Jira**: See `scripts/jira-integration.example.js` and `scripts/report-failure.example.js` for defect-tracking (both validate URLs for SSRF prevention).
8. **Security**: See [security.md](security.md) for secrets, signing, and supply-chain practices.
