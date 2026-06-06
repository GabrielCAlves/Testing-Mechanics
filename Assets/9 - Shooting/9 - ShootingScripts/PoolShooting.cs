using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolShooting : MonoBehaviour
{
    [Header("Shooting Configuration")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float shootForce;

    [Header("Pool Object")]
    [SerializeField] private int poolSize = 10;
    [SerializeField] private List<GameObject> bulletPool;
    [SerializeField] private float timeToDeactivate = 2f;

    [Header("Reload Waiting Effect")]
    [SerializeField] private bool useReloadTime = false;
    [SerializeField] private float reloadTime = 2f;

    private int shootCount = 0;
    private bool reloading = false;

    private void Start()
    {
        for(int i = 0; i < poolSize; ++i)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Add(bullet);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if(useReloadTime && shootCount == bulletPool.Count && !reloading)
            {
                reloading = true;

                StartCoroutine(Reload());
            }
            else if(useReloadTime && shootCount == bulletPool.Count && reloading)
            {
                Debug.Log("Reloading bullets...");
            }
            else
            {
                ShootPool();
            }
        }
    }

    private void ShootPool()
    {
        foreach(GameObject bullet in bulletPool)
        {
            if (!bullet.activeInHierarchy)
            {
                bullet.transform.position = shootPoint.transform.position;
                bullet.transform.rotation = shootPoint.transform.rotation;
                bullet.SetActive(true);
                ApplyForce(bullet);

                if (useReloadTime)
                    ++shootCount;

                return;
            }
        }
    }

    private void ApplyForce(GameObject bullet)
    {
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>(); 
        Vector3 shootDirection = shootPoint.transform.forward;
        
        // AddForce with Impulse mode can cause issues with the bullet's velocity when reusing it from the pool, as it may not reset properly. Instead, we can directly set the velocity of the bullet to ensure consistent behavior.
        //bulletRb.AddForce(shootDirection * shootForce, ForceMode.Impulse);
        bulletRb.linearVelocity = shootDirection * shootForce;

        StartCoroutine(DeactivateBullet(bullet));
    }

    IEnumerator DeactivateBullet(GameObject bullet)
    {
        yield return new WaitForSeconds(timeToDeactivate);

        bullet.SetActive(false);
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadTime);

        shootCount = 0;
        reloading = false;
    }
}
