using System.Collections;
using UnityEngine;

public class Shine : MonoBehaviour
{
    [Header("Shine Settings")]
    [SerializeField] private Material material;
    [SerializeField] private Color shineColor;

    [Header("Shine Effect Settings")]
    [SerializeField] private float startIntensity = 0f;
    [SerializeField] private float shineIntensity = 7f;
    [SerializeField] private float shineDuration = 5f;
    [SerializeField] private float chargeSpeed = .05f;

    //[SerializeField] private Color actualIntensity;

    [Header("Debug")]
    [SerializeField] private float elapsedTime = 0f;
    [SerializeField] private float intensity = 0f;
    [SerializeField] private float t = 0f;
    [SerializeField] private float smoothT = 0f;
    [SerializeField] private bool alreadyShining = false;

    void Start()
    {
        if(material == null)
        {
            material = GetComponent<Renderer>().material;
            material.EnableKeyword("_EMISSION");
        }
    }

    private void OnMouseDown()
    {
        if(material == null)
        {
            Debug.Log("Material not found on " + gameObject.name);
            return;
        }
        
        //material.SetColor("_EmissionColor", shineColor * shineIntensity);
        StartCoroutine(ShineEffect(startIntensity, shineIntensity));
    }

    private void OnMouseUp()
    {
        if (material == null)
        {
            Debug.Log("Material not found on " + gameObject.name);
            return;
        }

        StopAllCoroutines();

        StartCoroutine(ShineEffect(/*shineIntensity*/ intensity, startIntensity));
    }

    public void StartShine()
    {
        if (material == null)
        {
            Debug.Log("Material not found on " + gameObject.name);
            return;
        }
        StartCoroutine(ShineEffect(startIntensity, shineIntensity));
    }

    public void StopShine()
    {
        if (material == null)
        {
            Debug.Log("Material not found on " + gameObject.name);
            return;
        }
        StopAllCoroutines();
        alreadyShining = false; //
        StartCoroutine(ShineEffect(intensity, startIntensity));
    }

    IEnumerator ShineEffect(float start, float end)
    {
        if(alreadyShining) //
        {
            yield break; // Exit if already shining
        }

        alreadyShining = true; //
        elapsedTime = 0f;
        while (elapsedTime < shineDuration)
        {
            t = elapsedTime / shineDuration; // Normalizes the time (0 to 1)
            smoothT = Mathf.SmoothStep(0, 1, t); // Applies easing
            intensity = Mathf.Lerp(start, end, smoothT);
            material.SetColor("_EmissionColor", shineColor * intensity);
            //actualIntensity = shineColor * intensity;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        material.SetColor("_EmissionColor", shineColor * end);

        alreadyShining = false; //
    }
}
