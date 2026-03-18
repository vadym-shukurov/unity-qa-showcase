using UnityEngine;
using UnityEngine.UI;

namespace UnityMobileQA.UI
{
    /// <summary>
    /// Stores accessibility ID for Appium. Sets GameObject name for hierarchy consistency.
    /// Android: AccessibilityBridge sets root contentDescription; per-element requires Unity 6.
    /// iOS: accessibilityIdentifier requires Unity 6; gameObject.name used as fallback.
    /// Full-flow: tries ~accessibilityId first, falls back to center tap. See docs/ACCESSIBILITY.md.
    /// </summary>
    [RequireComponent(typeof(Selectable))]
    public class AccessibilityLabel : MonoBehaviour
    {
        [Tooltip("Appium selector: btn_play, btn_simulate_complete, btn_back_to_menu, btn_start")]
        public string accessibilityId;

        private void Awake()
        {
            if (!string.IsNullOrEmpty(accessibilityId))
                gameObject.name = accessibilityId;
        }
    }
}
