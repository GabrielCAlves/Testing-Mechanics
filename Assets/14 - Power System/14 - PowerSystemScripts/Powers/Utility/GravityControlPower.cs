// GravityControlPower.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGravityControlPower", menuName = "Powers/Utility/Gravity Control Power")]
public class GravityControlPower : Power
{
    [Header("Configurações de Gravidade")]
    public float gravityMultiplier = 0.2f;
    public float antiGravityRange = 8f;
    public float pullForce = 20f;
    public float pushForce = 30f;
    public GameObject gravityFieldEffect;
    public bool affectEnemies = true;
    public bool affectProjectiles = true;

    private bool isActive = false;
    private GameObject gravityField;
    private List<GameObject> affectedObjects = new List<GameObject>();
    private enum GravityMode { Normal, Anti, Push, Pull }
    private GravityMode currentMode = GravityMode.Normal;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        currentMode = GravityMode.Anti;
        ActivateAntiGravity(user);
    }

    void ActivateAntiGravity(GameObject user)
    {
        isActive = true;

        // Cria campo de gravidade
        if (gravityFieldEffect != null)
        {
            gravityField = Instantiate(gravityFieldEffect, user.transform.position, Quaternion.identity);
            gravityField.transform.localScale = Vector3.one * antiGravityRange;
        }

        // Afeta objetos na área
        Collider[] colliders = Physics.OverlapSphere(user.transform.position, antiGravityRange);
        foreach (var col in colliders)
        {
            if (col.gameObject == user) continue;

            if (affectEnemies && col.CompareTag("Enemy"))
            {
                var rb = col.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = false;
                    affectedObjects.Add(col.gameObject);
                }
            }

            if (affectProjectiles && col.CompareTag("Projectile"))
            {
                var rb = col.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = false;
                    affectedObjects.Add(col.gameObject);
                }
            }
        }
    }

    public void UpdateGravityControl(GameObject user)
    {
        if (!isActive) return;

        // Mantém campo centralizado
        if (gravityField != null)
        {
            gravityField.transform.position = user.transform.position;
        }

        // Modos de gravidade
        if (Input.GetKeyDown(KeyCode.G))
        {
            SwitchGravityMode(user);
        }

        // Aplica forças baseado no modo
        foreach (var obj in affectedObjects)
        {
            if (obj == null) continue;

            var rb = obj.GetComponent<Rigidbody>();
            if (rb == null) continue;

            Vector3 direction = obj.transform.position - user.transform.position;

            switch (currentMode)
            {
                case GravityMode.Anti:
                    rb.AddForce(-Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
                    break;
                case GravityMode.Push:
                    rb.AddForce(direction.normalized * pushForce, ForceMode.Impulse);
                    break;
                case GravityMode.Pull:
                    rb.AddForce(-direction.normalized * pullForce, ForceMode.Impulse);
                    break;
            }
        }
    }

    void SwitchGravityMode(GameObject user)
    {
        switch (currentMode)
        {
            case GravityMode.Normal:
                currentMode = GravityMode.Anti;
                break;
            case GravityMode.Anti:
                currentMode = GravityMode.Push;
                break;
            case GravityMode.Push:
                currentMode = GravityMode.Pull;
                break;
            case GravityMode.Pull:
                currentMode = GravityMode.Normal;
                break;
        }

        // Efeito visual de mudança
        if (gravityField != null)
        {
            var renderer = gravityField.GetComponent<Renderer>();
            if (renderer != null)
            {
                Color color = renderer.material.color;
                switch (currentMode)
                {
                    case GravityMode.Anti: color = Color.blue; break;
                    case GravityMode.Push: color = Color.red; break;
                    case GravityMode.Pull: color = Color.green; break;
                    case GravityMode.Normal: color = Color.white; break;
                }
                renderer.material.color = color;
            }
        }
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        isActive = false;

        // Restaura gravidade dos objetos
        foreach (var obj in affectedObjects)
        {
            if (obj != null)
            {
                var rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = true;
                }
            }
        }
        affectedObjects.Clear();

        if (gravityField != null)
        {
            Destroy(gravityField);
        }
    }
}