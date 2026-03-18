package com.unitymobileqa.accessibility;

import android.app.Activity;
import android.view.View;
import com.unity3d.player.UnityPlayer;

/**
 * Native bridge for accessibility labels. Sets contentDescription on the root view
 * so Appium/UiAutomator can identify the app. For per-element contentDescription,
 * Unity 6 Accessibility module or UAP plugin is required (Unity 2022 renders to single view).
 */
public class AccessibilityBridge {
    /**
     * Set contentDescription on the root view. Call from Unity via AndroidJavaObject.
     * Enables Appium to find the app; per-element IDs require Unity 6.
     */
    public static void setRootContentDescription(String description) {
        try {
            Activity activity = UnityPlayer.currentActivity;
            if (activity != null) {
                View root = activity.getWindow().getDecorView().getRootView();
                if (root != null) {
                    root.setContentDescription(description);
                }
            }
        } catch (Exception ignored) {
            // Silently fail if not on Android or view not ready
        }
    }
}
