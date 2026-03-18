#!/usr/bin/env bash
# Collect device logs for observability.
# Android: adb logcat
# iOS: xcrun simctl spawn booted log stream (simulator) or device logs
# Usage: ./scripts/collect-device-logs.sh [android|ios] [output_dir]
# Output: output_dir/device.log

set -e
PLATFORM="${1:-android}"
OUT_DIR="${2:-reports}"

# Prevent path traversal
if [[ "$OUT_DIR" == *".."* ]]; then
  echo "Invalid output path (path traversal rejected)."; exit 1
fi
mkdir -p "$OUT_DIR"
OUT="$OUT_DIR/device.log"

case "$PLATFORM" in
  android)
    if command -v adb &>/dev/null; then
      adb logcat -d -t 5000 > "$OUT" 2>/dev/null || true
      echo "Collected Android logs to $OUT"
    else
      echo "adb not found. Install Android SDK."
      exit 1
    fi
    ;;
  ios)
    if command -v xcrun &>/dev/null; then
      xcrun simctl spawn booted log stream --predicate 'processImagePath contains "Unity"' 2>/dev/null | head -5000 > "$OUT" || true
      echo "Collected iOS simulator logs to $OUT"
    else
      echo "xcrun not found. Run on macOS with Xcode."
      exit 1
    fi
    ;;
  *)
    echo "Usage: $0 [android|ios] [output_dir]"
    exit 1
    ;;
esac
