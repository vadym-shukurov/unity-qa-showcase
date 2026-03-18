# Sample Test Report Summary

**PRESENTATION:** "This is what we'd see before sign-off. Smoke green = release candidate."
**RELEASE CONFIDENCE:** Automated smoke + manual exploratory = confident release.

---

**Run Date**: 2025-03-18  
**Pipeline**: QA Automation  
**Branch**: main

## Results Overview

| Suite        | Passed | Failed | Skipped | Duration |
|-------------|--------|--------|---------|----------|
| Edit Mode   | 15     | 0      | 0       | 2.3s     |
| Play Mode   | 8      | 0      | 0       | 12.1s    |
| **Total**   | **23** | **0**  | **0**   | **14.4s**|

## Smoke Tests
- ✅ App flow reaches main menu
- ✅ Main menu → Gameplay transition
- ✅ Gameplay → Result transition
- ✅ Result → Menu return
- ✅ Full flow completes without error

## Failure Logging Strategy
- On failure: capture screenshot (Play Mode), attach to artifact
- Logs: `Player.log`, `Editor.log` uploaded to CI artifacts
- Device tests: `adb logcat` or simulator logs attached

## Notes
- Play Mode tests require scenes in Build Settings
- Device-level tests run only on release builds
