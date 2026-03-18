# Device Full-Flow Tests

**Goal:** Run Splash → Menu → Gameplay → Result → Menu on real device/emulator via Appium.

## Current Setup

- **AccessibilityLabel** on each button: `btn_start`, `btn_play`, `btn_settings`, `btn_simulate_complete`, `btn_back`, `btn_back_to_menu`
- **Android**: `AccessibilityBridge` sets root `contentDescription`; per-element requires Unity 6 (see [accessibility.md](accessibility.md))
- **Page Object Model**: `pages/AppPage.js` centralizes selectors and taps
- **Explicit waits**: `waitForAppReady`, `waitForTransition` replace fixed `browser.pause()`
- Full-flow tries `~accessibilityId` first; falls back to center tap. Runs in CI with smoke.

## Optional: Native contentDescription (Android)

For more robust Appium lookup, add native `contentDescription` via:
- Post-build script to inject into the view hierarchy
- Third-party Unity UI Accessibility package
- `AndroidJavaObject` in a MonoBehaviour to call `View.setContentDescription()` (requires view reference)

## Optional: accessibilityIdentifier (iOS)

Set via `UIAccessibilityIdentification` or extend Unity's `Selectable` for XCUITest.

## Running Full-Flow Tests

```bash
cd automation/mobile/appium
APK_PATH=/path/to/app.apk npx wdio run wdio.android.conf.js --spec specs/full-flow.android.js
```

For iOS:

```bash
IPA_PATH=/path/to/app.app npx wdio run wdio.ios.conf.js --spec specs/full-flow.ios.js
```

## Spec Structure

`specs/full-flow.android.js` and `specs/full-flow.ios.js` — try accessibility IDs first, fall back to center tap.
