using UnityEngine;

public class RightCube : MonoBehaviour
{
    [Header("Horizontal Direction")]
    [SerializeField] private bool right = false;
    [SerializeField] private bool left = false;
    [SerializeField] private float speed = 5f;

    // Update is called once per frame
    void Update()
    {
        if (right)
            transform.position += transform.right * speed * Time.deltaTime;

        if (left)
            transform.position += -transform.right * speed * Time.deltaTime;
    }
}
