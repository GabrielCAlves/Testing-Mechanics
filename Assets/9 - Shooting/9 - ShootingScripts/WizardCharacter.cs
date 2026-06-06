using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.InputSystem;

public class WizardCharacter : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private bool invertedSprite = false;

    [SerializeField] private float jumpForce = 8f;
    [SerializeField] GameObject groundRayObject;
    [SerializeField] private bool jumpAvailable = false;

    [SerializeField] Transform shootPoint;
    [SerializeField] GameObject attack1Prefab;
    [SerializeField] GameObject attack2Prefab;
    [SerializeField] Quaternion attack2Rotation;
    [SerializeField] private float attack1SpeedForce = 5f;
    [SerializeField] private float attack2SpeedForce = 5f;
    [SerializeField] private float rightRotation = 90;
    [SerializeField] private float leftRotation = 270;

    private Rigidbody2D rb;
    private float directionInput;
    private Animator animator;
    private bool alreadyTriggered;

    private const string ANIM_IDLE = "Idle";
    private const string ANIM_RUN = "Run";
    private const string ANIM_JUMP = "Jump";
    private const string ANIM_ATTACK_1 = "Attack_1";
    private const string ANIM_ATTACK_2 = "Attack_2";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (rb == null)
            Debug.LogWarning("Rigidbody not found!");
    }

    void Update()
    {
        MovePlayer();
        Inputs();
    }

    private void FixedUpdate()
    {
        //MovePlayer();

        RaycastHit2D hit = Physics2D.Raycast(groundRayObject.transform.position, Vector2.down);

        Debug.DrawRay(groundRayObject.transform.position, Vector2.down * hit.distance, Color.red);

        if (hit.collider != null)
        {
            if (hit.distance <= .2f)
            {
                jumpAvailable = true; 
                
                if (animator != null && !alreadyTriggered)
                {
                    animator.SetTrigger(ANIM_IDLE);
                    alreadyTriggered = true;
                }
            }
            else
            {
                jumpAvailable = false;

                alreadyTriggered = false;
            }
        }
    }

    void Inputs()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            animator.SetTrigger(ANIM_ATTACK_1);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            animator.SetTrigger(ANIM_ATTACK_2);
        }

        if (Input.GetKeyDown(KeyCode.Space) && jumpAvailable && groundRayObject != null)
        {
            animator.SetTrigger(ANIM_JUMP);
            rb.linearVelocity = Vector2.up * jumpForce;
        }
    }

    void MovePlayer()
    {
        // Movimento horizontal
        directionInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(directionInput * speed, rb.linearVelocity.y);

        if (directionInput != 0)
        {
            FlipSprite(directionInput);

            if (animator != null)
            {
                animator.SetBool(ANIM_RUN, true);
            }
        }
        else
        {
            if (animator != null)
            {
                animator.SetBool(ANIM_RUN, false);
            }
        }
    }

    void FlipSprite(float direction)
    {
        Vector3 scale = transform.localScale;

        if (invertedSprite)
        {
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction) * -1;
        }
        else
        {
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction);
        }

        transform.localScale = scale;
    }

    public void Attack1()
    {
        GameObject shootAttack = Instantiate(attack1Prefab, shootPoint);
        shootAttack.transform.SetParent(null);

        float directionMultiplier = transform.localScale.x < 0 ? -1 : 1;

        shootAttack.transform.localRotation = new Quaternion(attack2Rotation.x, attack2Rotation.y, attack2Rotation.z * directionMultiplier, attack2Rotation.w);

        shootAttack.GetComponent<Rigidbody2D>().linearVelocity = shootPoint.right * transform.localScale.x * attack1SpeedForce;

        Destroy(shootAttack, 1.5f);
    }

    public void Attack2()
    {
        GameObject shootAttack = Instantiate(attack2Prefab, shootPoint);
        shootAttack.transform.SetParent(null);

        float directionMultiplier = transform.localScale.x < 0 ? -1 : 1;

        shootAttack.transform.localRotation = new Quaternion(attack2Rotation.x, attack2Rotation.y, attack2Rotation.z * directionMultiplier, attack2Rotation.w);

        shootAttack.GetComponent<Rigidbody2D>().linearVelocity = shootPoint.right * transform.localScale.x * attack2SpeedForce;

        Destroy(shootAttack, 2f);
    }
}
