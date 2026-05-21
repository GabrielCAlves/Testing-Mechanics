using UnityEngine;

public class DamageOnPlayer : MonoBehaviour
{
    public int damage = 10;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            LifeSystem life = collision.gameObject.GetComponent<LifeSystem>();

            if (life != null)
            {
                life.TakeDamage(damage);
            }
        }
    }
}