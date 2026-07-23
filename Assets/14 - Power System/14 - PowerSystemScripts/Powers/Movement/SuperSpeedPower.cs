// SuperSpeedPower.cs
using SceneScript;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "NewSuperSpeedPower", menuName = "Powers/Movement/Super Speed Power")]
public class SuperSpeedPower : Power
{
    [Header("Configurações da Super Velocidade")]
    public float speedMultiplier = 5f;
    public float accelerationMultiplier = 3f;
    public float blurEffectIntensity = 1f;
    public GameObject speedTrailPrefab;
    public float trailLifetime = 0.5f;

    private float originalSpeed;
    private float originalAcceleration;
    private GameObject speedTrail;
    private bool isActive = false;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        ActivateSpeed(user);
    }

    void ActivateSpeed(GameObject user)
    {
        isActive = true;

        var movement = user.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            originalSpeed = movement.moveSpeed;
            originalAcceleration = movement.acceleration;

            movement.moveSpeed *= speedMultiplier;
            movement.acceleration *= accelerationMultiplier;
        }

        // Cria rastro de velocidade
        if (speedTrailPrefab != null)
        {
            speedTrail = Instantiate(speedTrailPrefab, user.transform);
            speedTrail.transform.localPosition = Vector3.zero;

            var trail = speedTrail.GetComponent<TrailRenderer>();
            if (trail != null)
            {
                trail.time = trailLifetime;
            }
        }

        // Efeito de blur (usando câmera)
        var cameraEffect = Camera.main.GetComponent<MotionBlur>();
        if (cameraEffect != null)
        {
            cameraEffect.blurAmount = blurEffectIntensity;
        }
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        isActive = false;

        var movement = user.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.moveSpeed = originalSpeed;
            movement.acceleration = originalAcceleration;
        }

        if (speedTrail != null)
        {
            Destroy(speedTrail);
        }

        var cameraEffect = Camera.main.GetComponent<MotionBlur>();
        if (cameraEffect != null)
        {
            cameraEffect.blurAmount = 0;
        }
    }
}