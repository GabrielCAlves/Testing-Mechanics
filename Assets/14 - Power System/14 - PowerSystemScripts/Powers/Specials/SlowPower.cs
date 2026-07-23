// SlowPower.cs
using SceneScript;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSlowPower", menuName = "Powers/Offensive/Slow Power")]
public class SlowPower : Power
{
    [Header("Configurações de Lentidão")]
    public float slowFactor = 0.3f;
    public float slowDuration = 5f;
    public float slowRadius = 5f;
    public GameObject slowEffect;
    public Material slowMaterial;
    public Color slowColor = new Color(0, 0, 1, 0.3f);

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        ApplySlowArea(user);
    }

    void ApplySlowArea(GameObject user)
    {
        Collider[] targets = Physics.OverlapSphere(user.transform.position, slowRadius);

        foreach (var col in targets)
        {
            if (col.gameObject == user) continue;

            var movement = col.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.ApplySlow(slowFactor, slowDuration);
            }

            var enemy = col.GetComponent<Enemy14>();
            if (enemy != null)
            {
                enemy.ApplySlow(slowFactor, slowDuration);
            }

            // Efeito visual
            if (slowEffect != null)
            {
                GameObject effect = Instantiate(slowEffect, col.transform);
                effect.transform.localPosition = Vector3.zero;

                var renderer = effect.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = slowMaterial;
                    renderer.material.color = slowColor;
                }

                Destroy(effect, slowDuration);
            }
        }
    }
}