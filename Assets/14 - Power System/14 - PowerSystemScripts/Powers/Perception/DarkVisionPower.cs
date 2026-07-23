// DarkVisionPower.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewDarkVisionPower", menuName = "Powers/Perception/Dark Vision Power")]
public class DarkVisionPower : Power
{
    [Header("Configurações de Visão Noturna")]
    public Color nightVisionColor = new Color(0, 1, 0, 0.5f);
    public float nightVisionIntensity = 2f;
    public float visionRange = 20f;
    public GameObject nightVisionOverlay;
    public bool enableShaderEffect = true;
    public AudioClip nightVisionSound;

    private bool isActive = false;
    private GameObject overlayObject;
    private Light playerLight;
    private float originalLightIntensity;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        ActivateDarkVision(user);
    }

    void ActivateDarkVision(GameObject user)
    {
        isActive = true;

        // Overlay de visão noturna
        if (nightVisionOverlay != null && enableShaderEffect)
        {
            overlayObject = Instantiate(nightVisionOverlay);
            overlayObject.transform.SetParent(Camera.main.transform);
            overlayObject.transform.localPosition = Vector3.zero;
            overlayObject.transform.localScale = Vector3.one * 10f;

            var renderer = overlayObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = nightVisionColor;
            }
        }

        // Aumenta iluminação ambiente
        playerLight = user.GetComponentInChildren<Light>();
        if (playerLight != null)
        {
            originalLightIntensity = playerLight.intensity;
            playerLight.intensity = nightVisionIntensity;
            playerLight.range = visionRange;
        }

        if (nightVisionSound != null)
        {
            AudioSource.PlayClipAtPoint(nightVisionSound, user.transform.position);
        }
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        isActive = false;

        if (overlayObject != null)
        {
            Destroy(overlayObject);
        }

        if (playerLight != null)
        {
            playerLight.intensity = originalLightIntensity;
        }
    }
}