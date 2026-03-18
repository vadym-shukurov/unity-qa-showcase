# Accessibility for Appium

**Goal:** Enable robust Appium element lookup via native `contentDescription` (Android) and `accessibilityIdentifier` (iOS).

## Current Implementation

### Android
- **Root view**: `AccessibilityBridge` sets `contentDescription` on the root view at app start (via native plugin). Improves app discovery.
- **Per-element**: Unity 2022 renders to a single view; per-element `contentDescription` requires Unity 6 Accessibility module or [UAP plugin](https://github.com/mikrima/UnityAccessibilityPlugin).
- **Fallback**: `AccessibilityLabel` sets `gameObject.name`; Appium uses `~accessibilityId` with center-tap fallback when IDs unavailable.

### iOS
- **Per-element**: `accessibilityIdentifier` requires Unity 6 or native plugin. Unity 2022 uses single-view rendering.
- **Fallback**: Same as Android — `gameObject.name` + center-tap.

### Upgrade Path
- **Unity 6**: Add `com.unity.modules.accessibility` for native per-element IDs.
- **Unity 2022**: Use [Unity Accessibility Plugin (UAP)](https://github.com/mikrima/UnityAccessibilityPlugin) for full support.

## Files
- `Assets/Scripts/UI/AccessibilityLabel.cs` — per-button ID (gameObject.name)
- `Assets/Scripts/UI/AccessibilityBridge.cs` — C# bridge to native
- `Assets/Plugins/Android/AccessibilityBridge.java` — Android native plugin
