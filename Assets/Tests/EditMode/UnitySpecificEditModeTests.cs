#if UNITY_EDITOR
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityMobileQA.Tests.TestUtils;

namespace UnityMobileQA.Tests.EditMode
{
    // PRESENTATION: "Unity-specific — IL2CPP, asset validation, build config."
    // WHY: IL2CPP = mobile default. Asset errors = runtime crashes. Build variants = release confidence.
    // SCALE: Add more asset checks (prefab, scene refs). Validate build settings per platform.
    // RELEASE: Catches config errors before build.
    /// <summary>
    /// Edit Mode: Unity-specific validation — scripting backend, assets, build config.
    /// </summary>
    [TestFixture]
    [Category("BuildConfig")]
    public class UnitySpecificEditModeTests : TestSetupBase
    {
        [Test]
        public void ScriptingBackend_Android_IsIL2CPP()
        {
            var backend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android);
            Assert.AreEqual(ScriptingImplementation.IL2CPP, backend,
                "Android should use IL2CPP for mobile. Set in Player Settings > Other Settings > Scripting Backend.");
        }

        [Test]
        public void ScriptingBackend_iOS_IsIL2CPP()
        {
            var backend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.iOS);
            Assert.AreEqual(ScriptingImplementation.IL2CPP, backend,
                "iOS requires IL2CPP. Set in Player Settings > Other Settings > Scripting Backend.");
        }

        [Test]
        public void BuildSettings_HasRequiredScenes()
        {
            var scenes = EditorBuildSettings.scenes;
            var names = new System.Collections.Generic.HashSet<string>();
            foreach (var s in scenes)
            {
                if (s.enabled && !string.IsNullOrEmpty(s.path))
                    names.Add(Path.GetFileNameWithoutExtension(s.path));
            }
            Assert.IsTrue(names.Contains("Splash"), "Splash scene required");
            Assert.IsTrue(names.Contains("MainMenu"), "MainMenu scene required");
            Assert.IsTrue(names.Contains("Gameplay"), "Gameplay scene required");
        }
    }
}
#endif
