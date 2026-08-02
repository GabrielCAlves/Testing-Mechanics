using System.Collections;
using UnityEngine;

public class HitBox2D : MonoBehaviour
{
    [SerializeField] private string targetTag = "Enemy";
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private bool knockback = false;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private Vector2 knockbackDirection;
    [SerializeField] private float knockbackSpeed = 5f;

    private LifeSystem enemyLifeSystem;
    private Rigidbody2D enemyRb2D;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("OnCollisionEnter2D activated!");
        Debug.Log("collision.gameObject.name = " + collision.gameObject.name);
        Debug.Log("collision.transform.tag = " + collision.transform.tag);
        Debug.Log("collision.gameObject.GetComponent<LifeSystem>() = " + collision.gameObject.GetComponent<LifeSystem>());

        if (collision.transform.CompareTag(targetTag) || collision.gameObject.layer == LayerMask.NameToLayer(targetTag))
        {
            if(!collision.gameObject.GetComponent<LifeSystem>())
            {
                Debug.Log($"{collision.gameObject.name} doesn't have LifeSystem.");
                return;
            }

            enemyLifeSystem = collision.gameObject.GetComponent<LifeSystem>();
            enemyLifeSystem.TakeDamage((int) damageAmount);

            if(knockback)
            {
                knockbackDirection = (collision.transform.position - transform.position).normalized;

                if (collision.gameObject.GetComponent<Rigidbody2D>() != null)
                {
                    enemyRb2D = collision.gameObject.GetComponent<Rigidbody2D>();
                    Knockback();
                }
                else
                {
                    collision.transform.position = Vector2.Lerp(collision.transform.position, (Vector2)collision.transform.position + knockbackDirection, knockbackSpeed * Time.deltaTime);
                    Debug.Log($"{collision.gameObject.name} doesn't have Rigidbody2D.");
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter2D activated!");
        Debug.Log("collision.gameObject.name = " + collision.gameObject.name);
        Debug.Log("collision.transform.tag = " + collision.transform.tag);
        Debug.Log("collision.gameObject.GetComponent<LifeSystem>() = " + collision.gameObject.GetComponent<LifeSystem>());

        if (collision.transform.CompareTag(targetTag) || collision.gameObject.layer == LayerMask.NameToLayer(targetTag))
        {
            if (!collision.gameObject.GetComponent<LifeSystem>())
            {
                Debug.Log($"{collision.gameObject.name} doesn't have LifeSystem.");
                return;
            }

            enemyLifeSystem = collision.gameObject.GetComponent<LifeSystem>();
            enemyLifeSystem.TakeDamage((int)damageAmount);

            if (knockback)
            {
                knockbackDirection = (collision.transform.position - transform.position).normalized;

                if (collision.gameObject.GetComponent<Rigidbody2D>() != null)
                {
                    enemyRb2D = collision.gameObject.GetComponent<Rigidbody2D>();
                    Knockback();
                }
                else
                {
                    collision.transform.position = Vector2.Lerp(collision.transform.position, (Vector2)collision.transform.position + knockbackDirection, knockbackSpeed * Time.deltaTime);
                    Debug.Log($"{collision.gameObject.name} doesn't have Rigidbody2D.");
                }
            }
        }
    }

    private void Knockback()
    {
        enemyRb2D.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
    }
}
