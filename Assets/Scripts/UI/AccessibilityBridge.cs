using System;
using UnityEngine;

namespace UnityMobileQA.UI
{
    /// <summary>
    /// Bridges Unity AccessibilityLabel to native contentDescription (Android) / accessibilityIdentifier (iOS).
    /// On Android: Sets root view contentDescription via native plugin. Per-element requires Unity 6 Accessibility module.
    /// On iOS: accessibilityIdentifier requires Unity 6 or native plugin; gameObject.name used as fallback.
    /// </summary>
    public static class AccessibilityBridge
    {
        private const string AndroidBridgeClass = "com.unitymobileqa.accessibility.AccessibilityBridge";

        /// <summary>Call at app start to set root contentDescription (Android). Improves Appium discovery.</summary>
        public static void SetRootContentDescription(string description)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using (var bridge = new AndroidJavaClass(AndroidBridgeClass))
                {
                    bridge.CallStatic("setRootContentDescription", description ?? "UnityMobileQA");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[AccessibilityBridge] setRootContentDescription failed: {e.Message}");
            }
#endif
        }
    }
}
