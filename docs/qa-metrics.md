# QA Metrics and KPIs

**PRESENTATION:** "Metrics drive release decisions. Smoke pass rate = release gate. Flaky rate = fix before adding more."
**RELEASE CONFIDENCE:** These KPIs tell us when we're safe to ship.

---

Suggested metrics for a Unity mobile QA automation program.

## Execution Metrics

| Metric | Target | Notes |
|--------|--------|------|
| **Smoke pass rate** | ≥ 99% | Critical path must be green |
| **Flaky rate** | < 2% | Quarantine flaky tests, fix or remove |
| **Execution duration** | Edit < 5 min, Play < 15 min | Keep feedback loop fast |
| **CI green rate** | ≥ 95% | Indicates stable automation |
| **Device pass rate** | Track when device farm runs | From `reports/device-metrics.json` (JUnit XML) |

## Quality Metrics

| Metric | Target | Notes |
|--------|--------|------|
| **Escaped defects** | Track and trend | Bugs found in production |
| **Crash-free rate** | ≥ 99.5% | From analytics/crash reporting |
| **Regression rate** | Decrease over time | Fixes that re-break |

## Process Metrics

| Metric | Target | Notes |
|--------|--------|------|
| **Automation coverage** | Risk-based, not 100% | High-risk flows first |
| **Time to feedback** | < 30 min | From commit to test result |
| **Maintenance ratio** | < 20% | Time fixing vs writing new tests |

## Leadership Angle
- Metrics inform **where to invest** (flaky tests, slow suites)
- Release gates based on **smoke + critical regression**
- Pragmatic coverage: automate what **matters**, not everything
