// LaserPower.cs
using FreeflowCombatSpace;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLaserPower", menuName = "Powers/Offensive/Laser Power")]
public class LaserPower : Power
{
    [Header("Configurações do Laser")]
    public float damage = 50f;
    public float range = 100f;
    public float duration = 0.5f;
    public LineRenderer laserPrefab;
    public Material laserMaterial;
    public Color laserColor = Color.red;
    public float laserWidth = 0.1f;

    private LineRenderer currentLaser;
    private float timer;
    private bool isActive;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        FireLaser(user);
    }

    void FireLaser(GameObject user)
    {
        if (currentLaser == null)
        {
            GameObject laserObj = new GameObject("Laser");
            currentLaser = laserObj.AddComponent<LineRenderer>();
            currentLaser.material = laserMaterial;
            currentLaser.startColor = laserColor;
            currentLaser.endColor = laserColor;
            currentLaser.startWidth = laserWidth;
            currentLaser.endWidth = laserWidth;
            currentLaser.positionCount = 2;
        }

        currentLaser.enabled = true;
        RaycastHit hit;
        Vector3 origin = user.transform.position + user.transform.forward * 1f;

        if (Physics.Raycast(origin, user.transform.forward, out hit, range))
        {
            currentLaser.SetPosition(0, origin);
            currentLaser.SetPosition(1, hit.point);

            var health = hit.collider.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
        else
        {
            currentLaser.SetPosition(0, origin);
            currentLaser.SetPosition(1, origin + user.transform.forward * range);
        }

        isActive = true;
        timer = duration;
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        if (currentLaser != null)
        {
            currentLaser.enabled = false;
        }
        isActive = false;
    }
}