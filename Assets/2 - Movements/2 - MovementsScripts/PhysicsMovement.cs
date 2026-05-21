using UnityEngine;

public class PhysicsMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float forceMovement = 10f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private bool isGrounded;

    [Header("Input Debug")]
    [SerializeField] private float horizontal;
    [SerializeField] private float vertical;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(rb == null) rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.Space) && Input.GetKeyDown(KeyCode.F) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Force);
        }

        if (Input.GetKeyDown(KeyCode.Space) && Input.GetKeyDown(KeyCode.D) && isGrounded)
        {
            rb.AddForce(Vector3.down * jumpForce, ForceMode.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.Space) && Input.GetKeyDown(KeyCode.R) && isGrounded)
        {
            rb.AddForce(Vector3.right * jumpForce, ForceMode.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.Space) && Input.GetKeyDown(KeyCode.L) && isGrounded)
        {
            rb.AddForce(Vector3.left * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0, vertical) * forceMovement;
        
        rb.AddForce(movement);
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
