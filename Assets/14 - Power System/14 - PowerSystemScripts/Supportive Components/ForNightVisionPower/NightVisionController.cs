using UnityEngine;
using UnityEngine.Rendering;

public class NightVisionController : MonoBehaviour
{
    [SerializeField] private Color defaultLightColor;
    [SerializeField] private Color boostedLightColor;
    [SerializeField] private DarkVisionPower darkVisionPower;
    [SerializeField] private Volume volume;

    private bool isNightVisionEnabled;

    private void OnEnable()
    {
        if (darkVisionPower != null)
        {
            darkVisionPower.OnDarkVisionActivated += ToggleNightVision;
        }
    }

    void Start()
    {
        RenderSettings.ambientLight = defaultLightColor;

        volume.weight = 0;
    }

    void ToggleNightVision()
    {
        if (volume == null)
            return;

        isNightVisionEnabled = !isNightVisionEnabled;

        Debug.Log($"Night Vision Enabled: {isNightVisionEnabled}");

        if (isNightVisionEnabled)
        {
            RenderSettings.ambientLight = boostedLightColor;
            volume.weight = 1;
        }
        else
        {
            RenderSettings.ambientLight = defaultLightColor;
            volume.weight = 0;
        }
    }

    private void OnDisable()
    {
        if (darkVisionPower != null)
        {
            darkVisionPower.OnDarkVisionActivated -= ToggleNightVision;
        }
    }
}
