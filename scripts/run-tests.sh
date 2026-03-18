#!/bin/bash
# Run Unity tests from command line. Requires Unity 2022.3+ installed.
# Usage: ./scripts/run-tests.sh [editmode|playmode|all]
# Proof of execution: outputs reports/ with JUnit XML.
# Cross-platform: Set UNITY_PATH or ensure Unity is in PATH.

set -e
MODE="${1:-all}"
PROJECT_PATH="$(cd "$(dirname "$0")/.." && pwd)"

# Cross-platform Unity path detection
find_unity_path() {
  if [ -n "$UNITY_PATH" ] && [ -f "$UNITY_PATH" ]; then
    echo "$UNITY_PATH"
    return
  fi
  if command -v Unity >/dev/null 2>&1; then
    command -v Unity
    return
  fi
  case "$(uname -s)" in
    Darwin)
      if [ -d "/Applications/Unity/Hub/Editor" ]; then
        latest=$(ls -1d /Applications/Unity/Hub/Editor/*/ 2>/dev/null | tail -1)
        if [ -n "$latest" ] && [ -f "${latest}Unity.app/Contents/MacOS/Unity" ]; then
          echo "${latest}Unity.app/Contents/MacOS/Unity"
          return
        fi
      fi
      if [ -f "/Applications/Unity/Unity.app/Contents/MacOS/Unity" ]; then
        echo "/Applications/Unity/Unity.app/Contents/MacOS/Unity"
        return
      fi
      ;;
    Linux)
      if [ -f /opt/Unity/Editor/Unity ]; then
        echo /opt/Unity/Editor/Unity
        return
      fi
      if [ -d "$HOME/Unity/Hub/Editor" ]; then
        latest=$(ls -1d "$HOME"/Unity/Hub/Editor/*/ 2>/dev/null | tail -1)
        if [ -n "$latest" ] && [ -f "${latest}Editor/Unity" ]; then
          echo "${latest}Editor/Unity"
          return
        fi
      fi
      ;;
    MINGW*|MSYS*|CYGWIN*)
      if [ -d "C:/Program Files/Unity/Hub/Editor" ]; then
        for dir in "C:/Program Files/Unity/Hub/Editor/"*/; do
          if [ -f "${dir}Editor/Unity.exe" ]; then
            echo "${dir}Editor/Unity.exe"
            return
          fi
        done
      fi
      if [ -f "C:/Program Files (x86)/Unity/Editor/Unity.exe" ]; then
        echo "C:/Program Files (x86)/Unity/Editor/Unity.exe"
        return
      fi
      ;;
  esac
  echo ""
}

UNITY_PATH="$(find_unity_path)"
if [ -z "$UNITY_PATH" ]; then
  echo "Unity not found. Set UNITY_PATH or install Unity 2022.3."
  echo "  Mac:   UNITY_PATH=/Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/MacOS/Unity"
  echo "  Linux: UNITY_PATH=/opt/Unity/Editor/Unity"
  echo "  Win:   UNITY_PATH=\"C:/Program Files/Unity/Hub/Editor/2022.3.0f1/Editor/Unity.exe\""
  exit 1
fi

mkdir -p "$PROJECT_PATH/reports"

run_editmode() {
  echo "Running Edit Mode tests..."
  "$UNITY_PATH" -batchmode -nographics -projectPath "$PROJECT_PATH" \
    -runTests -testPlatform EditMode \
    -testResults "$PROJECT_PATH/reports/editmode-results.xml" \
    -logFile "$PROJECT_PATH/reports/editmode.log"
}

run_playmode() {
  echo "Running Play Mode tests..."
  "$UNITY_PATH" -batchmode -nographics -projectPath "$PROJECT_PATH" \
    -runTests -testPlatform PlayMode \
    -testResults "$PROJECT_PATH/reports/playmode-results.xml" \
    -logFile "$PROJECT_PATH/reports/playmode.log"
}

case "$MODE" in
  editmode) run_editmode ;;
  playmode) run_playmode ;;
  all)
    run_editmode
    run_playmode
    ;;
  *)
    echo "Usage: $0 [editmode|playmode|all]"
    exit 1
    ;;
esac

echo "Tests complete. Results in $PROJECT_PATH/reports/"
