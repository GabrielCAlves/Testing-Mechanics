// PoisonPower.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewPoisonPower", menuName = "Powers/Offensive/Poison Power")]
public class PoisonPower : Power
{
    [Header("Configurações do Veneno")]
    public float poisonDamage = 10f;
    public float poisonDuration = 5f;
    public float tickInterval = 1f;
    public float areaRadius = 3f;
    public GameObject poisonCloudPrefab;
    public Material poisonMaterial;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        CreatePoisonArea(user);
    }

    void CreatePoisonArea(GameObject user)
    {
        Collider[] colliders = Physics.OverlapSphere(user.transform.position, areaRadius);
        foreach (var collider in colliders)
        {
            var health = collider.GetComponent<Health>();
            if (health != null && collider.gameObject != user)
            {
                health.ApplyPoison(poisonDamage, poisonDuration, tickInterval);
            }
        }

        if (poisonCloudPrefab != null)
        {
            GameObject cloud = Instantiate(poisonCloudPrefab, user.transform.position, Quaternion.identity);
            cloud.transform.localScale = Vector3.one * areaRadius;
            Destroy(cloud, poisonDuration);
        }
    }
}