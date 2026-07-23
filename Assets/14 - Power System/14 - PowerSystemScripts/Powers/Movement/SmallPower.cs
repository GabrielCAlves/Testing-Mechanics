// SmallPower.cs
using SceneScript;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSmallPower", menuName = "Powers/Movement/Small Power")]
public class SmallPower : Power
{
    [Header("Configurações de Pequeno")]
    public float sizeMultiplier = 0.3f;
    public float speedMultiplier = 1.5f;
    public float jumpMultiplier = 2f;
    public float stealthMultiplier = 0.5f;
    public GameObject shrinkEffect;
    public GameObject glowEffect;

    private Vector3 originalScale;
    private float originalSpeed;
    private float originalJump;
    private GameObject glowObject;
    private bool isSmall = false;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        Shrink(user);
    }

    void Shrink(GameObject user)
    {
        isSmall = true;

        originalScale = user.transform.localScale;
        user.transform.localScale *= sizeMultiplier;

        // Aumenta velocidade e salto
        var movement = user.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            originalSpeed = movement.moveSpeed;
            originalJump = movement.jumpForce;

            movement.moveSpeed *= speedMultiplier;
            movement.jumpForce *= jumpMultiplier;
        }

        // Glow effect para visibilidade
        if (glowEffect != null)
        {
            glowObject = Instantiate(glowEffect, user.transform);
            glowObject.transform.localPosition = Vector3.zero;
        }

        if (shrinkEffect != null)
        {
            Instantiate(shrinkEffect, user.transform.position, Quaternion.identity);
        }
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        isSmall = false;

        user.transform.localScale = originalScale;

        var movement = user.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.moveSpeed = originalSpeed;
            movement.jumpForce = originalJump;
        }

        if (glowObject != null)
        {
            Destroy(glowObject);
        }
    }
}