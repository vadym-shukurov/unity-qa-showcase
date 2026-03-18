# CI/CD Configuration

**PRESENTATION:** "Pipeline = release gate. Smoke fails → pipeline fails → no release."
**WHAT FIRST:** Edit Mode → Play Mode → reports. Device when build ready.
**WHY SCOPE:** Fast feedback loop. Quality gate on smoke.
**SCALE:** Add Android/iOS build jobs. Device tests after build. Parallelize.
**RELEASE:** Automated before manual sign-off. Metrics from artifacts.

---

## Pipeline Overview
The QA automation pipeline runs in GitHub Actions (see `.github/workflows/qa-automation.yml`).

## Stages
1. **Validate** — Project structure, lint placeholder
2. **Edit Mode Tests** — Fast unit/integration tests, no runtime
3. **Play Mode Tests** — Runtime tests, scene flow, smoke
4. **Publish Reports** — Artifacts, test summaries
5. **Quality Gate** — Fail pipeline if smoke tests fail

## Where Android/iOS Builds Go
- **Android**: Add `game-ci/unity-builder` job with `targetPlatform: Android`
- **iOS**: Add job on `macos-latest` with `targetPlatform: iOS`
- Device tests run **after** successful build, against the built APK/IPA

## Quality Gate
- Smoke tests are the release gate
- If any smoke test fails → pipeline fails → no artifact promotion
- Flaky tests should be quarantined, not ignored
