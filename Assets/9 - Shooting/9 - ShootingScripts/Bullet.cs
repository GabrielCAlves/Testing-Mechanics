using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float timeToDestroy = 1f;
    [SerializeField] private bool toDestroy = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(toDestroy)
            Destroy(gameObject, timeToDestroy);
    }

    //private void Shoot()
    //{
    //    GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
    //    Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
    //    bulletRb.AddForce(shootPoint.forward * shootForce, ForceMode.Impulse);

    //    Destroy(bullet, timeToDestroy);
    //}

    private void OnCollisionEnter(Collision collision)
    {
        if(!collision.transform.CompareTag("Player") && !collision.transform.CompareTag("Bullet"))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        Debug.Log("What did this object collided with? Answer: " + collision.transform.name);
    }
}
