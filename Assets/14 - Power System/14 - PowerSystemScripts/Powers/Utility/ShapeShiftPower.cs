using UnityEngine;

[CreateAssetMenu(fileName = "NewShapeShiftPower", menuName = "Powers/Utility/Shape Shift Power")]
public class ShapeShiftPower : Power
{
    [Header("Configurações de Mudança de Forma")]
    public GameObject[] alternateForms;
    public GameObject transformEffect;
    public AudioClip transformSound;

    private int currentFormIndex = -1;
    private GameObject userObject;
    private bool isActive = false;

    // Estado original do player
    private Mesh originalMesh;
    private Material[] originalMaterials;
    private SkinnedMeshRenderer userSkinnedRenderer;
    private MeshFilter userMeshFilter;
    private MeshRenderer userMeshRenderer;
    private SkinnedMeshRenderer userSkinnedOriginal;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        isActive = true;
        userObject = user;

        // Salva o estado original do player
        SaveOriginalState(user);

        // Aplica a próxima forma
        NextForm(user);
    }

    void SaveOriginalState(GameObject user)
    {
        // --- SALVA SKINNED MESH RENDERER ---
        userSkinnedRenderer = user.GetComponentInChildren<SkinnedMeshRenderer>();
        if (userSkinnedRenderer != null)
        {
            originalMesh = userSkinnedRenderer.sharedMesh;
            originalMaterials = userSkinnedRenderer.sharedMaterials;
            Debug.Log($"Original SkinnedMesh: {originalMesh?.name}");
            return;
        }

        // --- SALVA MESH RENDERER + MESH FILTER ---
        userMeshFilter = user.GetComponentInChildren<MeshFilter>();
        userMeshRenderer = user.GetComponentInChildren<MeshRenderer>();

        if (userMeshFilter != null && userMeshRenderer != null)
        {
            originalMesh = userMeshFilter.sharedMesh;
            originalMaterials = userMeshRenderer.sharedMaterials;
            Debug.Log($"Original Mesh: {originalMesh?.name}");
        }
        else
        {
            Debug.LogWarning("Nenhum renderer encontrado no usuário!");
        }
    }

    void NextForm(GameObject user)
    {
        if (alternateForms == null || alternateForms.Length == 0)
        {
            Debug.LogWarning("Nenhuma forma definida!");
            return;
        }

        currentFormIndex = (currentFormIndex + 1) % alternateForms.Length;
        GameObject formPrefab = alternateForms[currentFormIndex];

        // --- TENTA APLICAR A FORMA ---
        bool success = ApplyForm(user, formPrefab);

        if (!success)
        {
            Debug.LogError($"Falha ao aplicar forma: {formPrefab.name}");
            return;
        }

        // Efeitos
        if (transformEffect != null)
            Instantiate(transformEffect, user.transform.position, Quaternion.identity);
        if (transformSound != null)
            AudioSource.PlayClipAtPoint(transformSound, user.transform.position);

        Debug.Log($"Forma alterada para: {formPrefab.name}");
    }

    bool ApplyForm(GameObject user, GameObject formPrefab)
    {
        // --- MÉTODO 1: PROCURA POR SKINNED MESH RENDERER ---
        SkinnedMeshRenderer formSkinned = formPrefab.GetComponentInChildren<SkinnedMeshRenderer>(true);
        if (formSkinned != null)
        {
            // Verifica se o player também tem SkinnedMeshRenderer
            if (userSkinnedRenderer != null)
            {
                userSkinnedRenderer.sharedMesh = formSkinned.sharedMesh;
                userSkinnedRenderer.sharedMaterials = formSkinned.sharedMaterials;

                if (formSkinned.bones != null && formSkinned.bones.Length > 0)
                {
                    userSkinnedRenderer.bones = formSkinned.bones;
                    userSkinnedRenderer.rootBone = formSkinned.rootBone;
                }

                Debug.Log($"SkinnedMesh aplicado: {formSkinned.sharedMesh?.name}");
                return true;
            }
            else
            {
                // O player não tem SkinnedMeshRenderer, mas a forma tem
                // Podemos tentar criar um ou usar MeshFilter
                Debug.Log($"Forma tem SkinnedMesh, mas player não. Tentando MeshFilter...");
            }
        }

        // --- MÉTODO 2: PROCURA POR MESH FILTER ---
        MeshFilter[] allFilters = formPrefab.GetComponentsInChildren<MeshFilter>(true);
        if (allFilters.Length > 0)
        {
            // Pega o primeiro MeshFilter com mesh válido
            foreach (var filter in allFilters)
            {
                if (filter != null && filter.sharedMesh != null)
                {
                    if (userMeshFilter != null)
                    {
                        userMeshFilter.sharedMesh = filter.sharedMesh;

                        // Tenta pegar os materiais
                        MeshRenderer formRenderer = filter.GetComponent<MeshRenderer>();
                        if (formRenderer != null && userMeshRenderer != null)
                        {
                            userMeshRenderer.sharedMaterials = formRenderer.sharedMaterials;
                        }

                        Debug.Log($"MeshFilter aplicado: {filter.sharedMesh.name} (de {filter.gameObject.name})");
                        return true;
                    }
                }
            }
        }

        // --- MÉTODO 3: PROCURA POR QUALQUER MESH EM COMPONENTES ---
        // Alguns .fbx têm o Mesh como componente do próprio GameObject raiz
        MeshFilter rootFilter = formPrefab.GetComponent<MeshFilter>();
        if (rootFilter != null && rootFilter.sharedMesh != null && userMeshFilter != null)
        {
            userMeshFilter.sharedMesh = rootFilter.sharedMesh;

            MeshRenderer rootRenderer = formPrefab.GetComponent<MeshRenderer>();
            if (rootRenderer != null && userMeshRenderer != null)
            {
                userMeshRenderer.sharedMaterials = rootRenderer.sharedMaterials;
            }

            Debug.Log($"Mesh da raiz aplicado: {rootFilter.sharedMesh.name}");
            return true;
        }

        // --- MÉTODO 4: PROCURA POR MESHES EM TODOS OS FILHOS (recursivo) ---
        // Esta é a busca mais agressiva
        MeshFilter[] allMeshFilters = formPrefab.GetComponentsInChildren<MeshFilter>(true);
        foreach (var filter in allMeshFilters)
        {
            if (filter != null && filter.sharedMesh != null)
            {
                if (userMeshFilter != null)
                {
                    userMeshFilter.sharedMesh = filter.sharedMesh;

                    MeshRenderer renderer = filter.GetComponent<MeshRenderer>();
                    if (renderer != null && userMeshRenderer != null)
                    {
                        userMeshRenderer.sharedMaterials = renderer.sharedMaterials;
                    }

                    Debug.Log($"Mesh encontrado em filho: {filter.sharedMesh.name} (em {filter.gameObject.name})");
                    return true;
                }
            }
        }

        // --- MÉTODO 5: TENTA EXTRAIR O MESH DE UM SKINNED MESH RENDERER (fallback) ---
        // Se tudo falhar, mas tiver SkinnedMeshRenderer, tenta extrair o mesh
        if (formSkinned != null && formSkinned.sharedMesh != null)
        {
            if (userMeshFilter != null)
            {
                // Extrai o mesh do SkinnedMeshRenderer e aplica como MeshFilter
                userMeshFilter.sharedMesh = formSkinned.sharedMesh;

                if (userMeshRenderer != null)
                {
                    userMeshRenderer.sharedMaterials = formSkinned.sharedMaterials;
                }

                Debug.Log($"Mesh extraído do SkinnedMeshRenderer: {formSkinned.sharedMesh.name}");
                return true;
            }
        }

        Debug.LogWarning($"Nenhum mesh encontrado em {formPrefab.name}");
        return false;
    }

    void RestoreOriginalState()
    {
        if (userObject == null) return;

        if (userSkinnedRenderer != null)
        {
            if (originalMesh != null)
                userSkinnedRenderer.sharedMesh = originalMesh;
            if (originalMaterials != null)
                userSkinnedRenderer.sharedMaterials = originalMaterials;
            Debug.Log($"SkinnedMesh restaurado: {originalMesh?.name}");
        }
        else if (userMeshFilter != null && userMeshRenderer != null)
        {
            if (originalMesh != null)
                userMeshFilter.sharedMesh = originalMesh;
            if (originalMaterials != null)
                userMeshRenderer.sharedMaterials = originalMaterials;
            Debug.Log($"Mesh restaurado: {originalMesh?.name}");
        }
    }

    public override void UpdatePower(GameObject user)
    {
        // Não precisa fazer nada
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        isActive = false;
        //RestoreOriginalState();
        //currentFormIndex = -1;
        Debug.Log("ShapeShiftPower desativado");
    }
}