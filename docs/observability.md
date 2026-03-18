# Observability

**PRESENTATION:** "Logs, metrics, traces — we see what's happening. Failures are debuggable."
**RELEASE CONFIDENCE:** Observability = faster diagnosis, fewer blind spots.

---

## Overview

| Pillar | Tool | Output | Use |
|--------|------|--------|-----|
| **Logs** | RuntimeLogCapture, LogCollector | `reports/run.log`, device logs | Failure investigation, crash diagnosis |
| **Metrics** | generate-metrics.js, TestTelemetry | `reports/metrics.json`, `telemetry.json` | Pass rate, duration, FPS, load times |
| **Device metrics** | device-metrics.js (JUnit XML) | `reports/device-metrics.json` | Appium pass/fail when device farm runs |
| **Traces** | NUnit XML, CI artifacts | TestResults/*.xml | Test-level timing, retries |
| **Artifacts** | ScreenshotCapture, CI upload | `reports/screenshots/`, workflow artifacts | Visual debugging on failure |

## Logs

### In-Editor / Play Mode
- **RuntimeLogCapture**: Subscribes to `Application.logMessageReceived` during Play Mode tests.
- Writes to `reports/run.log` (append per run).
- Captures Log, Warning, Error with timestamps.

### Device / CI
- **LogCollector** (automation): `scripts/collect-device-logs.sh [android|ios] [output_dir]` — `adb logcat` (Android), `xcrun simctl` (iOS).
- CI uploads `Player.log`, `Editor.log` when available (game-ci/unity-test-runner).
- Device tests: run `./scripts/collect-device-logs.sh android` before/after run; attach `reports/device.log` to artifacts.

## Metrics

### Test Results
- **generate-metrics.js**: Parses NUnit XML → `metrics.json`, `dashboard.html`, `metrics-history.json`.
- Tracks: passed/failed/skipped, duration, smoke pass rate, flaky rate.

### Performance Telemetry
- **TestTelemetry**: Tests emit custom metrics (load time, FPS, memory).
- Written to `reports/telemetry.json` (merged into dashboard when present).
- Use for: load time trends, FPS regression, memory growth.
- **Memory leak tests**: See [memory-leaks.md](memory-leaks.md).

## Artifacts

| Artifact | Path | When |
|----------|------|------|
| Screenshots | `reports/screenshots/` | Play Mode failure (TearDown) |
| Telemetry | `reports/telemetry.json` | After performance tests |
| Run log | `reports/run.log` | During Play Mode |
| Metrics | `reports/metrics.json` | After `generate-metrics.js` |
| Dashboard | `reports/dashboard.html` | After `generate-metrics.js` |
| Device metrics | `reports/device-metrics.json` | After device farm (JUnit XML parsed) |

## CI Integration

- **Upload**: `editmode-reports`, `playmode-reports`, `qa-dashboard` (includes metrics, dashboard, telemetry).
- **Screenshots**: Include `reports/screenshots/` in playmode-reports when failures occur.
- **Retention**: 30 days for reports, 7 days for builds.

## Failure Reporting (Centralized)

Integrate with JIRA, Slack, or webhooks to notify on test failures.

### Slack Webhook (CI)
Add to quality-gate job `if: failure()`:
```yaml
- name: Notify Slack on failure
  if: failure()
  run: |
    curl -X POST -H 'Content-type: application/json' \
      --data '{"text":"QA pipeline failed: ${{ github.server_url }}/${{ github.repository }}/actions/runs/${{ github.run_id }}"}' \
      "${{ secrets.SLACK_WEBHOOK_URL }}"
```

### JIRA (Example)
`scripts/report-failure.example.js` — parses NUnit XML, creates JIRA ticket. Set `JIRA_URL`, `JIRA_TOKEN` in secrets. **SSRF prevention**: JIRA and Slack URLs validated (HTTPS + allowlist) before requests.

### Production (Future)

- Crash reporting: Unity Crashlytics, Sentry, or platform-native.
- Analytics: Custom events for flow completion, errors.
- Remote config: Feature flags, A/B for QA experiments.
