using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Ahuku.HDRP2URP
{
    public class HDRP2URP_Window : EditorWindow
    {
        enum SaveMode { Override, SaveCopy }

        List<Material> _mats      = new List<Material>();
        Vector2         _scroll;
        Vector2         _logScroll;
        SaveMode        _saveMode  = SaveMode.Override;
        bool            _sceneFold = false;
        string          _scenePath = "";
        string          _status   = "";
        List<string>    _log      = new List<string>();

        static readonly Color _green = new Color(0.4f, 0.9f, 0.4f);
        static readonly Color _red   = new Color(1f,   0.4f, 0.4f);
        static readonly Color _gray  = new Color(0.6f, 0.6f, 0.6f);

        [MenuItem("Tools/HDRP → URP Converter (Free)")]
        public static void Open() =>
            GetWindow<HDRP2URP_Window>("HDRP → URP").minSize = new Vector2(400, 480);

        void OnGUI()
        {
            GUILayout.Space(6);

            // Header
            var hs = new GUIStyle(EditorStyles.boldLabel) { fontSize = 13, alignment = TextAnchor.MiddleCenter };
            GUILayout.Label("HDRP  →  URP  Converter", hs);
            var badge = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                { normal = { textColor = new Color(0.4f, 0.9f, 0.4f) } };
            GUILayout.Label("FREE", badge);
            var hr = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(hr, new Color(0.5f, 0.5f, 0.5f, 0.4f));
            GUILayout.Space(4);

            // Toolbar
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Find Broken", GUILayout.Height(22))) FindBroken();
                if (GUILayout.Button("Add Folder",  GUILayout.Height(22))) AddFolder();
                if (GUILayout.Button("Clear", GUILayout.Width(50), GUILayout.Height(22)))
                { _mats.Clear(); _log.Clear(); _status = ""; }
            }
            GUILayout.Space(4);

            // Drop area
            var drop = GUILayoutUtility.GetRect(0, 110, GUILayout.ExpandWidth(true));
            GUI.Box(drop, GUIContent.none, GUI.skin.box);
            if (_mats.Count == 0)
            {
                GUI.Label(drop, "Drag materials or folders here",
                          new GUIStyle(EditorStyles.centeredGreyMiniLabel) { fontSize = 11 });
            }
            else
            {
                var sv = new Rect(drop.x+2, drop.y+2, drop.width-4, drop.height-4);
                var vw = new Rect(0, 0, sv.width-16, _mats.Count*18);
                _scroll = GUI.BeginScrollView(sv, _scroll, vw);
                for (int i = 0; i < _mats.Count; i++)
                {
                    GUI.Label(new Rect(2, i*18, vw.width-22, 16),
                              _mats[i] != null ? _mats[i].name : "(null)");
                    if (GUI.Button(new Rect(vw.width-18, i*18, 18, 16), "×", EditorStyles.miniButton))
                    { _mats.RemoveAt(i); break; }
                }
                GUI.EndScrollView();
            }
            HandleDrop(drop);
            GUILayout.Label(_mats.Count + " material" + (_mats.Count == 1 ? "" : "s"),
                            EditorStyles.miniLabel);
            GUILayout.Space(4);

            // Save mode
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Save:", GUILayout.Width(36));
                _saveMode = (SaveMode)GUILayout.SelectionGrid(
                    (int)_saveMode, new[] { "Override", "Save Copy" },
                    2, EditorStyles.miniButton, GUILayout.Height(20));
            }
            GUILayout.Space(6);

            // Convert button
            GUI.enabled = _mats.Count > 0;
            if (GUILayout.Button("▶  Convert  " + _mats.Count + "  Materials  →  URP",
                new GUIStyle(GUI.skin.button) { fontSize = 13, fixedHeight = 32 }))
                Run();
            GUI.enabled = true;

            if (!string.IsNullOrEmpty(_status))
                GUILayout.Label(_status, EditorStyles.centeredGreyMiniLabel);

            GUILayout.Space(4);
            DrawSceneSection();

            // Log
            if (_log.Count > 0)
            {
                GUILayout.Space(4);
                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.5f,0.5f,0.5f,0.3f));
                GUILayout.Label("Log", EditorStyles.boldLabel);
                _logScroll = EditorGUILayout.BeginScrollView(_logScroll, GUILayout.Height(110));
                var ms = new GUIStyle(EditorStyles.miniLabel);
                foreach (var line in _log)
                {
                    ms.normal.textColor = line.StartsWith("✓") ? _green
                                        : line.StartsWith("✗") ? _red : _gray;
                    GUILayout.Label(line, ms);
                }
                EditorGUILayout.EndScrollView();
            }
        }

        void Run()
        {
            _log.Clear();
            int conv = 0, unlit = 0, skip = 0, err = 0;
            try
            {
                for (int i = 0; i < _mats.Count; i++)
                {
                    var m = _mats[i];
                    if (m == null) { skip++; continue; }
                    EditorUtility.DisplayProgressBar("HDRP → URP", m.name, (float)i / _mats.Count);
                    var r = HDRP2URP_Converter.Convert(m, _saveMode == SaveMode.SaveCopy);
                    if      (r.tag == "converted") { conv++;  _log.Add("✓ " + m.name); }
                    else if (r.tag == "unlit")     { unlit++; _log.Add("✓ " + m.name + "  [Unlit]"); }
                    else if (r.tag == "error")     { err++;   _log.Add("✗ " + m.name + "  " + r.error); }
                    else                           { skip++;  _log.Add("· " + m.name); }
                }
            }
            finally { EditorUtility.ClearProgressBar(); }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            _status = "Converted: " + conv + "  Unlit: " + unlit + "  Skipped: " + skip
                    + (err > 0 ? "  Errors: " + err : "");
            _log.Add("─────────────");
            _log.Add(_status);
            _logScroll = new Vector2(0, float.MaxValue);
            Repaint();
        }

        void HandleDrop(Rect area)
        {
            var e = Event.current;
            if (!area.Contains(e.mousePosition)) return;
            if (e.type != EventType.DragUpdated && e.type != EventType.DragPerform) return;
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            if (e.type != EventType.DragPerform) return;
            DragAndDrop.AcceptDrag();
            foreach (var o in DragAndDrop.objectReferences)
            {
                if (o is Material m) Add(m);
                else if (o is DefaultAsset)
                {
                    var p = AssetDatabase.GetAssetPath(o);
                    if (Directory.Exists(p)) AddFromFolder(p);
                }
            }
            foreach (var p in DragAndDrop.paths)
                if (Directory.Exists(p)) AddFromFolder(p);
            e.Use();
        }

        void Add(Material m) { if (m != null && !_mats.Contains(m)) _mats.Add(m); }
        void AddFromFolder(string path)
        {
            foreach (var g in AssetDatabase.FindAssets("t:Material", new[] { path }))
                Add(AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(g)));
        }
        void AddFolder()
        {
            var abs = EditorUtility.OpenFolderPanel("Select folder", "Assets", "");
            if (string.IsNullOrEmpty(abs)) return;
            AddFromFolder("Assets" + abs.Replace(Application.dataPath, "").Replace("\\", "/"));
        }
        void FindBroken()
        {
            int found = 0;
            foreach (var g in AssetDatabase.FindAssets("t:Material", new[] { "Assets" }))
            {
                var p = AssetDatabase.GUIDToAssetPath(g);
                if (!p.StartsWith("Assets/")) continue;
                var m = AssetDatabase.LoadAssetAtPath<Material>(p);
                if (m == null) continue;
                var sn = m.shader.name;
                if (sn.Contains("Hidden/InternalErrorShader") || sn.Contains("HDRP/") ||
                    sn.Contains("HDRenderPipeline"))
                { Add(m); found++; }
            }
            _log.Add("Found " + found + " broken/HDRP materials");
            Repaint();
        }

        // ── scene lighting normalization ──────────────────────────────────────
        void DrawSceneSection()
        {
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1),
                               new Color(0.5f, 0.5f, 0.5f, 0.3f));
            _sceneFold = EditorGUILayout.Foldout(_sceneFold, "Scene Lighting Normalization", true);
            if (!_sceneFold) return;

            EditorGUILayout.HelpBox(
                "HDRP uses physical light units (10000+ Lux).\n" +
                "This normalizes light intensities for URP and clears HDRP lightmap data.",
                MessageType.Info);

            using (new EditorGUILayout.HorizontalScope())
            {
                _scenePath = EditorGUILayout.TextField(_scenePath);
                if (GUILayout.Button("Browse", GUILayout.Width(58)))
                {
                    var p = EditorUtility.OpenFilePanel("Select .unity scene", "Assets", "unity");
                    if (!string.IsNullOrEmpty(p))
                        _scenePath = "Assets" + p.Replace(Application.dataPath, "").Replace("\\", "/");
                }
                if (GUILayout.Button("Active", GUILayout.Width(48)))
                    _scenePath = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path;
            }

            GUI.enabled = !string.IsNullOrEmpty(_scenePath) && File.Exists(_scenePath);
            if (GUILayout.Button("▶  Normalize Scene Lighting", GUILayout.Height(26)))
                NormalizeScene(_scenePath);
            GUI.enabled = true;
        }

        void NormalizeScene(string path)
        {
            try
            {
                var c = File.ReadAllText(path);
                int lights = 0;

                // Adjust over-bright HDRP lights
                c = Regex.Replace(c,
                    @"--- !u!108 &[\d-]+[\r\n]+Light:([\s\S]*?)(?=--- !u!|\Z)",
                    m => {
                        var tm = Regex.Match(m.Value, @"m_Type:\s*(\d+)");
                        var im = Regex.Match(m.Value, @"m_Intensity:\s*([\d.]+)");
                        if (!tm.Success || !im.Success) return m.Value;
                        int   t  = int.Parse(tm.Groups[1].Value);
                        float iv = float.Parse(im.Groups[1].Value,
                                               System.Globalization.CultureInfo.InvariantCulture);
                        float ni = iv;
                        if (t == 1 && iv > 10f)          ni = 1.5f;
                        else if ((t == 0 || t == 2) && iv > 5f) ni = Mathf.Clamp(iv / 100f, 0.5f, 3f);
                        if (ni == iv) return m.Value;
                        lights++;
                        var ivStr = im.Groups[1].Value;
                        var niStr = ni.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
                        return m.Value.Replace("m_Intensity: " + ivStr, "m_Intensity: " + niStr);
                    });

                // Clear HDRP baked lightmaps
                c = Regex.Replace(c, @"m_LightmapIndex:\s*\d+",        "m_LightmapIndex: 65535");
                c = Regex.Replace(c, @"m_LightmapIndexDynamic:\s*\d+", "m_LightmapIndexDynamic: 65535");
                c = Regex.Replace(c,
                    @"m_LightmapScaleOffset:\s*\{x:\s*[\d.-]+,\s*y:\s*[\d.-]+,\s*z:\s*[\d.-]+,\s*w:\s*[\d.-]+\}",
                    "m_LightmapScaleOffset: {x: 1, y: 1, z: 0, w: 0}");
                c = Regex.Replace(c,
                    @"m_LightingDataAsset:\s*\{fileID:\s*[\d-]+,\s*guid:\s*[0-9a-f]+,\s*type:\s*\d+\}",
                    "m_LightingDataAsset: {fileID: 0}");

                // Flat ambient
                c = Regex.Replace(c, @"m_AmbientMode:\s*\d+", "m_AmbientMode: 1");
                c = Regex.Replace(c,
                    @"m_AmbientSkyColor:\s*\{r:\s*[\d.-]+,\s*g:\s*[\d.-]+,\s*b:\s*[\d.-]+,\s*a:\s*[\d.-]+\}",
                    "m_AmbientSkyColor: {r: 0.2, g: 0.2, b: 0.22, a: 1}");

                File.WriteAllText(path, c, new System.Text.UTF8Encoding(false));
                AssetDatabase.ImportAsset(path);

                _log.Add("✓ " + Path.GetFileName(path) + "  — " + lights + " lights adjusted");
            }
            catch (System.Exception e)
            {
                _log.Add("✗ Scene error: " + e.Message);
            }
            _logScroll = new Vector2(0, float.MaxValue);
            Repaint();
        }
    }
}
