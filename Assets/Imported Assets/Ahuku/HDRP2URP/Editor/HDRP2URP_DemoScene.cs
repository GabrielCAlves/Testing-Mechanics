using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

namespace Ahuku.HDRP2URP
{
    public static class HDRP2URP_DemoScene
    {
        const string DemoFolder = "Assets/Ahuku/HDRP2URP/Demo";
        const string MatFolder  = "Assets/Ahuku/HDRP2URP/Demo/Materials";

        [MenuItem("Tools/HDRP → URP Converter/Create Demo Scene")]
        public static void CreateDemoScene()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            // ── folders ──────────────────────────────────────────────────────────
            if (!AssetDatabase.IsValidFolder(DemoFolder))
                AssetDatabase.CreateFolder("Assets/Ahuku/HDRP2URP", "Demo");
            if (!AssetDatabase.IsValidFolder(MatFolder))
                AssetDatabase.CreateFolder(DemoFolder, "Materials");

            // ── materials ────────────────────────────────────────────────────────
            MakeMat("Sample_Metal",    new Color(0.85f, 0.85f, 0.85f), 1f,  0.8f);
            MakeMat("Sample_Wood",     new Color(0.55f, 0.35f, 0.15f), 0f,  0.25f);
            MakeMat("Sample_Plastic",  new Color(0.2f,  0.45f, 0.85f), 0f,  0.65f);
            MakeMat("Sample_Emissive", Color.white,                     0.3f, 0.5f,
                    emissive: new Color(1f, 0.55f, 0f));

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // ── new empty scene ───────────────────────────────────────────────────
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Camera
            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            camGo.AddComponent<Camera>();
            camGo.AddComponent<AudioListener>();
            camGo.transform.position = new Vector3(0f, 2.5f, -7f);
            camGo.transform.rotation = Quaternion.Euler(15f, 0f, 0f);

            // Light
            var lightGo = new GameObject("Directional Light");
            var dl = lightGo.AddComponent<Light>();
            dl.type      = LightType.Directional;
            dl.intensity  = 1f;
            dl.color      = Color.white;
            lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            // Ground
            var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.name = "Ground";
            plane.transform.localScale = new Vector3(1.5f, 1f, 1.5f);

            // Objects
            Spawn(PrimitiveType.Sphere,    "Metal_Sphere",     new Vector3(-3f, 0.5f, 0f), "Sample_Metal");
            Spawn(PrimitiveType.Cube,      "Wood_Cube",        new Vector3(-1f, 0.5f, 0f), "Sample_Wood");
            Spawn(PrimitiveType.Cylinder,  "Plastic_Cylinder", new Vector3( 1f, 0.7f, 0f), "Sample_Plastic");
            Spawn(PrimitiveType.Sphere,    "Emissive_Sphere",  new Vector3( 3f, 0.5f, 0f), "Sample_Emissive");

            // Instructions marker
            new GameObject("[ Tools  ▶  HDRP → URP Converter  ▶  Add Folder: Demo/Materials  ▶  Convert ]");

            // ── save ─────────────────────────────────────────────────────────────
            var scenePath = DemoFolder + "/HDRP2URP_Demo.unity";
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), scenePath);
            AssetDatabase.ImportAsset(scenePath);
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Done",
                "Demo scene saved:\n" + scenePath +
                "\n\nTo try the converter:\n" +
                "1. Tools → HDRP → URP Converter (Free)\n" +
                "2. Add Folder → Demo/Materials\n" +
                "3. Convert", "OK");
        }

        static void Spawn(PrimitiveType type, string name, Vector3 pos, string matName)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.position = pos;
            var mat = AssetDatabase.LoadAssetAtPath<Material>(MatFolder + "/" + matName + ".mat");
            if (mat != null)
                go.GetComponent<Renderer>().sharedMaterial = mat;
        }

        static void MakeMat(string name, Color color, float metallic, float smoothness,
                            Color emissive = default)
        {
            var path = MatFolder + "/" + name + ".mat";
            if (AssetDatabase.LoadAssetAtPath<Material>(path) != null) return;

            var shader = Shader.Find("Universal Render Pipeline/Lit")
                      ?? Shader.Find("Standard");
            if (shader == null) return;

            var mat = new Material(shader) { name = name };

            mat.SetColor("_BaseColor",          color);
            mat.SetFloat("_Metallic",           metallic);
            mat.SetFloat("_Smoothness",         smoothness);
            mat.SetFloat("_MetallicRemapMax",   metallic);
            mat.SetFloat("_SmoothnessRemapMax", smoothness);
            mat.SetColor("_BaseColorMap",       color);

            if (emissive != default(Color))
            {
                mat.SetColor("_EmissiveColor",    emissive);
                mat.SetColor("_EmissiveColorLDR", emissive);
                mat.SetColor("_EmissionColor",    emissive);
                mat.EnableKeyword("_EMISSION");
            }

            AssetDatabase.CreateAsset(mat, path);
        }
    }
}
