using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerInventoryItem : MonoBehaviour, IDamageable
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 15;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Components")]
    [SerializeField] private Transform attackPoint;


    [Header("Item Collection")]
    [SerializeField] private float collectionRange = 2f;
    [SerializeField] private LayerMask itemLayer; //

    private Camera mainCamera;

    private Rigidbody2D rb;  
    private InputSystem_Actions playerControls; 
    private Vector2 moveInput;  
    private LifeSystem lifeSystem;
    private Animator animator;
    private float lastAttackTime = 0f;
    private bool isAttacking = false;
    private bool isDead = false;

    private const string ANIM_IDLE = "Idle";
    private const string ANIM_WALK = "Walk";
    private const string ANIM_ATTACK = "Attack";
    private const string ANIM_DEAD = "Dead";

    void Awake()
    {
        playerControls = new InputSystem_Actions();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lifeSystem = GetComponent<LifeSystem>();
        animator = GetComponent<Animator>();

        if (rb == null)
            Debug.LogError("Rigidbody not found!");

        if (lifeSystem != null)
        {
            lifeSystem.maxHealth = maxHealth;
            lifeSystem.currentHealth = maxHealth;
        }
        else
        {
            currentHealth = maxHealth;
        }

        if (attackPoint == null)
        {
            GameObject point = new GameObject("AttackPoint");
            point.transform.parent = transform;
            point.transform.localPosition = new Vector3(1f, 0, 0);
            attackPoint = point.transform;
        }

        mainCamera = Camera.main;
    }

    //void OnEnable()
    //{
    //    playerControls.Player.Enable();

    //    playerControls.Player.Move.performed += OnMove;
    //    playerControls.Player.Move.canceled += OnMove;

    //    //playerControls.Player.Attack.performed += OnAttack;
    //}

    //void OnDisable()
    //{
    //    playerControls.Player.Move.performed -= OnMove;
    //    playerControls.Player.Move.canceled -= OnMove;
    //    //playerControls.Player.Attack.performed -= OnAttack;
    //    playerControls.Player.Disable();
    //}

    void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    //void OnAttack(InputAction.CallbackContext context)
    //{
    //    if (!isDead && !isAttacking && Time.time >= lastAttackTime + attackCooldown)
    //    {
    //        StartCoroutine(AttackCoroutine());
    //    }
    //}

    void Update()
    {
        if (isDead) return;

        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);

        if (animator != null)
        {
            bool isMoving = movement.magnitude > 0.1f;
            animator.SetBool(ANIM_WALK, isMoving);

            if (!isMoving)
                animator.SetBool(ANIM_IDLE, true);
            else
                animator.SetBool(ANIM_IDLE, false);
        }

        if (moveInput.x != 0)
        {
            FlipSprite(moveInput.x);
        }

        transform.Translate(movement * speed * Time.deltaTime);

        // Verifica itens para coletar
        CheckForNearbyItems(); // Só para caso de o Player e o Collectable não estiverem no mesmo espaço no mundo (Um no World Space e o outro no Canvas)
    }

    IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        if (animator != null)
            animator.SetTrigger(ANIM_ATTACK);

        yield return new WaitForSeconds(0.2f);

        PerformAttack();

        yield return new WaitForSeconds(0.3f);

        isAttacking = false;
    }

    void PerformAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
            }
        }
    }

    void FlipSprite(float direction)
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction);
        transform.localScale = scale;

        if (attackPoint != null)
        {
            Vector3 attackPointPos = attackPoint.localPosition;
            attackPointPos.x = Mathf.Abs(attackPointPos.x) * Mathf.Sign(direction);
            attackPoint.localPosition = attackPointPos;
        }
    }

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

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        moveInput = Vector2.zero;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        if (animator != null)
            animator.SetTrigger(ANIM_DEAD);

        Destroy(gameObject, 1.5f);
    }

    IEnumerator DamageFlash()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            Color originalColor = sprite.color;
            sprite.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sprite.color = originalColor;
        }
    }

    void CheckForNearbyItems()
    {
        // Converte a posição do player (UI) para mundo
        if (mainCamera == null) return;

        Vector3 playerWorldPos = mainCamera.ScreenToWorldPoint(transform.position);
        playerWorldPos.z = 0;

        // Busca itens próximos
        Collider2D[] nearbyItems = Physics2D.OverlapCircleAll(playerWorldPos, collectionRange, itemLayer);

        foreach (Collider2D item in nearbyItems)
        {
            Collectable collectable = item.GetComponent<Collectable>();
            if (collectable != null)
            {
                CollectItem(collectable);
                break; // Coleta um item por vez (opcional)
            }
        }
    }

    void CollectItem(Collectable collectable)
    {
        ItemData itemData = collectable.GetItemData();
        if (itemData == null) return;

        // Adiciona ao inventário
        Debug.Log($"Collected: {itemData.itemName}");

        collectable.Collect(GetComponent<Collider2D>());
        Destroy(collectable.gameObject);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }

        // Visualiza o range de coleta
        if (mainCamera != null && Application.isPlaying)
        {
            Vector3 playerWorldPos = mainCamera.ScreenToWorldPoint(transform.position);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(playerWorldPos, collectionRange);
        }
    }

    void OnDestroy()
    {
        if (playerControls != null)
        {
            playerControls.Player.Move.performed -= OnMove;
            playerControls.Player.Move.canceled -= OnMove;
            //playerControls.Player.Attack.performed -= OnAttack;
            playerControls.Dispose();
        }
    }
}