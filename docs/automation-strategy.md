# Automation Strategy

**Use this doc to explain:** What I automated first, why that scope, how to scale, release confidence.

---

## Risk-Based Prioritization

Automation is prioritized by **risk**, not coverage percentage:

1. **Critical path** - App launch, main menu, gameplay flow, result, return to menu
2. **Data integrity** - Progress save/load, currency, settings persistence
3. **Monetization** - Currency rewards, IAP/ads integration (mocked in tests)
4. **Stability** - Repeated operations, scene reload, background/foreground

## Why Smoke First

- **Fast feedback**: Smoke tests run on every commit
- **Release gate**: No release if smoke fails
- **Foundation**: Smoke passing = app is runnable; other tests build on that

## Edit Mode vs Play Mode vs Device-Level

| Layer | What | When | Speed |
|-------|------|------|-------|
| **Edit Mode** | Logic, services, no runtime | Every commit | ~seconds |
| **Play Mode** | Scene flow, integration, smoke | Every PR, nightly | ~minutes |
| **Device** | Native integrations, real devices | Release builds | ~10–30 min |

### Edit Mode
- No `MonoBehaviour`, no `SceneManager` runtime
- Pure C# logic: services, GameManager, GameplaySession
- Fast, deterministic, easy to maintain

### Play Mode
- Requires Unity runtime, scenes in Build Settings
- Validates scene transitions, flow, basic smoke
- More flaky risk: timing, async, platform differences

### Device-Level
- Real APK/IPA on device/emulator
- Ads, IAP, push, analytics, memory, orientation
- Expensive: run on release builds only

## Tradeoffs and Limitations

- **Play Mode in CI**: Requires headless Unity, license, build time
- **Device tests**: Need device farm or cloud (BrowserStack, Unity Cloud)
- **Flaky tests**: Mitigate with retries, timeouts, quarantine
- **Coverage**: We do not aim for 100%; we aim for **high-risk coverage**

## Smoke vs Regression Flow (Play Mode)

| Flow Type | Test Class | Approach | Use |
|-----------|------------|----------|-----|
| **UI-driven** | SmokeFlowPlayModeTests | Simulates user taps via controllers (OnPlayClicked, SimulateCompletion) | Release gate; validates real navigation path |
| **API-driven** | RegressionFlowPlayModeTests | Uses SceneFlowController.LoadScene() directly | Regression; validates scene transitions and persistence |

Both cover the same path (Menu → Gameplay → Result → Menu); UI-driven is closer to user behavior.

## Test Categories

| Category | Scope | Run |
|----------|-------|-----|
| **Smoke** | Critical path, UI-driven full flow | Release gate; run first |
| **Regression** | Full flow, settings, stability, orientation | PR validation |
| **DataIntegrity** | Progress, currency, settings (Edit Mode) | Every commit |
| **Performance** | FPS, memory, load time, memory leaks | Pre-release |
| **BuildConfig** | IL2CPP, build settings | Every commit |

Filter in Test Runner: `[Category("Smoke")]` for smoke-only runs.

## Scaling as QA Lead (How I Would Scale)

1. **Start small**: Smoke + critical regression
2. **Stabilize**: Fix flaky tests before adding more
3. **Expand**: Add regression by risk area
4. **Device**: Add device smoke when build pipeline is ready
5. **Metrics**: Track pass rate, flaky rate, execution time
6. **Ownership**: Developers own unit tests; QA owns integration/smoke/device
