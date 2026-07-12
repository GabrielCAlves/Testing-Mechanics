using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    [SerializeField] private Vector3 offset;
    [SerializeField] private bool withOffset = false;

    void Update()
    {
        transform.position = followTarget.position + (withOffset ? offset : Vector3.zero);
    }
}
