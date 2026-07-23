// ElementalControlPower.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewElementalControlPower", menuName = "Powers/Utility/Elemental Control Power")]
public class ElementalControlPower : Power
{
    public enum Element { Fire, Water, Earth, Air, Lightning, Light, Dark }

    [Header("Configurações Elementais")]
    public Element currentElement = Element.Fire;
    public Element[] unlockedElements;

    [Header("Elementos Configurações")]
    public float elementSwitchCooldown = 1f;
    public GameObject[] elementEffects;
    public Material[] elementMaterials;
    public Color[] elementColors;
    public float[] elementDamageMultipliers;

    private int currentElementIndex = 0;
    private float switchTimer = 0f;
    private GameObject currentEffect;
    private bool isActive = false;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        isActive = true;
        SwitchElement(user, currentElement);
    }

    public void UpdateElementalControl(GameObject user)
    {
        if (!isActive) return;

        switchTimer -= Time.deltaTime;

        // Troca de elemento
        if (Input.GetKeyDown(KeyCode.Alpha1) && switchTimer <= 0)
        {
            SwitchElement(user, Element.Fire);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && switchTimer <= 0)
        {
            SwitchElement(user, Element.Water);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && switchTimer <= 0)
        {
            SwitchElement(user, Element.Earth);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && switchTimer <= 0)
        {
            SwitchElement(user, Element.Air);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) && switchTimer <= 0)
        {
            SwitchElement(user, Element.Lightning);
        }
    }

    void SwitchElement(GameObject user, Element element)
    {
        if (!System.Array.Exists(unlockedElements, e => e == element)) return;

        currentElement = element;
        currentElementIndex = System.Array.IndexOf(unlockedElements, element);
        switchTimer = elementSwitchCooldown;

        // Atualiza efeito visual
        if (currentEffect != null)
        {
            Destroy(currentEffect);
        }

        if (elementEffects != null && currentElementIndex < elementEffects.Length)
        {
            currentEffect = Instantiate(elementEffects[currentElementIndex], user.transform);
            currentEffect.transform.localPosition = Vector3.zero;

            var renderer = currentEffect.GetComponent<Renderer>();
            if (renderer != null && elementMaterials != null && currentElementIndex < elementMaterials.Length)
            {
                renderer.material = elementMaterials[currentElementIndex];
            }
        }

        // Aplica dano elemental
        var damageDealer = user.GetComponent<DamageDealer>();
        if (damageDealer != null && elementDamageMultipliers != null && currentElementIndex < elementDamageMultipliers.Length)
        {
            damageDealer.elementalMultiplier = elementDamageMultipliers[currentElementIndex];
            damageDealer.elementType = currentElement;
        }
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        isActive = false;

        if (currentEffect != null)
        {
            Destroy(currentEffect);
        }
    }
}