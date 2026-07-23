using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PowerUI : MonoBehaviour
{
    public PowerUser powerUser;
    public GameObject powerSlotPrefab;
    public Transform powerSlotsParent;

    private List<Image> cooldownImages = new List<Image>();
    private List<Image> energyImages = new List<Image>();

    void Start()
    {
        if (powerUser == null)
            powerUser = GetComponent<PowerUser>();
        SetupPowerUI();
    }

    void SetupPowerUI()
    {
        // Limpa slots antigos
        foreach (Transform child in powerSlotsParent)
            Destroy(child.gameObject);

        cooldownImages.Clear();
        energyImages.Clear();

        foreach (var power in powerUser.powers)
        {
            GameObject slot = Instantiate(powerSlotPrefab, powerSlotsParent);
            // Ícone
            Image icon = slot.GetComponent<Image>();
            if (icon != null && power.icon != null)
                icon.sprite = power.icon;

            // Overlay de cooldown (filho chamado "CooldownOverlay")
            Transform cdTrans = slot.transform.Find("CooldownOverlay");
            Image cdImg = cdTrans != null ? cdTrans.GetComponent<Image>() : null;
            cooldownImages.Add(cdImg);

            // Overlay de energia (filho chamado "EnergyOverlay")
            Transform enTrans = slot.transform.Find("EnergyOverlay");
            Image enImg = enTrans != null ? enTrans.GetComponent<Image>() : null;
            energyImages.Add(enImg);
        }
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        for (int i = 0; i < powerUser.powers.Count && i < cooldownImages.Count; i++)
        {
            Power power = powerUser.powers[i];

            // Cooldown
            float remaining = powerUser.GetCooldownRemaining(power);
            if (cooldownImages[i] != null)
            {
                // Evita divisão por zero
                if (power.cooldown > 0)
                    cooldownImages[i].fillAmount = remaining / power.cooldown;
                else
                    cooldownImages[i].fillAmount = 0f;
                // Mostra apenas se estiver em cooldown
                cooldownImages[i].gameObject.SetActive(remaining > 0);
            }

            // Energia (custo relativo)
            if (energyImages[i] != null)
            {
                float percent = 1f - (power.energyCost / powerUser.maxEnergy);
                energyImages[i].fillAmount = Mathf.Clamp01(percent);
                // Cor vermelha se não tiver energia suficiente
                energyImages[i].color = (powerUser.currentEnergy < power.energyCost) ? Color.red : Color.white;
            }
        }
    }
}