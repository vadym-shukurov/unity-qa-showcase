# Case Study: Unity Mobile QA Automation

*Written as an interview case study. Use to explain: what you automated first, why scope, how scale, release confidence.*

---

## Situation

A mobile Unity game team ships bi-weekly releases to Android and iOS. Manual QA runs a 2-hour smoke suite before each release. Regression escapes have occurred: progress loss, currency bugs, and scene flow breaks. The team wants to automate critical paths and integrate with CI.

## Risks

- **Release confidence**: Manual smoke is slow and can be inconsistent
- **Regression**: New features break existing flows
- **Data integrity**: Save/load, currency, and settings are high-risk
- **Scalability**: Manual testing does not scale with release frequency

## Proposed Solution

### 1. Automation Scope (Risk-Based)

| Priority | Area | Automation Type | Rationale |
|----------|------|-----------------|-----------|
| P0 | Progress, currency, settings | Edit Mode | Fast, deterministic, high value |
| P0 | Scene flow (menu → gameplay → result) | Play Mode | Critical path, smoke |
| P0 | **UI-driven full flow** | Play Mode | Simulates user taps; validates real path |
| P1 | Repeated save/load, scene reload | Play Mode | Stability |
| P1 | FPS, memory, load time, memory leaks | Play Mode | Performance gate |
| P2 | Device install, launch, smoke | Device | Final validation |

### 2. Test Categories

| Category | Scope | Use |
|----------|-------|-----|
| **Smoke** | Critical path, UI flow | Release gate; run first |
| **Regression** | Full flow, settings, stability | PR validation |
| **DataIntegrity** | Progress, currency, settings | Edit Mode; fast feedback |
| **Performance** | FPS, memory, load time | Pre-release check |

### 3. CI Approach

- **Edit Mode**: Run on every push (fast feedback)
- **Play Mode**: Run on PR and main (smoke + regression)
- **Device**: Run on release builds only (after APK/IPA build)
- **Quality gate**: Pipeline fails if smoke tests fail
- **CodeQL**: SAST on push/PR

### 4. Expected Impact

- **Faster feedback**: Edit Mode in &lt; 5 min, Play Mode in &lt; 15 min
- **Release confidence**: Automated smoke before manual sign-off
- **Fewer escapes**: Data integrity and flow covered by automation
- **Scalability**: Add tests by risk, not by coverage target
- **Observability**: Logs, telemetry, dashboard, screenshots on failure

### 5. Leadership Angle

- **Pragmatic coverage**: Automate what matters, not everything
- **Maintainability**: Clean structure, test utilities, avoid flaky patterns
- **Team alignment**: Define ownership (QA vs dev), document strategy
- **Metrics**: Track smoke pass rate, flaky rate, execution time
- **Iterative**: Start with smoke, stabilize, then expand
