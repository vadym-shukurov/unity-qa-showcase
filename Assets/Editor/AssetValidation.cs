#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace UnityMobileQA.Editor
{
    /// <summary>
    /// Asset validation and build security checks for release confidence.
    /// Run: Tools > Unity Mobile QA > Validate Assets | Validate Build Security
    /// </summary>
    public static class AssetValidation
    {
        [MenuItem("Tools/Unity Mobile QA/Validate Build Security")]
        public static void ValidateBuildSecurity()
        {
            var errors = 0;
            if (PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android) != ScriptingImplementation.IL2CPP)
            {
                Debug.LogError("[BuildSecurity] Android must use IL2CPP. Edit → Project Settings → Player → Other → Scripting Backend.");
                errors++;
            }
            if (PlayerSettings.GetScriptingBackend(BuildTargetGroup.iOS) != ScriptingImplementation.IL2CPP)
            {
                Debug.LogError("[BuildSecurity] iOS must use IL2CPP. Edit → Project Settings → Player → Other → Scripting Backend.");
                errors++;
            }
            if (errors == 0)
                Debug.Log("[BuildSecurity] IL2CPP configured for Android and iOS.");
        }

        [MenuItem("Tools/Unity Mobile QA/Validate Assets")]
        public static void ValidateAssets()
        {
            var errors = 0;
            var guids = AssetDatabase.FindAssets("t:Script", new[] { "Assets/Scripts" });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                if (obj == null)
                {
                    Debug.LogError($"[AssetValidation] Missing asset: {path}");
                    errors++;
                }
            }
            if (errors == 0)
                Debug.Log("[AssetValidation] All assets valid.");
            else
                Debug.LogError($"[AssetValidation] {errors} asset(s) failed validation.");
        }
    }
}
#endif
