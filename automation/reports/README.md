# Test Reports

This folder is used by CI to publish test results.

## Output Structure
- `editmode/` — Edit Mode test results (JUnit XML, Unity format)
- `playmode/` — Play Mode test results
- `screenshots/` — Failure screenshots (Play Mode)
- `logs/` — Player.log, Editor.log on failure

## Failure Logging Strategy
- On Play Mode failure: capture screenshot, attach to artifact
- On device failure: collect adb logcat / simulator logs
- All artifacts uploaded to CI for investigation
