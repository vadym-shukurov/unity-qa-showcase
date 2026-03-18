using System;
using System.IO;
using UnityEngine;

namespace UnityMobileQA.Tests.TestUtils
{
    /// <summary>
    /// Captures screenshots on Play Mode failure for CI artifact investigation.
    /// Call from [TearDown] or OnTestFailure.
    /// In batchmode/headless, CaptureScreenshot may not produce output; we fail gracefully.
    /// </summary>
    public static class ScreenshotCapture
    {
        public static string OutputDir => Path.Combine(Application.dataPath, "..", "reports", "screenshots");

        public static void CaptureOnFailure(string testName)
        {
            try
            {
                if (!Application.isPlaying) return;
                var dir = OutputDir;
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var safeName = string.Join("_", testName.Split(Path.GetInvalidFileNameChars()));
                var path = Path.Combine(dir, $"{safeName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                ScreenCapture.CaptureScreenshot(path);
            }
            catch (Exception)
            {
                // In batchmode/headless, CaptureScreenshot may not work; avoid failing the teardown
            }
        }
    }
}
