# Security

## Secrets Management

- **Never commit** credentials, API keys, keystores, or tokens.
- Use **GitHub Secrets** for CI: `UNITY_LICENSE`, `ANDROID_KEYSTORE_BASE64`, `ANDROID_KEYSTORE_PASS`, `ANDROID_KEYALIAS_NAME`, `ANDROID_KEYALIAS_PASS`.
- Use **environment variables** for local scripts: `JIRA_TOKEN`, `APK_PATH`, etc.
- See `.gitignore` for excluded patterns: `.env`, `*.keystore`, `*.jks`, `*.p12`.

## Release Signing

- Keystore is base64-encoded and stored as `ANDROID_KEYSTORE_BASE64` secret.
- Passwords never appear in logs or code.
- Release builds run only on version tags (`v*`).
- See [rc-signing.md](rc-signing.md) for setup.

## Supply Chain

- **Dependabot** runs weekly for `package.json` and GitHub Actions.
- **npm audit** runs in CI with `--audit-level=critical`; pipeline **fails** on critical vulns.
- **SBOM** generated for npm dependencies (artifact `npm-sbom`; CycloneDX format).
- `npm overrides` force patched versions (e.g. `@appium/support` ≥ 7.0.6 for Zip Slip fix).
- `package-lock.json` is committed for reproducible installs.
- See `.env.example` for local env vars (JIRA, APK_PATH); never commit `.env`.

### Appium/WebDriverIO Dependencies

Automation toolchain only; no npm deps in shipped APK/IPA. Overrides mitigate known CVEs. If audit fails, update overrides or upgrade packages.

## SAST (Static Analysis)

- **CodeQL** runs on push/PR (`.github/workflows/codeql.yml`). Analyzes JavaScript and C#.
- Fix or triage findings before merge.

## Data Protection

- **SecureStorage**: AES-256 encrypted storage over PlayerPrefs. Key derived via PBKDF2 (100k iterations) from device + app-specific salt (SHA256 of Application.identifier).
- Salt is app-specific (not hardcoded); key names validated (non-empty, ≤128 chars).
- Protects progress, currency, settings from backup extraction and casual file access.
- **deviceUniqueIdentifier**: Deprecated on iOS; may return fixed/empty value. Entropy is sufficient for demo; for production, prefer Android Keystore / iOS Keychain for hardware-backed key storage.
- For hardware-backed keys (Android Keystore, iOS Keychain), replace `SecureStorage` with platform-specific implementation.

## Input Validation

- **Services**: Negative values rejected; overflow prevented (currency, score, level capped).
- **Settings**: Volume clamped 0–1; NaN/Infinity rejected.
- **Scripts**: Path traversal rejected in `collect-device-logs.sh`; JIRA/Slack URLs validated (HTTPS + allowlist) in `report-failure.example.js` and `jira-integration.example.js` (SSRF prevention).

## Artifact Retention

- APK/IPA artifacts: **7 days** retention.
- Test reports: **30 days** retention.
- Reduces exposure of build artifacts.

## CI Permissions

Workflows use least-privilege `permissions:` blocks: `contents: read`, `actions: write` (or `read` where upload not needed), `checks: write` for status reports. No broad `write-all` scope.

## AI Tooling

`.cursorignore` (same patterns as `.gitignore` for secrets) excludes `.env`, `secrets/`, `*.keystore`, `*.p12`, `node_modules/` from AI context. Reduces risk of accidental exposure when using Cursor or similar tools.

## Build Hardening

| Area | Implementation |
|------|----------------|
| **Data storage** | SecureStorage (AES-256 over PlayerPrefs); app-specific salt via SHA256(identifier) |
| **IL2CPP** | Enforced: Edit Mode tests (`UnitySpecificEditModeTests`) + Tools → Validate Build Security |
| **Stripping** | Enable "Strip Engine Code" for release builds |
| **Obfuscation** | IL2CPP + stripping; for production consider Obfuscator-EE or Unity's managed stripping level |
| **Network** | When added: certificate pinning, HTTPS only |
| **Keystore** | Base64 in secret; document rotation; backup in vault |
| **Integrity** | `scripts/verify-apk-signature.sh` verifies APK signature in device farm |

## Reporting Vulnerabilities

Report security issues privately: open an issue with the `security` label or contact the maintainer directly. Do not disclose in public issues.
