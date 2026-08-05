using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshFixer : MonoBehaviour
{
    [Header("Configurações")]
    public float yOffset = 0.1f;
    public LayerMask groundLayers;
    public bool autoFix = true;

    private NavMeshSurface navMeshSurface;

    void Start()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
        if (autoFix)
        {
            FixNavMesh();                                                        // O playerMovement é que está fazendo o player ficar com um espaço acima do chão
        }
    }

    void FixNavMesh()
    {
        // Ajusta o offset do NavMesh
        // Isso força o NavMesh a ser gerado um pouco acima do chão
        if (navMeshSurface != null)
        {
            // Re-bake com o offset
            navMeshSurface.BuildNavMesh();
        }
    }

    // Opção: Ajustar objetos que estão flutuando
    public void AdjustFloatingObjects()
    {
        Collider[] allObjects = Physics.OverlapSphere(Vector3.zero, 100f, groundLayers);

        foreach (var obj in allObjects)
        {
            if (obj.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                // Ajusta a posição Y do objeto
                Vector3 pos = obj.transform.position;
                pos.y = 0f;
                obj.transform.position = pos;
            }
        }
    }
}