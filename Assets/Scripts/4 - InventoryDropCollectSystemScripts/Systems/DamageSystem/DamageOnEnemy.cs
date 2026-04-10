using UnityEngine;

public class DamageOnEnemy : MonoBehaviour
{
    public int damage = 20;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            LifeSystem life = collision.gameObject.GetComponent<LifeSystem>();

            if (life != null)
            {
                life.TakeDamage(damage);
            }
        }
    }
}