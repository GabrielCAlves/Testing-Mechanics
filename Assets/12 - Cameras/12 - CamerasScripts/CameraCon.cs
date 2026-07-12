using UnityEngine;

public class CameraCon : MonoBehaviour
{
    [SerializeField] private Transform camTarget;
    [SerializeField] private float pLerp;
    [SerializeField] private float rLerp;
    [SerializeField] private bool allowPosLerp = true;
    [SerializeField] private bool allowRotLerp = true;

    void Update()
    {
        if(allowPosLerp)
            transform.position = Vector3.Lerp(transform.position, camTarget.position, pLerp);

        if(allowRotLerp)
            transform.rotation = Quaternion.Lerp(transform.rotation, camTarget.rotation, rLerp);
    }
}
