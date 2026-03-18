#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace UnityMobileQA.Editor
{
    /// <summary>
    /// Custom build for iOS Simulator (.app). Use with game-ci: buildMethod: UnityMobileQA.Editor.BuildIOSSimulator.Build
    /// Sets sdkVersion to SimulatorSDK so output is .app for device farm (Firebase Test Lab, local simulator).
    /// </summary>
    public static class BuildIOSSimulator
    {
        public static void Build()
        {
            var options = ParseCommandLineArgs();
            if (!options.TryGetValue("customBuildPath", out string buildPath))
            {
                buildPath = Path.Combine("build", "iOSSimulator", "UnityMobileQA");
            }

            var originalSdk = PlayerSettings.iOS.sdkVersion;
            var originalArch = PlayerSettings.iOS.simulatorSdkArchitecture;
            try
            {
                PlayerSettings.iOS.sdkVersion = iOSSdkVersion.SimulatorSDK;
                PlayerSettings.iOS.simulatorSdkArchitecture = AppleMobileArchitectureSimulator.ARM64;

                var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
                var buildOptions = new BuildPlayerOptions
                {
                    scenes = scenes,
                    target = BuildTarget.iOS,
                    locationPathName = buildPath
                };

                var report = BuildPipeline.BuildPlayer(buildOptions);
                var summary = report.summary;
                Debug.Log($"[BuildIOSSimulator] Result: {summary.result}, Duration: {summary.totalTime}");
                EditorApplication.Exit(summary.result == BuildResult.Succeeded ? 0 : 101);
            }
            finally
            {
                PlayerSettings.iOS.sdkVersion = originalSdk;
                PlayerSettings.iOS.simulatorSdkArchitecture = originalArch;
            }
        }

        private static Dictionary<string, string> ParseCommandLineArgs()
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("-") && i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                {
                    result[args[i].TrimStart('-')] = args[i + 1];
                    i++;
                }
            }
            return result;
        }
    }
}
#endif
