using UnityEngine;
using UnityEditor;
using System.IO;

namespace AcousticsLite
{
    [InitializeOnLoad]
    public class AcousticsWelcomeWindow : EditorWindow
    {
        private static readonly string PREF_KEY = "AcousticsLite_WelcomeShown_v1.0";
        private static readonly string PREF_SHOW_AT_STARTUP = "AcousticsLite_ShowAtStartup";

        // --- LOGO VARIABLE ---
        private Texture2D _logo;

        static AcousticsWelcomeWindow()
        {
            EditorApplication.delayCall += ShowAtStartup;
        }

        private static void ShowAtStartup()
        {
            bool isFirstTime = !EditorPrefs.HasKey(PREF_KEY);
            bool showAtStartup = EditorPrefs.GetBool(PREF_SHOW_AT_STARTUP, true);

            if (isFirstTime || showAtStartup)
            {
                OpenWindow();
                if (isFirstTime)
                {
                    EditorPrefs.SetBool(PREF_KEY, true);
                }
            }
        }

        [MenuItem("Tools/Acoustics Lite/Help Window", false, 100)]
        public static void OpenWindow()
        {
            AcousticsWelcomeWindow window = GetWindow<AcousticsWelcomeWindow>(true, "Welcome to Acoustics Lite", true);
            window.minSize = new Vector2(400, 550);
            window.Show();
        }

        private void OnEnable()
        {
            LoadLogo();
        }

        private void LoadLogo()
        {
            if (_logo == null)
            {
                string[] guids = AssetDatabase.FindAssets("AcousticsLogo t:Texture");
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    _logo = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                }
            }
        }

        /// <summary>
        /// Opens the local documentation file.
        /// </summary>
        private void OpenLocalDocumentation()
        {
            // Find the local documentation file
            string[] guids = AssetDatabase.FindAssets("Acoustics Lite - Documentation");

            if (guids.Length > 0)
            {
                string relativePath = AssetDatabase.GUIDToAssetPath(guids[0]);
                string fullPath = Path.GetFullPath(relativePath);

                // Open the documentation file
                Application.OpenURL(fullPath);
            }
            else
            {
                // Display an error if the documentation file was not found
                EditorUtility.DisplayDialog("Error", "Documentation PDF not found! Please ensure 'Acoustics Lite - Documentation.pdf' is in the project folder.", "OK");
                Debug.LogError("AcousticsLite: Could not find file named 'Acoustics Lite - Documentation' in the project.");
            }
        }

        private void OnGUI()
        {
            if (_logo == null) LoadLogo();

            // --- DRAW LOGO ---
            if (_logo != null)
            {
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                float aspect = (float)_logo.width / _logo.height;
                float height = 80f;
                float width = height * aspect;

                GUILayout.Label(_logo, GUILayout.Width(width), GUILayout.Height(height));

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            // --- Header Style ---
            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter,
                margin = new RectOffset(0, 0, 10, 20)
            };

            GUILayout.Space(10);

            // --- Title ---
            GUILayout.Label("Acoustics Lite", headerStyle);
            GUILayout.Space(5);

            // --- Introduction ---
            EditorGUILayout.HelpBox("Thank you for choosing Acoustics Lite ! This tool provides realistic real-time sound occlusion and diffraction without any baking required.", MessageType.Info);

            GUILayout.Space(20);
            GUILayout.Label("Getting Started (Quick Setup):", EditorStyles.boldLabel);

            // --- Steps List ---
            DrawStep(1, "Setup Physics", "Ensure your walls and obstacles have Colliders and are assigned to a specific Layer (e.g., 'Default' or 'Obstacles').");
            DrawStep(2, "Add Audio", "Select any sound emitting object and add the 'AcousticSource' component. Or use the AcousticSource prefab inside the plugin's folder.");
            DrawStep(3, "Assign Listener", "Drag your Player or Main Camera into the 'Listener' field of the AcousticSource's script.");
            DrawStep(4, "Configure", "Set the 'Occlusion Layers' to match your walls (You can choose multiple layers). Adjust 'Source Radius' to simulate object size.");

            GUILayout.FlexibleSpace();

            // --- Show at Startup Checkbox ---
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            bool currentPref = EditorPrefs.GetBool(PREF_SHOW_AT_STARTUP, true);
            bool newPref = GUILayout.Toggle(currentPref, "Show this window at startup");

            if (newPref != currentPref)
            {
                EditorPrefs.SetBool(PREF_SHOW_AT_STARTUP, newPref);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // --- Buttons ---
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Open Full Documentation", GUILayout.Height(40)))
            {
                OpenLocalDocumentation();
            }

            if (GUILayout.Button("Close", GUILayout.Height(40)))
            {
                Close();
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(20);
        }

        /// <summary>
        /// Draws a step in the setup process.
        /// </summary>
        /// <param name="number">The step number.</param>
        /// <param name="title">The title of the step.</param>
        /// <param name="description">The description of the step.</param>
        void DrawStep(int number, string title, string description)
        {
            // Draws a step in the setup process.
            GUILayout.BeginVertical(EditorStyles.helpBox);
            // Step number and title
            GUILayout.Label($"{number}. {title}", EditorStyles.boldLabel);
            // Step description
            GUILayout.Label(description, EditorStyles.wordWrappedLabel);
            GUILayout.EndVertical();
            // Add some space between steps
            GUILayout.Space(5);
        }
    }
}
