# Release Signing Setup

For store-ready Android builds, add these GitHub secrets:

| Secret | Description |
|--------|-------------|
| `ANDROID_KEYSTORE_BASE64` | Base64-encoded keystore: `openssl base64 -A -in release.keystore` |
| `ANDROID_KEYSTORE_PASS` | Keystore password |
| `ANDROID_KEYALIAS_NAME` | Key alias (e.g. `upload`) |
| `ANDROID_KEYALIAS_PASS` | Key alias password |

Without these, the build uses debug signing (fine for testing).

**Notes:**
- GitHub Secrets have size limits. For large keystores, consider fetching from a secure store at build time.
- **Backup:** Store keystore in a secure vault (e.g. 1Password, HashiCorp Vault). Loss = cannot update app on store.
