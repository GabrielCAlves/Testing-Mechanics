// InvisibilityPower.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewInvisibilityPower", menuName = "Powers/Utility/Invisibility Power")]
public class InvisibilityPower : Power
{
    [Header("Configurações de Invisibilidade")]
    public float duration = 10f;
    public float fadeSpeed = 2f;
    public float revealDistance = 2f;
    [Range(0f, 1f)] public float levelOfAlphaVisibility = 0.2f;
    public GameObject invisibilityEffect;
    public bool silentMovement = true;

    [SerializeField] private bool isInvisible = false;
    [SerializeField] private float timer;
    [SerializeField] private Renderer[] renderers;
    private AudioSource audioSource;
    private float originalVolume;
    private GameObject effectObject;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        BecomeInvisible(user);
    }

    void BecomeInvisible(GameObject user)
    {
        isInvisible = true;
        timer = duration;

        renderers = user.GetComponentsInChildren<Renderer>();
        audioSource = user.GetComponent<AudioSource>();

        // Fade out dos renderers
        var mono = user.GetComponent<MonoBehaviour>();

        if (mono != null)
            mono.StartCoroutine(FadeRenderers(user, 1f, levelOfAlphaVisibility)); // The material needs to be set transparent for this to work, not opaque

        // Silencia movimentos
        if (silentMovement && audioSource != null)
        {
            originalVolume = audioSource.volume;
            audioSource.volume = 0f;
        }

        // Efeito visual
        if (invisibilityEffect != null)
        {
            effectObject = Instantiate(invisibilityEffect, user.transform);
            effectObject.transform.localPosition = Vector3.zero;
        }
    }

    System.Collections.IEnumerator FadeRenderers(GameObject user, float from, float to)
    {
        float progress = 0f;
        while (progress < 1f)
        {
            progress += Time.deltaTime * fadeSpeed;
            float alpha = Mathf.Lerp(from, to, progress);

            Debug.Log("alpha -> " + alpha);

            foreach (var rend in renderers)
            {
                Color color = rend.material.color;
                color.a = alpha;
                rend.material.color = color;
            }

            yield return null;
        }
    }

    public override void UpdatePower(GameObject user)
    {
        if (!isInvisible) return;

        timer -= Time.deltaTime;

        // Revela se estiver muito perto de inimigos
        Collider[] nearby = Physics.OverlapSphere(user.transform.position, revealDistance);
        foreach (var col in nearby)
        {
            if (col.CompareTag("Enemy"))
            {
                // Torna parcialmente visível
                foreach (var rend in renderers)
                {
                    Color color = rend.material.color;
                    color.a = 0.5f;
                    rend.material.color = color;
                }
                break;
            }
        }

        if (timer <= 0)
        {
            Deactivate(user);
        }
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        isInvisible = false;

        // Restaura visibilidade
        var mono = user.GetComponent<MonoBehaviour>();
        mono.StartCoroutine(FadeRenderers(user, 0f, 1f));

        // Restaura som
        if (silentMovement && audioSource != null)
        {
            audioSource.volume = originalVolume;
        }

        if (effectObject != null)
        {
            Destroy(effectObject);
        }
    }
}