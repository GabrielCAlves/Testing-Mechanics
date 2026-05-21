using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class Direction8MovementCube : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private float horizontal;
    [SerializeField] private float vertical;
    [SerializeField] private float speed = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (rigidbody != null)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(horizontal, 0, vertical).normalized * speed * Time.deltaTime;

            rigidbody.linearVelocity = movement;
        }
    }
}
