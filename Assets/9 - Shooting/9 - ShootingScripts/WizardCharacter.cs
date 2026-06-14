using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class WizardCharacter : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private bool invertedSprite = false;

    [Header("Jump Configurations")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] GameObject groundRayObject;
    [SerializeField] private bool jumpAvailable = false;

    [Header("Shoot Attack Configurations")]
    [SerializeField] Transform shootPoint;
    [SerializeField] GameObject attack1Prefab;
    [SerializeField] GameObject attack2Prefab;
    [SerializeField] Quaternion attackRotation;
    [SerializeField] private float attack1SpeedForce = 5f;
    [SerializeField] private float attack2SpeedForce = 5f;

    [Header("Rigidbody and Input Set")]
    private Rigidbody2D rb2D;
    private Rigidbody rb3D;
    private float horizontalDirectionInput;
    private float verticalDirectionInput;

    [Header("Animator Set")]
    private Animator animator;
    private bool alreadyTriggered;

    [Header("Animations")]
    private const string ANIM_IDLE = "Idle";
    private const string ANIM_RUN = "Run";
    private const string ANIM_JUMP = "Jump";
    private const string ANIM_ATTACK_1 = "Attack_1";
    private const string ANIM_ATTACK_2 = "Attack_2";

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        rb3D = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        if (rb2D == null)
            Debug.LogWarning("Rigidbody (2D) not found!");
        if (rb3D == null)
            Debug.LogWarning("Rigidbody (3D) not found!");
    }

    void Update()
    {
        MovePlayer();
        Inputs();
    }

    private void FixedUpdate()
    {
        if (groundRayObject == null)
            return;

        // 2D physics
        if (rb2D != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(groundRayObject.transform.position, Vector2.down);
            Debug.DrawRay(groundRayObject.transform.position, Vector2.down * (hit.collider != null ? hit.distance : 1f), Color.red);

            JumpHandler(hit, null, true);
        }
        else // 3D physics
        {
            RaycastHit hit = new RaycastHit();
            bool hasHit = Physics.Raycast(groundRayObject.transform.position, Vector3.down, out hit);
            Debug.DrawRay(groundRayObject.transform.position, Vector3.down * (hasHit ? hit.distance : 1f), Color.red);
            
            JumpHandler(null, hit, hasHit);
        }
    }

    private void JumpHandler(RaycastHit2D? hit2D, RaycastHit? hit3D, bool hasHit)
    {
        if ((rb2D != null && hit2D?.collider == null) || !hasHit)
        {
            jumpAvailable = false;
            alreadyTriggered = false;
            return;
        }

        if (hit2D?.distance <= .2f || hit3D?.distance <= .2f)
        {
            jumpAvailable = true;

            if (!alreadyTriggered)
            {
                alreadyTriggered = true;
            }

            if (animator != null)
            {
                animator.SetTrigger(ANIM_IDLE);
            }
        }
        else
        {
            jumpAvailable = false;
            alreadyTriggered = false;
        }

        //bool hit2DNull = !hit2D.HasValue || hit2D.Value.collider == null;
        //if (hit2DNull || !hasHit)
        //{
        //    jumpAvailable = false;
        //    alreadyTriggered = false;
        //    return;
        //}

        //float hit2DDistance = hit2D.Value.distance;
        //float hit3DDistance = hit3D.HasValue ? hit3D.Value.distance : float.MaxValue;
        //if (hit2DDistance <= .2f || hit3DDistance <= .2f)
        //{
        //    jumpAvailable = true;

        //    if (animator != null && !alreadyTriggered)
        //    {
        //        animator.SetTrigger(ANIM_IDLE);
        //        alreadyTriggered = true;
        //    }
        //}
        //else
        //{
        //    jumpAvailable = false;
        //    alreadyTriggered = false;
        //}
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
            if (animator != null)
                animator.SetTrigger(ANIM_JUMP);

            if (rb2D != null)
            {
                rb2D.linearVelocity = Vector2.up * jumpForce;
            }
            else if (rb3D != null)
            {
                //Debug.Log("Space key pressed! rb3D.linearVelocity.y = "+ rb3D.linearVelocity.y + ". jumpForce = "+ jumpForce);
                //rb3D.linearVelocity = new Vector3(rb3D.linearVelocity.x, jumpForce, rb3D.linearVelocity.z);
                rb3D.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }

    void MovePlayer()
    {
        horizontalDirectionInput = Input.GetAxis("Horizontal");
        if (rb2D != null)
        {
            rb2D.linearVelocity = new Vector2(horizontalDirectionInput * speed, rb2D.linearVelocity.y);
        }
        else if (rb3D != null)
        {
            verticalDirectionInput = Input.GetAxis("Vertical");
            //Vector3 movement = new Vector3(horizontal, 0, vertical) * forceMovement;

            //rb3D.linearVelocity = new Vector3(horizontalDirectionInput * speed, rb3D.linearVelocity.y, verticalDirectionInput * speed).normalized;
            rb3D.AddForce(new Vector3(0, 0, verticalDirectionInput) * speed);
        }

        if (horizontalDirectionInput != 0)
        {
            if(rb2D != null)
                FlipSprite(horizontalDirectionInput);

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
        ShootAttack(attack1Prefab, attack1SpeedForce, 1.5f);
    }

    public void Attack2()
    {
        ShootAttack(attack2Prefab, attack2SpeedForce, 2f);
    }

    public void ShootAttack(GameObject attackPrefab, float attackSpeedForce, float timeToDestroy)
    {
        GameObject shootAttack = Instantiate(attackPrefab, shootPoint);
        shootAttack.transform.SetParent(null);

        float directionMultiplier = transform.localScale.x < 0 ? -1 : 1;
                                                             // 0              //0               // 90 in inspector
        shootAttack.transform.localRotation = new Quaternion(attackRotation.x, attackRotation.y, attackRotation.z * directionMultiplier, attackRotation.w);

        shootAttack.GetComponent<Rigidbody2D>().linearVelocity = shootPoint.right * transform.localScale.x * attackSpeedForce;

        Destroy(shootAttack, timeToDestroy);
    }
}
