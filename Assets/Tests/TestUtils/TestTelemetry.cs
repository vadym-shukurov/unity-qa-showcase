using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UnityMobileQA.Tests.TestUtils
{
    /// <summary>
    /// Emits performance metrics from tests to reports/telemetry.json.
    /// Used for load time trends, FPS regression, memory growth observability.
    /// </summary>
    public static class TestTelemetry
    {
        private static readonly List<object> _events = new List<object>();
        private static readonly object _lock = new object();

        public static string OutputPath => Path.Combine(Application.dataPath, "..", "reports", "telemetry.json");

        /// <summary>Record a scalar metric (e.g. load time, FPS, memory MB).</summary>
        public static void Record(string name, double value, string unit = null)
        {
            lock (_lock)
            {
                var evt = new Dictionary<string, object>
                {
                    ["ts"] = DateTime.UtcNow.ToString("o"),
                    ["name"] = name,
                    ["value"] = value
                };
                if (!string.IsNullOrEmpty(unit)) evt["unit"] = unit;
                _events.Add(evt);
            }
        }

        /// <summary>Record a named duration in seconds.</summary>
        public static void RecordDuration(string name, double seconds) => Record(name, seconds, "s");

        /// <summary>Record FPS.</summary>
        public static void RecordFps(string name, double fps) => Record(name, fps, "fps");

        /// <summary>Record memory in MB.</summary>
        public static void RecordMemoryMb(string name, double mb) => Record(name, mb, "MB");

        /// <summary>Flush all recorded events to telemetry.json.</summary>
        public static void Flush()
        {
            lock (_lock)
            {
                if (_events.Count == 0) return;
                try
                {
                    var dir = Path.GetDirectoryName(OutputPath);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    var lines = new List<string>
                    {
                        "{",
                        $"  \"timestamp\": \"{DateTime.UtcNow:o}\",",
                        "  \"events\": ["
                    };
                    for (int i = 0; i < _events.Count; i++)
                    {
                        var evt = (Dictionary<string, object>)_events[i];
                        var name = ((string)evt["name"]).Replace("\\", "\\\\").Replace("\"", "\\\"");
                        var unit = evt.ContainsKey("unit") ? $", \"unit\": \"{evt["unit"]}\"" : "";
                        lines.Add($"    {{\"ts\": \"{evt["ts"]}\", \"name\": \"{name}\", \"value\": {evt["value"]}{unit}}}" +
                            (i < _events.Count - 1 ? "," : ""));
                    }
                    lines.Add("  ]");
                    lines.Add("}");
                    File.WriteAllText(OutputPath, string.Join("\n", lines));
                    _events.Clear();
                }
                catch (Exception)
                {
                    // Avoid failing tests if write fails (e.g. CI permissions)
                }
            }
        }
    }
}
