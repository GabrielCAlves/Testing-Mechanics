using UnityEngine;

public class ForwardCube : MonoBehaviour
{
    [Header("Vertical Direction")]
    [SerializeField] private bool forward = false;
    [SerializeField] private bool back = false;
    [SerializeField] private float speed = 5f;

    // Update is called once per frame
    void Update()
    {
        if (forward)
            transform.position += transform.forward * speed * Time.deltaTime;

        if(back)
            transform.position += -transform.forward * speed * Time.deltaTime;
    }
}
