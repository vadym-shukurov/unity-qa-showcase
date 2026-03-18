# Device-Level Smoke Test Specification

**PRESENTATION:** "Device specs — what we'd automate when build pipeline is ready."
**WHAT FIRST:** Install/launch, then guest flow, then background/foreground.
**WHY SCOPE:** Native integrations, real device behavior. Expensive — run on release only.
**RELEASE:** Final validation before store submission.

---

## Scope
These scenarios run on physical devices or emulators after APK/IPA installation.
They validate the critical path that Edit/Play Mode tests cannot cover (e.g., native integrations, device-specific behavior).

## Prerequisites
- Android: APK built, device/emulator connected, `adb` available
- iOS: IPA built, simulator or device, `xcrun` available

## Scenarios

### SMOKE-001: Install and Launch
- **Given** a fresh device/emulator
- **When** APK/IPA is installed and app is launched
- **Then** app opens without crash
- **And** splash/start screen is visible within 10s

### SMOKE-002: Guest Entry / Login Placeholder
- **Given** app is launched
- **When** user taps "Play as Guest" or equivalent
- **Then** main menu is displayed
- **And** no login error is shown

### SMOKE-003: App Background/Foreground
- **Given** app is on main menu
- **When** app is sent to background for 5s
- **And** app is brought to foreground
- **Then** app resumes without crash
- **And** main menu is still visible

### SMOKE-004: Critical Path on Device
- **Given** app is on main menu
- **When** user navigates: Menu → Gameplay → Result → Menu
- **Then** all transitions complete without crash
- **And** reward is reflected (if applicable)

## Implementation Notes
- Use Appium, Maestro, or Unity Cloud Build device testing
- Capture screenshots on failure
- Attach device logs to CI artifacts
