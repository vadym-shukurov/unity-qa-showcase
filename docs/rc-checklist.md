# Release Checklist

Pre-release validation gate. Automation runs these, manual sign-off follows.

## Automated (CI)

1. **Edit Mode** — All logic tests pass
2. **Play Mode** — Smoke + regression + stability pass
3. **Android Build** — APK builds, signed with release keystore
4. **iOS Build** — IPA builds (macOS runner)
5. **iOS Simulator** — .app builds for device farm (when configured)
6. **Metrics** — Smoke pass rate ≥ 99%, no new flaky
7. **Device Farm** — Smoke on matrix of devices (when configured)

## Notes

- **Full-flow**: Android root contentDescription via AccessibilityBridge; per-element requires Unity 6 (see docs/accessibility.md). Center tap fallback when IDs unavailable.
- **iOS device farm**: Fails on test failure when build exists; skips when no iOS build. `ios-simulator-app` artifact when available.
- **Sample app**: Intentionally minimal (Splash → Menu → Settings | Gameplay → Result); sufficient for automation showcase.

## Security Checklist

1. **Secrets** — No credentials in code; keystore in GitHub Secrets
2. **Signing** — Release builds use release keystore (see [rc-signing.md](rc-signing.md))
3. **Supply chain** — npm audit passes (no critical vulns); Dependabot PRs merged
4. **Artifacts** — APK/IPA retention 7 days; no long-term exposure
5. **SAST** — CodeQL (JavaScript + C#) passes; no new security findings
6. **Storage** — SecureStorage (AES-256, app-specific salt) for progress, currency, settings
7. **SBOM** — npm-sbom artifact generated; APK signature verified in device farm
8. **IL2CPP** — Enforced by Edit Mode tests + Tools → Validate Build Security (Edit → Project Settings → Player)

## Manual (Post-Automation)

1. **Exploratory** — Key flows manually verified
2. **Store compliance** — Privacy policy, age rating, content
3. **Release notes** — Changelog updated
4. **Stakeholder sign-off** — PM/Lead approval

## Rollback Criteria

- Smoke pass rate < 95%
- Critical regression in data integrity
- Crash on launch (device smoke)
