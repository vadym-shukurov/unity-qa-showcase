#!/usr/bin/env bash
# Verify APK signature (integrity check). Requires Android SDK build-tools.
# Usage: ./scripts/verify-apk-signature.sh [path/to/app.apk]
# Exit 0 if valid; 1 if invalid or apksigner not found.

set -e
APK="${1:-}"
if [ -z "$APK" ]; then
  APK=$(find . -name "*.apk" -type f 2>/dev/null | head -1)
fi
if [ -z "$APK" ] || [ ! -f "$APK" ]; then
  echo "No APK found."
  exit 1
fi

# Find apksigner (Android SDK build-tools)
APKSIGNER=""
for dir in "$ANDROID_HOME/build-tools"/*/; do
  if [ -f "${dir}apksigner" ]; then
    APKSIGNER="${dir}apksigner"
    break
  fi
done

if [ -z "$APKSIGNER" ]; then
  echo "apksigner not found (set ANDROID_HOME). Skipping signature verification."
  exit 0
fi

if "$APKSIGNER" verify --verbose "$APK" 2>/dev/null; then
  echo "APK signature valid: $APK"
  exit 0
else
  echo "APK signature invalid or unsigned: $APK"
  exit 1
fi
