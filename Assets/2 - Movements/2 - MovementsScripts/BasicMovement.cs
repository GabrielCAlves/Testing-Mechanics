using Unity.VisualScripting;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private Rigidbody rb;

    [Header("Movement Debug")]
    [SerializeField] private float horizontal;
    [SerializeField] private float vertical;
    [SerializeField] private Vector3 movement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(rb == null) rb = GetComponent<Rigidbody>();
        else Debug.LogWarning("Rigidbody component not found on " + gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        movement = new Vector3(horizontal, 0, vertical);
        //movement = new Vector3(horizontal, 0f, vertical).normalized * speed;

        transform.Translate(movement * speed * Time.deltaTime);
        //transform.Translate(movement * speed * Time.deltaTime, Space.World);

        FlipSprite(horizontal);
    }

    void FlipSprite(float direction)
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction);
        transform.localScale = scale;
    }
}
