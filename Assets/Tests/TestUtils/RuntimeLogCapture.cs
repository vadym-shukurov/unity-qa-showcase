using System;
using System.IO;
using UnityEngine;

namespace UnityMobileQA.Tests.TestUtils
{
    /// <summary>
    /// Captures Application.logMessageReceived during Play Mode tests.
    /// Writes to reports/run.log for failure investigation.
    /// Call Start() in [OneTimeSetUp], Stop() in [OneTimeTearDown].
    /// </summary>
    public static class RuntimeLogCapture
    {
        private static StreamWriter _writer;
        private static readonly object _lock = new object();

        public static string OutputPath => Path.Combine(Application.dataPath, "..", "reports", "run.log");

        public static void Start()
        {
            lock (_lock)
            {
                if (_writer != null) return;
                try
                {
                    var dir = Path.GetDirectoryName(OutputPath);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    _writer = new StreamWriter(OutputPath, append: true) { AutoFlush = true };
                    _writer.WriteLine($"--- Run started {DateTime.UtcNow:o} ---");
                    Application.logMessageReceived += OnLog;
                }
                catch (Exception)
                {
                    // Non-fatal; tests continue without log capture
                }
            }
        }

        public static void Stop()
        {
            lock (_lock)
            {
                Application.logMessageReceived -= OnLog;
                try
                {
                    _writer?.Dispose();
                }
                catch (Exception) { /* ignore */ }
                _writer = null;
            }
        }

        private static void OnLog(string message, string stackTrace, LogType type)
        {
            lock (_lock)
            {
                _writer?.WriteLine($"[{DateTime.UtcNow:HH:mm:ss.fff}] [{type}] {message}");
                if (!string.IsNullOrEmpty(stackTrace) && type == LogType.Exception)
                    _writer?.WriteLine(stackTrace);
            }
        }
    }
}
