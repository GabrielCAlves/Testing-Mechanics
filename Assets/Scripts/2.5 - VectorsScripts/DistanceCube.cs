using UnityEngine;

public class DistanceCube : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private float distance;

    // Update is called once per frame
    void Update()
    {
        if (targetObject == null)
            return;

        distance = Vector3.Distance(transform.position, targetObject.transform.position);
    }
}
