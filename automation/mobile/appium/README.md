# Appium Device Tests

Real device-level automation using Appium + WebdriverIO.

## Prerequisites
- Node.js 18+
- Appium 2.x (`npm install -g appium`)
- **Android**: SDK, emulator or device, APK built from Unity
- **iOS**: Xcode, simulator or device, .app/.ipa (macOS only)

## Setup
```bash
npm install
```

## Run

### Android
```bash
export APK_PATH=/path/to/your/app.apk
npx wdio run wdio.android.conf.js
# Full flow (requires accessibility IDs): npx wdio run wdio.android.conf.js --spec specs/full-flow.android.js
```

### iOS
```bash
export IPA_PATH=/path/to/your/app.app  # or .ipa for device
npx wdio run wdio.ios.conf.js
```

## Specs
| Spec | Platform | Description |
|------|----------|-------------|
| `smoke.android.js` | Android | Launch, back button, resilience (explicit waits) |
| `smoke.ios.js` | iOS | Launch, back gesture, resilience (explicit waits) |
| `full-flow.android.js` | Android | Menu → gameplay → result + Settings flow (POM, explicit waits) |
| `full-flow.ios.js` | iOS | Menu → gameplay → result + Settings flow (POM, explicit waits) |

## Structure
- `pages/AppPage.js` — Page Object Model; centralizes selectors and taps
- `helpers/waitHelper.js` — Explicit waits (waitForAppReady, waitForTransition)
- **JUnit reporter** — Outputs to `reports/junit/`; `scripts/device-metrics.js` parses for dashboard
