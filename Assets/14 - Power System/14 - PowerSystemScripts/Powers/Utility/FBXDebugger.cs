using UnityEngine;

public class FBXDebugger : MonoBehaviour
{
    public GameObject fbxToInspect;

    [ContextMenu("Inspect FBX")]
    void InspectFBX()
    {
        if (fbxToInspect == null)
        {
            Debug.LogWarning("Nenhum FBX para inspecionar! Arraste um .fbx para o campo.");
            return;
        }

        Debug.Log($"=== INSPECIONANDO: {fbxToInspect.name} ===");

        // --- 1. PROCURA SKINNED MESH RENDERER ---
        SkinnedMeshRenderer[] skinned = fbxToInspect.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        Debug.Log($"SkinnedMeshRenderers encontrados: {skinned.Length}");
        foreach (var s in skinned)
        {
            Debug.Log($"  ?? [{s.gameObject.name}]");
            Debug.Log($"  ?  ?? Mesh: {s.sharedMesh?.name ?? "NULL"}");
            Debug.Log($"  ?     ?? Materiais: {s.sharedMaterials?.Length ?? 0}");
        }

        // --- 2. PROCURA MESH FILTER ---
        MeshFilter[] filters = fbxToInspect.GetComponentsInChildren<MeshFilter>(true);
        Debug.Log($"MeshFilters encontrados: {filters.Length}");
        foreach (var f in filters)
        {
            Debug.Log($"  ?? [{f.gameObject.name}]");
            Debug.Log($"  ?  ?? Mesh: {f.sharedMesh?.name ?? "NULL"}");
        }

        // --- 3. PROCURA MESH RENDERER ---
        MeshRenderer[] renderers = fbxToInspect.GetComponentsInChildren<MeshRenderer>(true);
        Debug.Log($"MeshRenderers encontrados: {renderers.Length}");
        foreach (var r in renderers)
        {
            Debug.Log($"  ?? [{r.gameObject.name}]");
            Debug.Log($"  ?  ?? Materiais: {r.sharedMaterials?.Length ?? 0}");
        }

        // --- 4. MOSTRA A ESTRUTURA COMPLETA ---
        Debug.Log("=== ESTRUTURA DO FBX ===");
        PrintHierarchy(fbxToInspect.transform, "");

        Debug.Log("=== FIM DA INSPEÇÃO ===");
    }

    void PrintHierarchy(Transform parent, string indent)
    {
        Debug.Log($"{indent}?? {parent.name} (Layer: {parent.gameObject.layer})");

        // Mostra componentes importantes
        if (parent.GetComponent<MeshFilter>() != null)
            Debug.Log($"{indent}?  ?? ? Tem MeshFilter");
        if (parent.GetComponent<SkinnedMeshRenderer>() != null)
            Debug.Log($"{indent}?  ?? ? Tem SkinnedMeshRenderer");
        if (parent.GetComponent<MeshRenderer>() != null)
            Debug.Log($"{indent}?  ?? ? Tem MeshRenderer");

        foreach (Transform child in parent)
        {
            PrintHierarchy(child, indent + "?  ");
        }
    }
}