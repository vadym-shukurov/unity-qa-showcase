# Device-Level Automation Layer

**PRESENTATION:** "I automated Edit/Play first; device is placeholder. In production, device smoke runs after build."
**WHAT FIRST:** Device last — expensive, needs build. Edit/Play give fast feedback.
**WHY SCOPE:** Device tests for native integrations (ads, IAP, push). Not in this demo — specs only.
**SCALE:** Add when build pipeline ready. Parallel device pools. Screenshot + log on failure.
**RELEASE:** Final validation before store. Crash-free, install/launch, smoke on real device.

---

## Purpose
This folder contains the structure and specifications for **device-level** (Android/iOS) test automation. Unity Edit Mode and Play Mode tests run inside the Unity Editor; device tests run on actual devices or emulators after build.

## Why Device-Level Tests?
- **Native integrations**: Ads, IAP, push notifications, analytics
- **Device-specific behavior**: Memory, orientation, backgrounding
- **Release confidence**: Final validation before store submission

## Structure
- `DeviceTestRunner.cs` — Orchestrates install, launch, run, collect
- `DeviceLauncher.cs` — Abstracts adb/xcrun for install and launch
- `LogCollector.cs` — Collects device logs for failure analysis
- `specs/` — BDD-style specifications for device scenarios

## How Device Smoke Would Be Handled

### Android
1. Build APK in CI (Unity Cloud Build or custom)
2. Install via `adb install -r app.apk`
3. Launch via `adb shell am start -n com.company.game/.MainActivity`
4. Run Appium/Maestro tests or image-based UI automation
5. Collect `adb logcat` on failure

### iOS
1. Build IPA in CI
2. Install on simulator: `xcrun simctl install booted app.app`
3. Launch: `xcrun simctl launch booted com.company.game`
4. Run XCTest/Appium tests
5. Collect simulator logs on failure

## Interview Talking Points
- **Risk-based**: Device tests are expensive; we run smoke only on release builds
- **CI integration**: Device tests run after successful build, before artifact promotion
- **Flaky mitigation**: Retries, timeouts, screenshot-on-failure
- **Scalability**: Parallel device pools, cloud device farms (BrowserStack, Sauce Labs, Unity Cloud)
