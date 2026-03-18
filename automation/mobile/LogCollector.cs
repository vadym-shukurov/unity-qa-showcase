// Log collection from device for observability.
// Implementation: scripts/collect-device-logs.sh (adb logcat, xcrun simctl).
// Used for: failure investigation, crash-free rate metrics, flaky test diagnosis.

namespace UnityMobileQA.Automation.Mobile
{
    /// <summary>
    /// Interface for device log collection. Actual collection is done by
    /// scripts/collect-device-logs.sh (Android: adb logcat, iOS: xcrun simctl).
    /// </summary>
    public static class LogCollector
    {
        /// <summary>Output path for collected logs (matches script default).</summary>
        public const string DefaultOutputPath = "reports/device.log";

        /// <summary>Run: ./scripts/collect-device-logs.sh [platform] [output_dir]</summary>
        public static string GetCollectCommand(string platform = "android", string outputDir = "reports") =>
            $"scripts/collect-device-logs.sh {platform} {outputDir}";
    }
}
