using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class AnimationHandlerScript : MonoBehaviour, IDamageable
{
    [Header("General Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 15;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Collection Settings")]
    [SerializeField] private Collectable[] worldItems;
    [SerializeField] private Vector3 screenPos;
    [SerializeField] private RectTransform playerRect;
    [SerializeField] private float collectionRange = 100f;

    private Rigidbody2D rb;

    private LifeSystem lifeSystem;
    private Animator animator;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputAction moveAction;
    [SerializeField] private InputAction jumpAction;
    
    private Vector2 direction;

    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private bool isJumping;

    private bool isDead = false;

    [SerializeField] private bool invertedSprite = false;
    private const string ANIM_RUN = "Run";
    private const string ANIM_JUMP = "Jump";
    private const string ANIM_DIE = "Die";
    private const string ANIM_ATTACK_1 = "Attack_1";
    private const string ANIM_ATTACK_2 = "Attack_2";

    private bool isMoving;
    private Collider2D col;
    private SpriteRenderer sprite;
    private Color originalColor;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lifeSystem = GetComponent<LifeSystem>();
        animator = GetComponent<Animator>();

        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];

        if (rb == null)
            Debug.LogWarning("Rigidbody not found!");

        if (lifeSystem != null)
        {
            lifeSystem.maxHealth = maxHealth;
            lifeSystem.currentHealth = maxHealth;
        }
        else
        {
            currentHealth = maxHealth;
        }
    }

    void Update()
    {
        if (isDead) return;

        MovePlayer();

        Inputs();
        //WorldCollection();
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
    }

    void MovePlayer()
    {
        // Movimento horizontal
        direction = moveAction.ReadValue<Vector2>();
        transform.position += new Vector3(direction.x, 0, direction.y) * speed * Time.deltaTime;

        if (direction.x != 0)
        {
            FlipSprite(direction.x);
        }

        if (animator != null)
        {
            isMoving = direction.magnitude > 0.1f;

            if (!isMoving)
                animator.SetBool(ANIM_RUN, false);
            else
                animator.SetBool(ANIM_RUN, true);
        }

        // Verifica se está no chão e reseta a velocidade vertical
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Pequeno valor para manter no chão
            isJumping = false;
        }

        if (isGrounded && jumpAction.triggered && !isJumping)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            isJumping = true;
            animator.SetBool(ANIM_JUMP, isJumping);
            Debug.Log("Pulo executado! Velocidade Y: " + velocity.y);
        }

        // Aplicar gravidade
        velocity.y += gravity * Time.deltaTime;

        // Aplicar movimento vertical SEPARADO do horizontal
        if (isJumping) transform.position += new Vector3(0, velocity.y, 0) * Time.deltaTime;

        WorldCollection();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = false;
            animator.SetBool(ANIM_JUMP, isJumping);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;

            // Resetar pulo ao tocar no chão
            if (velocity.y <= 0)
            {
                isJumping = false;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    // Collecting item from 2D world to 3D world
    void WorldCollection()
    {
        worldItems = GameObject.FindObjectsOfType<Collectable>();

        foreach (Collectable item in worldItems)
        {
            screenPos = Camera.main.WorldToScreenPoint(item.transform.position);
            playerRect = GetComponent<RectTransform>();

            float distance = Vector2.Distance(screenPos, playerRect.position);
            if (distance < collectionRange)
            {
                item.Collect(GetComponent<Collider2D>());
            }
        }
    }

    void FlipSprite(float direction)
    {
        Vector3 scale = transform.localScale;
        //scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction);

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

    // Take damage handler
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        if (lifeSystem != null)
        {
            lifeSystem.TakeDamage(damage);

            if (lifeSystem.currentHealth <= 0)
            {
                Die();
            }
        }
        else
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        StartCoroutine(DamageFlash());
    }

    // Die handler
    public void Die()
    {
        if (isDead) return;

        isDead = true;

        direction = Vector2.zero;

        col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        if (animator != null)
            animator.SetTrigger(ANIM_DIE);

        Destroy(gameObject, 1.5f);
    }

    IEnumerator DamageFlash()
    {
        sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            originalColor = sprite.color;
            sprite.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sprite.color = originalColor;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, collectionRange);
    }
}
