// Power.cs
using UnityEngine;

public abstract class Power : ScriptableObject
{
    [Header("Configurações Gerais")]
    public string powerName;
    public Sprite icon;
    [TextArea] public string description;
    public float cooldown;
    public float energyCost;
    public KeyCode activationKey = KeyCode.None;

    [Header("Feedback Visual")]
    public GameObject activationEffect;
    public AudioClip activationSound;

    public virtual void Activate(GameObject user)
    {
        // Lógica base que será sobrescrita
        Debug.Log($"Ativando poder: {powerName}");
    }

    public virtual void Deactivate(GameObject user)
    {
        // Para poderes com ativação contínua
        Debug.Log($"Desativando poder: {powerName}");
    }

    public virtual bool CanActivate(GameObject user)
    {
        return true; // Sobrescreva para condições específicas
    }

    public virtual void UpdatePower(GameObject user)
    {
        // Poderes que precisam de update sobrescrevem este método
    }
}