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
    public bool linearOrNot = true;

    [Header("Configurações de Brilho")]
    public float glowIntensity = 5f;

    private LineRenderer currentLaser;
    private float timer;
    private bool isActive;
    private Color lastAppliedColor;

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

            SetupLaserMaterial();

            currentLaser.startWidth = laserWidth;
            currentLaser.endWidth = laserWidth;
            currentLaser.positionCount = 2;

            ApplyColorSettings();
            lastAppliedColor = laserColor;
        }
        else
        {
            if (lastAppliedColor != laserColor)
            {
                ApplyColorSettings();
                lastAppliedColor = laserColor;
            }
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

    void SetupLaserMaterial()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");

        if (laserMaterial == null)
        {
            laserMaterial = new Material(shader);
        }
        else
        {
            laserMaterial = new Material(laserMaterial);
        }

        // Configurar superfície como opaca
        if (laserMaterial.HasProperty("_Surface"))
            laserMaterial.SetFloat("_Surface", 0);
        if (laserMaterial.HasProperty("_Blend"))
            laserMaterial.SetFloat("_Blend", 0);

        // Ativar emissão
        laserMaterial.EnableKeyword("_EMISSION");
        laserMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

        // Base Map = laserColor
        if (laserMaterial.HasProperty("_BaseColor"))
        {
            laserMaterial.SetColor("_BaseColor", laserColor);
        }

        // Emission Map = EXACTLY the same color as Base Map (without HDR conversion)
        if (laserMaterial.HasProperty("_EmissionColor"))
        {
            // Use Color.Linear to avoid HDR conversion
            Color emissionColor = new Color(
                laserColor.r,
                laserColor.g,
                laserColor.b,
                1f
            );

            // Alternative: Use gamma space to match
            // emissionColor = laserColor.gamma;

            laserMaterial.SetColor("_EmissionColor", ConvertToHDR(emissionColor, glowIntensity));
        }

        // Emission Intensity = 5
        if (laserMaterial.HasProperty("_EmissionIntensity"))
        {
            laserMaterial.SetFloat("_EmissionIntensity", glowIntensity);
        }

        currentLaser.material = laserMaterial;
    }

    //private Color ConvertToHDR(Color rgbColor, float intensity)
    //{
    //    intensity = Mathf.Max(0f, intensity);

    //    Color hdr = new Color(
    //        rgbColor.r * intensity,
    //        rgbColor.g * intensity,
    //        rgbColor.b * intensity,
    //        rgbColor.a
    //    );

    //    Debug.Log(
    //        $"RGB: ({rgbColor.r:F3}, {rgbColor.g:F3}, {rgbColor.b:F3}) " +
    //        $"Intensity: {intensity:F2} -> " +
    //        $"HDR: ({hdr.r:F3}, {hdr.g:F3}, {hdr.b:F3})"
    //    );

    //    return hdr;
    //}
    private Color ConvertToHDR(Color rgbColor, float exposure)
    {
        float multiplier = Mathf.Pow(2f, exposure);
        return rgbColor * multiplier;
    }


    void ApplyColorSettings()
    {
        if (currentLaser == null) return;

        // Aplicar cores ao LineRenderer
        currentLaser.startColor = laserColor;
        currentLaser.endColor = laserColor;

        // Aplicar cores ao material
        if (currentLaser.material != null)
        {
            // Base Color = laserColor
            if (currentLaser.material.HasProperty("_BaseColor"))
            {
                currentLaser.material.SetColor("_BaseColor", laserColor);
            }

            // Emission Color = EXACT match using Linear conversion
            if (currentLaser.material.HasProperty("_EmissionColor"))
            {
                // Convert to linear space to match Base Map
                Debug.Log($"laserColor.linear.r = {laserColor.linear.r}, laserColor.linear.g = {laserColor.linear.g}, laserColor.linear.b = {laserColor.linear.b}");
                Debug.Log($"laserColor.r = {laserColor.r}, laserColor.g = {laserColor.g}, laserColor.b = {laserColor.b}");
                Debug.Log($"glowIntensity = {glowIntensity}");

                Color emissionColor;
                if (linearOrNot)
                    emissionColor = laserColor.linear * (glowIntensity != 0 ? glowIntensity : 1f);
                else
                    emissionColor = laserColor * (glowIntensity != 0 ? glowIntensity : 1f);

                emissionColor.a = 1f;
                Debug.Log($"emissionColor.r = {emissionColor.r}, emissionColor.g = {emissionColor.g}, emissionColor.b = {emissionColor.b}");

                // Or try gamma if linear doesn't work
                // Color emissionColor = laserColor.gamma;

                currentLaser.material.SetColor("_EmissionColor", emissionColor);
            }

            // Emission Intensity = 5
            if (currentLaser.material.HasProperty("_EmissionIntensity"))
            {
                currentLaser.material.SetFloat("_EmissionIntensity", glowIntensity);
            }
        }
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

    public void SetLaserColor(Color newColor)
    {
        laserColor = newColor;
        if (currentLaser != null && currentLaser.enabled)
        {
            ApplyColorSettings();
            lastAppliedColor = newColor;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (currentLaser != null && currentLaser.enabled)
        {
            ApplyColorSettings();
            lastAppliedColor = laserColor;
        }
    }
#endif
}