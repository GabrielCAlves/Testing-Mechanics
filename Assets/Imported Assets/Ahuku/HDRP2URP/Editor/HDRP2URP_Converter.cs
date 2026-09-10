using UnityEngine;
using UnityEditor;
using System.IO;

namespace Ahuku.HDRP2URP
{
    public static class HDRP2URP_Converter
    {
        public struct Result { public string tag; public string error; }

        public static Result Convert(Material mat, bool saveCopy)
        {
            if (mat == null) return new Result { tag = "skipped" };

            HDRPData data;
            try { data = ReadHDRP(mat); }
            catch (System.Exception e)
            { return new Result { tag = "error", error = e.Message }; }

            Material dst = saveCopy ? MakeCopy(mat) : mat;
            if (dst == null) return new Result { tag = "error", error = "copy failed" };

            Undo.RecordObject(dst, "HDRP → URP");
            try
            {
                if (data.isUnlit) { ApplyUnlit(dst, data); }
                else              { ApplyURP(dst, data); }
                EditorUtility.SetDirty(dst);
                return new Result { tag = data.isUnlit ? "unlit" : "converted" };
            }
            catch (System.Exception e)
            { return new Result { tag = "error", error = e.Message }; }
        }

        // ── URP Lit ───────────────────────────────────────────────────────────
        static void ApplyURP(Material mat, HDRPData d)
        {
            mat.shader = Shader.Find("Universal Render Pipeline/Lit")
                      ?? Shader.Find("Lightweight Render Pipeline/Lit");
            if (mat.shader == null) throw new System.Exception("URP Lit shader not found. Is URP installed?");

            mat.SetColor("_BaseColor", d.baseColor);
            if (d.baseMap != null) mat.SetTexture("_BaseMap", d.baseMap);

            if (d.normalMap != null)
            {
                mat.SetTexture("_BumpMap", d.normalMap);
                mat.SetFloat("_BumpScale", d.normalScale);
                mat.EnableKeyword("_NORMALMAP");
            }
            else mat.DisableKeyword("_NORMALMAP");

            if (d.maskMap != null)
            {
                mat.SetTexture("_MetallicGlossMap", d.maskMap);
                mat.SetTexture("_OcclusionMap", d.maskMap);
                mat.SetFloat("_Metallic",   Mathf.Clamp01(d.metallicRemapMax > 0f ? d.metallicRemapMax : d.effMet));
                mat.SetFloat("_Smoothness", Mathf.Clamp01(d.smRemapMax));
                mat.EnableKeyword("_METALLICSPECGLOSSMAP");
            }
            else
            {
                mat.SetFloat("_Metallic",   d.effMet);
                mat.SetFloat("_Smoothness", d.effSm);
                mat.DisableKeyword("_METALLICSPECGLOSSMAP");
            }

            bool hasEmit = d.emissiveMap != null ||
                           d.emissiveColor.r > 0.001f || d.emissiveColor.g > 0.001f || d.emissiveColor.b > 0.001f;
            if (hasEmit)
            {
                mat.SetTexture("_EmissionMap", d.emissiveMap);
                mat.SetColor("_EmissionColor", d.emissiveColor);
                mat.EnableKeyword("_EMISSION");
            }
            else
            {
                mat.SetColor("_EmissionColor", Color.black);
                mat.DisableKeyword("_EMISSION");
            }

            mat.DisableKeyword("_ALPHATEST_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            if (d.isTransparent)
            {
                mat.SetFloat("_Surface", 1f);
                mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
            else if (d.isCutout)
            {
                mat.SetFloat("_Surface",   0f);
                mat.SetFloat("_AlphaClip", 1f);
                mat.SetFloat("_Cutoff",    d.cutoff);
                mat.EnableKeyword("_ALPHATEST_ON");
            }
            else
            {
                mat.SetFloat("_Surface",   0f);
                mat.SetFloat("_AlphaClip", 0f);
            }
        }

        static void ApplyUnlit(Material mat, HDRPData d)
        {
            mat.shader = Shader.Find("Universal Render Pipeline/Unlit")
                      ?? Shader.Find("Unlit/Texture");
            if (mat.shader == null) throw new System.Exception("Unlit shader not found. Is URP installed?");
            var tex = d.baseMap ?? d.emissiveMap;
            if (tex != null) mat.SetTexture("_BaseMap", tex);
            mat.SetColor("_BaseColor", Color.white);
        }

        // ── read HDRP props via SerializedObject (single instance per material) ──
        static HDRPData ReadHDRP(Material mat)
        {
            var d  = new HDRPData();
            var so = new SerializedObject(mat);

            d.baseMap     = Tex(so, "_BaseMap", "_BaseColorMap", "_BaseColorMap0", "_MainTex");
            d.normalMap   = Tex(so, "_NormalMap", "_NormalMap0", "_BumpMap");
            d.maskMap     = Tex(so, "_MaskMap", "_MaskMap0");
            d.emissiveMap = Tex(so, "_EmissiveColorMap", "_EmissiveMap", "_EmissionMap");

            var raw = Col(so, "_BaseColor", Color.white);
            if (raw == Color.white) raw = Col(so, "_Color", Color.white);
            d.baseColor = Clamp01(raw);

            var ldr = Col(so, "_EmissiveColorLDR", Color.black);
            if (ldr.r > 0.001f || ldr.g > 0.001f || ldr.b > 0.001f) d.emissiveColor = ldr;
            else
            {
                var hdr = Col(so, "_EmissiveColor", Color.black);
                float mx = Mathf.Max(hdr.r, hdr.g, hdr.b);
                d.emissiveColor = mx > 1f ? new Color(hdr.r/mx, hdr.g/mx, hdr.b/mx, hdr.a) : hdr;
            }

            d.metallic         = Flt(so, "_Metallic", 0f);
            d.smoothness       = Flt(so, "_Smoothness", -1f);
            if (d.smoothness < 0f) d.smoothness = Flt(so, "_Glossiness", 0.5f);
            d.metallicRemapMax = Flt(so, "_MetallicRemapMax", 1f);
            d.metallicRemapMin = Flt(so, "_MetallicRemapMin", 0f);
            d.smRemapMax       = Flt(so, "_SmoothnessRemapMax", 1f);
            d.smRemapMin       = Flt(so, "_SmoothnessRemapMin", 0f);
            d.normalScale      = Flt(so, "_NormalScale", Flt(so, "_BumpScale", 1f));
            d.cutoff           = Flt(so, "_AlphaCutoff", 0.5f);

            d.effMet = Mathf.Clamp01(Mathf.Lerp(d.metallicRemapMin, d.metallicRemapMax, d.metallic));
            d.effSm  = Mathf.Clamp01(Mathf.Lerp(d.smRemapMin, d.smRemapMax, d.smoothness));
            float gs = Flt(so, "_GlossMapScale", -1f);
            if (d.effSm <= 0.001f && gs > 0.001f) d.effSm = Mathf.Clamp01(gs);

            // Detect Unlit by shader name — more reliable than property check
            var sn = mat.shader.name;
            d.isUnlit = sn.Contains("Unlit") || sn.Contains("unlit");

            d.isTransparent = Flt(so, "_SurfaceType", 0f) == 1f;
            d.isCutout      = Flt(so, "_AlphaCutoffEnable", 0f) == 1f || Flt(so, "_AlphaClipping", 0f) == 1f;

            return d;
        }

        static Material MakeCopy(Material src)
        {
            var p = AssetDatabase.GetAssetPath(src);
            if (string.IsNullOrEmpty(p)) return null;
            var dir = Path.GetDirectoryName(p).Replace("\\", "/") + "/ConvertedMaterials";
            Directory.CreateDirectory(dir);
            var dst = AssetDatabase.GenerateUniqueAssetPath(dir + "/" + Path.GetFileName(p));
            AssetDatabase.CopyAsset(p, dst);
            AssetDatabase.ImportAsset(dst);
            return AssetDatabase.LoadAssetAtPath<Material>(dst);
        }

        static Texture Tex(SerializedObject so, params string[] names)
        {
            var envs = so.FindProperty("m_SavedProperties.m_TexEnvs");
            if (envs == null) return null;
            foreach (var n in names)
                for (int i = 0; i < envs.arraySize; i++)
                {
                    var e = envs.GetArrayElementAtIndex(i);
                    if (e.FindPropertyRelative("first")?.stringValue != n) continue;
                    var t = e.FindPropertyRelative("second")?.FindPropertyRelative("m_Texture");
                    if (t?.objectReferenceValue is Texture tex) return tex;
                }
            return null;
        }

        static Color Col(SerializedObject so, string n, Color def)
        {
            var c = so.FindProperty("m_SavedProperties.m_Colors");
            if (c == null) return def;
            for (int i = 0; i < c.arraySize; i++)
            {
                var e = c.GetArrayElementAtIndex(i);
                if (e.FindPropertyRelative("first")?.stringValue != n) continue;
                var v = e.FindPropertyRelative("second");
                if (v != null) return v.colorValue;
            }
            return def;
        }

        static float Flt(SerializedObject so, string n, float def)
        {
            var f = so.FindProperty("m_SavedProperties.m_Floats");
            if (f == null) return def;
            for (int i = 0; i < f.arraySize; i++)
            {
                var e = f.GetArrayElementAtIndex(i);
                if (e.FindPropertyRelative("first")?.stringValue != n) continue;
                var v = e.FindPropertyRelative("second");
                if (v != null) return v.floatValue;
            }
            return def;
        }

        static Color Clamp01(Color c) =>
            new Color(Mathf.Clamp01(c.r), Mathf.Clamp01(c.g), Mathf.Clamp01(c.b), Mathf.Clamp01(c.a));

        struct HDRPData
        {
            public Texture baseMap, normalMap, maskMap, emissiveMap;
            public Color   baseColor, emissiveColor;
            public float   metallic, smoothness;
            public float   metallicRemapMax, metallicRemapMin, smRemapMax, smRemapMin;
            public float   effMet, effSm, normalScale, cutoff;
            public bool    isUnlit, isTransparent, isCutout;
        }
    }
}
