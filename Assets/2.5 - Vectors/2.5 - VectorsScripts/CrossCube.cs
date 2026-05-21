using UnityEngine;

public class CrossCube : MonoBehaviour
{
    [Header("Vertical Direction")]
    [SerializeField] private bool forward = false;
    [SerializeField] private bool back = false;

    [Header("Horizontal Direction")]
    [SerializeField] private bool right = false;
    [SerializeField] private bool left = false;

    [Header("Horizontal Direction")]
    [SerializeField] private bool up = false;
    [SerializeField] private bool down = false;

    [Header("Speed")]
    [SerializeField] private float speed = 5f;

    // Update is called once per frame
    void Update()
    {
        if (right)                              // Thumb      // Pointer finger
            transform.position += Vector3.Cross(transform.up, transform.forward) * speed * Time.deltaTime;

        if (left)
            transform.position += Vector3.Cross(-transform.up, transform.forward) * speed * Time.deltaTime;

        if (forward)
            transform.position += Vector3.Cross(-transform.right, -transform.up) * speed * Time.deltaTime;

        if (back)
            transform.position += Vector3.Cross(-transform.right, transform.up) * speed * Time.deltaTime;

        if (up)
            transform.position += Vector3.Cross(-transform.right, transform.forward) * speed * Time.deltaTime;

        if (down)
            transform.position += Vector3.Cross(transform.right, transform.forward) * speed * Time.deltaTime;
    }
}
