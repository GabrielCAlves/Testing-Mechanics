using UnityEngine;

public class CameraCon : MonoBehaviour
{
    [SerializeField] private Transform camTarget;
    [SerializeField] private float pLerp;
    [SerializeField] private float rLerp;

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, camTarget.position, pLerp);
        transform.rotation = Quaternion.Lerp(transform.rotation, camTarget.rotation, rLerp);
    }
}
