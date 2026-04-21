using UnityEngine;
using UnityEngine.AI;

public class NavMeshMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;

    [Header("Settings")]
    [SerializeField] private float speed = 10f;

    [Header("Ground Detection")]
    [SerializeField] private float sampleDistance = .5f;
    [SerializeField] private LayerMask groundLayer;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = speed;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if(Physics.Raycast(ray, out RaycastHit hit, groundLayer))
            {
                if(NavMesh.SamplePosition(hit.point, out NavMeshHit navMeshHit, sampleDistance, NavMesh.AllAreas))
                {
                    agent.SetDestination(navMeshHit.position);
                }
                else
                {
                    Debug.LogError("Clicked point is not on a walkable area.");
                }
            }
        }
    }
}
