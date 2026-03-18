#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityMobileQA.Core;
using UnityMobileQA.Gameplay;
using UnityMobileQA.UI;

namespace UnityMobileQA.Editor
{
    /// <summary>
    /// Creates required scenes with controllers, UI buttons, and adds them to Build Settings on first load.
    /// Ensures Play Mode tests and device full-flow (coordinate tap) work without manual setup.
    /// </summary>
    [InitializeOnLoad]
    public static class SceneSetupOnLoad
    {
        private const string SetupDoneKey = "UnityMobileQA_SceneSetupDone";
        private const int SetupVersion = 4; // Bump when controller/UI setup changes
        private static readonly string[] SceneNames = { "Splash", "MainMenu", "Settings", "Gameplay", "Result" };

        static SceneSetupOnLoad()
        {
            EditorApplication.delayCall += EnsureScenesExist;
        }

        private static void EnsureScenesExist()
        {
            if (EditorPrefs.GetInt(SetupDoneKey, 0) >= SetupVersion)
                return;

            var scenePaths = new System.Collections.Generic.List<string>();
            var scenesDir = "Assets/Scenes";

            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
                AssetDatabase.CreateFolder("Assets", "Scenes");

            if (EditorPrefs.GetInt(SetupDoneKey, 0) > 0)
            {
                foreach (var name in SceneNames)
                {
                    var path = $"{scenesDir}/{name}.unity";
                    if (System.IO.File.Exists(path))
                        AssetDatabase.DeleteAsset(path);
                }
            }

            foreach (var name in SceneNames)
            {
                var path = $"{scenesDir}/{name}.unity";
                if (!System.IO.File.Exists(path))
                {
                    var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                    AddControllersAndUI(name, scene);
                    EditorSceneManager.SaveScene(scene, path);
                }
                scenePaths.Add(path);
            }

            var buildScenes = new EditorBuildSettingsScene[scenePaths.Count];
            for (int i = 0; i < scenePaths.Count; i++)
                buildScenes[i] = new EditorBuildSettingsScene(scenePaths[i], true);
            EditorBuildSettings.scenes = buildScenes;

            EditorPrefs.SetInt(SetupDoneKey, SetupVersion);
            Debug.Log("[UnityMobileQA] Scenes created with controllers and UI, added to Build Settings.");
        }

        private static void AddControllersAndUI(string sceneName, UnityEditor.SceneManagement.Scene scene)
        {
            switch (sceneName)
            {
                case "MainMenu":
                    new GameObject("ServiceLocator").AddComponent<ServiceLocator>();
                    var menuCtrl = new GameObject("MainMenuController").AddComponent<MainMenuController>();
                    CreateButtonRow(new[] {
                        ("Play", "btn_play", menuCtrl.OnPlayClicked),
                        ("Settings", "btn_settings", menuCtrl.OnSettingsClicked)
                    });
                    break;
                case "Gameplay":
                    var gameplayCtrl = new GameObject("GameplayController").AddComponent<GameplayController>();
                    CreateCenteredButton("Complete", "btn_simulate_complete", () => gameplayCtrl.SimulateCompletion(100));
                    break;
                case "Result":
                    var resultCtrl = new GameObject("ResultScreenController").AddComponent<ResultScreenController>();
                    CreateCenteredButton("Back to Menu", "btn_back_to_menu", resultCtrl.OnBackToMenuClicked);
                    break;
                case "Splash":
                    var splashCtrl = new GameObject("SplashController").AddComponent<SplashController>();
                    CreateCenteredButton("Start", "btn_start", splashCtrl.OnStartClicked);
                    break;
                case "Settings":
                    var settingsCtrl = new GameObject("SettingsController").AddComponent<SettingsController>();
                    CreateCenteredButton("Back", "btn_back", settingsCtrl.OnBackClicked);
                    break;
            }
        }

        private static void CreateButtonRow((string label, string accessibilityId, System.Action onClick)[] buttons)
        {
            var canvasObj = new GameObject("Canvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObj.AddComponent<GraphicRaycaster>();

            var es = Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
            if (es == null)
            {
                var esObj = new GameObject("EventSystem");
                esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            var panel = new GameObject("Panel");
            panel.transform.SetParent(canvasObj.transform, false);
            var panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.offsetMin = panelRect.offsetMax = Vector2.zero;
            panel.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.15f, 0.9f);

            float spacing = 100f;
            float totalWidth = (buttons.Length - 1) * spacing + buttons.Length * 300;
            float startX = -totalWidth / 2f + 150;
            for (int i = 0; i < buttons.Length; i++)
            {
                var (label, accessibilityId, onClick) = buttons[i];
                float x = startX + i * (300 + spacing);
                CreateButtonInPanel(panel.transform, label, accessibilityId, new Vector2(x, 0), onClick);
            }
        }

        private static void CreateCenteredButton(string label, string accessibilityId, System.Action onClick)
        {
            var canvasObj = new GameObject("Canvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObj.AddComponent<GraphicRaycaster>();

            var es = Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
            if (es == null)
            {
                var esObj = new GameObject("EventSystem");
                esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            var panel = new GameObject("Panel");
            panel.transform.SetParent(canvasObj.transform, false);
            var panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.offsetMin = panelRect.offsetMax = Vector2.zero;
            panel.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.15f, 0.9f);

            var btnObj = new GameObject(accessibilityId);
            btnObj.transform.SetParent(panel.transform, false);
            var btnRect = btnObj.AddComponent<RectTransform>();
            btnRect.anchorMin = btnRect.anchorMax = new Vector2(0.5f, 0.5f);
            btnRect.pivot = new Vector2(0.5f, 0.5f);
            btnRect.sizeDelta = new Vector2(300, 80);
            btnRect.anchoredPosition = Vector2.zero;

            var img = btnObj.AddComponent<Image>();
            img.color = new Color(0.2f, 0.5f, 0.8f);
            var btn = btnObj.AddComponent<Button>();
            btn.targetGraphic = img;
            var a11y = btnObj.AddComponent<AccessibilityLabel>();
            a11y.accessibilityId = accessibilityId;

            var textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform, false);
            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = textRect.offsetMax = Vector2.zero;
            var text = textObj.AddComponent<Text>();
            text.text = label;
            text.font = Font.CreateDynamicFontFromOSFont("Arial", 28);
            text.fontSize = 28;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;

            btn.onClick.AddListener(() => onClick?.Invoke());
        }

        private static void CreateButtonInPanel(Transform panel, string label, string accessibilityId, Vector2 position, System.Action onClick)
        {
            var btnObj = new GameObject(accessibilityId);
            btnObj.transform.SetParent(panel, false);
            var btnRect = btnObj.AddComponent<RectTransform>();
            btnRect.anchorMin = btnRect.anchorMax = new Vector2(0.5f, 0.5f);
            btnRect.pivot = new Vector2(0.5f, 0.5f);
            btnRect.sizeDelta = new Vector2(300, 80);
            btnRect.anchoredPosition = position;

            var img = btnObj.AddComponent<Image>();
            img.color = new Color(0.2f, 0.5f, 0.8f);
            var btn = btnObj.AddComponent<Button>();
            btn.targetGraphic = img;
            var a11y = btnObj.AddComponent<AccessibilityLabel>();
            a11y.accessibilityId = accessibilityId;

            var textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform, false);
            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = textRect.offsetMax = Vector2.zero;
            var text = textObj.AddComponent<Text>();
            text.text = label;
            text.font = Font.CreateDynamicFontFromOSFont("Arial", 28);
            text.fontSize = 28;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;

            btn.onClick.AddListener(() => onClick?.Invoke());
        }
    }
}
#endif
