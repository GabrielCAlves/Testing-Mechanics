// TimeManipulationPower.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTimeManipulationPower", menuName = "Powers/Utility/Time Manipulation Power")]
public class TimeManipulationPower : Power
{
    [Header("Configurações de Tempo")]
    public float timeScale = 0.1f;
    public float duration = 5f;
    public float slowRadius = 10f;
    public GameObject timeFieldEffect;
    public Color timeFieldColor = new Color(0, 1, 1, 0.3f);
    public AudioClip timeSound;

    private bool isActive = false;
    private float timer;
    private GameObject timeField;
    private float originalTimeScale;
    private List<Rigidbody> affectedRigidbodies = new List<Rigidbody>();

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        ActivateTimeSlow(user);
    }

    void ActivateTimeSlow(GameObject user)
    {
        isActive = true;
        timer = duration;

        originalTimeScale = Time.timeScale;
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // Cria campo visual
        if (timeFieldEffect != null)
        {
            timeField = Instantiate(timeFieldEffect, user.transform.position, Quaternion.identity);
            timeField.transform.localScale = Vector3.one * slowRadius;

            var renderer = timeField.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = timeFieldColor;
            }
        }

        // Afeta rigidbodies na área
        Collider[] colliders = Physics.OverlapSphere(user.transform.position, slowRadius);
        foreach (var col in colliders)
        {
            var rb = col.GetComponent<Rigidbody>();
            if (rb != null && rb.gameObject != user)
            {
                affectedRigidbodies.Add(rb);
                rb.linearVelocity *= timeScale;
            }
        }

        if (timeSound != null)
        {
            AudioSource.PlayClipAtPoint(timeSound, user.transform.position);
        }
    }

    public void UpdateTimeManipulation(GameObject user)
    {
        if (!isActive) return;

        timer -= Time.unscaledDeltaTime;

        // Mantém campo de tempo centralizado no usuário
        if (timeField != null)
        {
            timeField.transform.position = user.transform.position;
        }

        if (timer <= 0)
        {
            Deactivate(user);
        }
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        isActive = false;

        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = 0.02f;

        if (timeField != null)
        {
            Destroy(timeField);
        }

        // Restaura rigidbodies
        foreach (var rb in affectedRigidbodies)
        {
            if (rb != null)
            {
                rb.linearVelocity /= timeScale;
            }
        }
        affectedRigidbodies.Clear();
    }
}