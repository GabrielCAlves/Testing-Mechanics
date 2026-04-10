using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyInventoryItem : MonoBehaviour, IDamageable
{
    [Header("Enemy Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float detectionRange = 5f;

    [Header("Drop System")]
    [SerializeField] private List<ItemDrop> dropItems = new List<ItemDrop>();
    [SerializeField] private GameObject dropPrefab;

    [Header("Components")]
    [SerializeField] private Transform playerTarget;

    private LifeSystem lifeSystem;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isAttacking = false;
    private float lastAttackTime = 0f;
    private bool isDead = false;

    void Start()
    {
        lifeSystem = GetComponent<LifeSystem>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTarget = player.transform;
        }

        Debug.Log($"Enemy {gameObject.name} initialized. Drops: {dropItems.Count}");
    }

    void Update()
    {
        if (isDead) return;

        if (playerTarget != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

            if (distanceToPlayer <= attackRange)
            {
                Attack();
            }
            else if (distanceToPlayer <= detectionRange)
            {
                MoveTowardsPlayer();
            }
            else
            {
                Idle();
            }
        }
    }

    void MoveTowardsPlayer()
    {
        if (isAttacking) return;

        Vector2 direction = (playerTarget.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
        FlipSprite(direction.x);

        if (animator != null)
            animator.SetBool("Walk", true);
    }

    void Idle()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        if (animator != null)
            animator.SetBool("Walk", false);
    }

    void Attack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;
        if (isAttacking) return;

        lastAttackTime = Time.time;
        StartCoroutine(AttackCoroutine());
    }

    IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        if (animator != null)
            animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.3f);

        if (playerTarget != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);
            if (distanceToPlayer <= attackRange + 0.5f)
            {
                IDamageable damageable = playerTarget.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(attackDamage);
                }
            }
        }

        yield return new WaitForSeconds(attackCooldown - 0.3f);
        isAttacking = false;
    }

    void FlipSprite(float direction)
    {
        if (direction != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction);
            transform.localScale = scale;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        if (lifeSystem != null)
        {
            lifeSystem.TakeDamage(damage);
            Debug.Log($"Enemy took {damage} damage. Health: {lifeSystem.currentHealth}/{lifeSystem.maxHealth}");
        }
        else
        {
            Debug.LogWarning($"Enemy {gameObject.name} has no LifeSystem!");
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log($"Enemy {gameObject.name} died! Dropping items...");

        rb.linearVelocity = Vector2.zero;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        if (animator != null)
            animator.SetTrigger("Dead");

        DropItems();

        Destroy(gameObject, 1.5f);
    }

    public void DropItems()
    {
        if (dropItems.Count == 0)
        {
            return;
        }

        Debug.Log($"Dropping items from {gameObject.name}. Total drop entries: {dropItems.Count}");

        foreach (ItemDrop drop in dropItems)
        {
            if (drop.itemData == null)
            {
                Debug.LogError($"ItemData is null in drop entry for {gameObject.name}");
                continue;
            }

            float randomValue = Random.Range(0f, 100f);
            Debug.Log($"Drop: {drop.itemData.itemName}, Chance: {drop.dropChance}%, Roll: {randomValue}");

            if (randomValue <= drop.dropChance)
            {
                int quantityToDrop = Random.Range(drop.minQuantity, drop.maxQuantity + 1);
                Debug.Log($"Dropping {quantityToDrop}x {drop.itemData.itemName}");

                for (int i = 0; i < quantityToDrop; i++)
                {
                    CreateDropItem(drop.itemData);
                }
            }
        }
    }

    void CreateDropItem(ItemData itemData) // Criado para centralizar a lógica de criação do item dropado, do Enemy no Canvas para os drops no World Space
    {
        if (itemData.worldPrefab == null)
        {
            Debug.LogError($"Cannot drop {itemData.itemName}! No worldPrefab assigned in ItemData.");
            return;
        }

        GameObject droppedItem = Instantiate(itemData.worldPrefab);

        // O enemy está no Canvas, mas o drop vai para o World Space
        // Converte a posição do UI enemy para coordenadas do mundo

        RectTransform enemyRect = GetComponent<RectTransform>();
        if (enemyRect != null)
        {
            // Converte a posição âncora do UI para posição do mundo
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(enemyRect.position);

            // Ajusta a profundidade (Z axis) se necessário
            worldPosition.z = 0;

            // Adiciona um offset aleatório
            Vector2 randomOffset = Random.insideUnitCircle * 0.5f;
            worldPosition += (Vector3)randomOffset;

            droppedItem.transform.position = worldPosition;

            Debug.Log($"Dropped item at world position: {worldPosition} (converted from UI position: {enemyRect.position})");
        }
        else
        {
            // Fallback caso não encontre RectTransform
            Vector2 randomOffset = Random.insideUnitCircle * 0.5f;
            droppedItem.transform.position = transform.position + (Vector3)randomOffset;
            Debug.LogWarning($"Enemy has no RectTransform, using transform.position: {droppedItem.transform.position}");
        }

        // Configura o Collectable
        Collectable collectable = droppedItem.GetComponent<Collectable>();
        if (collectable != null)
        {
            if (collectable.GetItemData() == null)
            {
                collectable.SetItemData(itemData);
            }
            Debug.Log($"Item dropped successfully: {itemData.itemName} at world position {droppedItem.transform.position}");
        }
        else
        {
            Debug.LogError($"Dropped item {droppedItem.name} has no Collectable component!");
            Destroy(droppedItem);
        }
    }

    //void CreateDropItem(ItemData itemData) // Criado para centralizar a lógica de criação do item dropado, tanto para UI quanto para World Space
    //{
    //    if (itemData.worldPrefab == null)
    //    {
    //        Debug.LogError($"Cannot drop {itemData.itemName}! No worldPrefab assigned in ItemData.");
    //        return;
    //    }

    //    GameObject droppedItem = Instantiate(itemData.worldPrefab);

    //    // Verifica se o enemy está em um Canvas (UI) ou no mundo
    //    bool isEnemyInCanvas = GetComponent<RectTransform>() != null &&
    //                           GetComponentInParent<Canvas>() != null;

    //    if (isEnemyInCanvas)
    //    {
    //        // Lógica para UI (Canvas)
    //        RectTransform rectTransform = droppedItem.GetComponent<RectTransform>();
    //        if (rectTransform == null)
    //        {
    //            Debug.LogError($"Dropped item {droppedItem.name} has no RectTransform! Cannot place in Canvas.");
    //            Destroy(droppedItem);
    //            return;
    //        }

    //        RectTransform enemyRect = GetComponent<RectTransform>();
    //        if (enemyRect != null && enemyRect.parent != null)
    //        {
    //            rectTransform.SetParent(enemyRect.parent);
    //            rectTransform.anchoredPosition = enemyRect.anchoredPosition;

    //            Vector2 randomOffset = Random.insideUnitCircle * 0.5f;
    //            rectTransform.anchoredPosition += randomOffset;
    //        }
    //        else
    //        {
    //            // Fallback - posiciona no centro da tela ou posição default
    //            rectTransform.anchoredPosition = Vector2.zero;
    //            Vector2 randomOffset = Random.insideUnitCircle * 0.5f;
    //            rectTransform.anchoredPosition += randomOffset;
    //        }
    //    }
    //    else
    //    {
    //        // Lógica para World Space
    //        Vector2 randomOffset = Random.insideUnitCircle * 0.5f;
    //        droppedItem.transform.position = transform.position + (Vector3)randomOffset;
    //    }

    //    // Configura o Collectable
    //    Collectable collectable = droppedItem.GetComponent<Collectable>();
    //    if (collectable != null)
    //    {
    //        if (collectable.GetItemData() == null)
    //        {
    //            collectable.SetItemData(itemData);
    //        }
    //        Debug.Log($"Item dropped successfully: {itemData.itemName} at position {droppedItem.transform.position}");
    //    }
    //    else
    //    {
    //        Debug.LogError($"Dropped item {droppedItem.name} has no Collectable component!");
    //        Destroy(droppedItem);
    //    }
    //}
}

[System.Serializable]
public class ItemDrop
{
    public ItemData itemData;
    [Range(0, 100)] public float dropChance = 50f;
    public int minQuantity = 1;
    public int maxQuantity = 1;
}