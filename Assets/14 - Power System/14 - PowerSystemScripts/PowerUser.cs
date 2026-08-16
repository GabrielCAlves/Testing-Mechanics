using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUser : MonoBehaviour
{
    [Header("Lista de Poderes")]
    public List<Power> powers = new List<Power>();

    [Header("Configurações do Usuário")]
    public float maxEnergy = 100f;
    public float currentEnergy;
    public float energyRegenRate = 10f;

    private Dictionary<Power, float> cooldownTimers = new Dictionary<Power, float>();
    private Dictionary<Power, bool> activePowers = new Dictionary<Power, bool>();

    [Header("Configurações Elementais (para ElementalControl)")]
    public float elementalMultiplier = 1f;
    public ElementalControlPower.Element currentElementType = ElementalControlPower.Element.Fire;

    [SerializeField] private float rangeArea;

    void Start()
    {
        currentEnergy = maxEnergy;
        foreach (var power in powers)
        {
            cooldownTimers[power] = 0f;
            activePowers[power] = false;
        }
    }

    void Update()
    {
        UpdateCooldowns();
        RegenerateEnergy();
        HandlePowerInput();

        // --- CHAMA O Update DE TODOS OS PODERES ATIVOS ---
        foreach (var power in powers)
        {
            if (activePowers.ContainsKey(power) && activePowers[power])
            {
                power.UpdatePower(gameObject);  // <-- SIMPLES ASSIM!
            }
        }
    }

    void HandlePowerInput()
    {
        foreach (var power in powers)
        {
            if (power != null && Input.GetKeyDown(power.activationKey))
                TryActivatePower(power);
            if (power != null && Input.GetKeyUp(power.activationKey) && activePowers[power])
                DeactivatePower(power);
        }
    }

    public bool TryActivatePower(Power power)
    {
        if (!powers.Contains(power)) return false;
        if (cooldownTimers[power] > 0) return false;
        if (currentEnergy < power.energyCost) return false;
        if (!power.CanActivate(gameObject)) return false;
        if (activePowers[power]) return false;

        currentEnergy -= power.energyCost;
        power.Activate(gameObject);
        activePowers[power] = true;
        cooldownTimers[power] = power.cooldown;
        return true;
    }

    public void DeactivatePower(Power power)
    {
        if (activePowers[power])
        {
            power.Deactivate(gameObject);
            activePowers[power] = false;
        }
    }

    void UpdateCooldowns()
    {
        List<Power> keys = new List<Power>(cooldownTimers.Keys);
        foreach (var power in keys)
        {
            if (cooldownTimers[power] > 0)
            {
                cooldownTimers[power] -= Time.deltaTime;
                if (cooldownTimers[power] < 0) cooldownTimers[power] = 0;
            }
        }
    }

    void RegenerateEnergy()
    {
        if (currentEnergy < maxEnergy)
        {
            currentEnergy += energyRegenRate * Time.deltaTime;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
        }
    }

    public void AddPower(Power power)
    {
        if (!powers.Contains(power))
        {
            powers.Add(power);
            cooldownTimers[power] = 0f;
            activePowers[power] = false;
        }
    }

    public void RemovePower(Power power)
    {
        if (powers.Contains(power))
        {
            DeactivatePower(power);
            powers.Remove(power);
            cooldownTimers.Remove(power);
            activePowers.Remove(power);
        }
    }

    public float GetCooldownRemaining(Power power)
    {
        if (cooldownTimers.ContainsKey(power))
            return cooldownTimers[power];
        return 0f;
    }

    public GameObject InstantiateObject(GameObject prefab, Vector3 offset, Quaternion rotation)
    {
        return Instantiate(prefab, transform.position+offset, rotation);
    }

    public void DeactivateBulletCoroutine(GameObject bullet, float deactivateTime, Transform shootPoint)
    {
        StartCoroutine(DeactivateBullet(bullet, deactivateTime, shootPoint));
    }

    public void ReloadCoroutine(float shootCount, float reloadTime, bool reloading)
    {
        StartCoroutine(Reload(shootCount, reloadTime, reloading));
    }

    public void DeactivateAbilityCoroutine(GameObject ability, float deactivateTime)
    {
        StartCoroutine(DeactivateAbility(ability, deactivateTime));
    }

    IEnumerator DeactivateBullet(GameObject bullet, float timeToDeactivate, Transform shootPoint)
    {
        yield return new WaitForSeconds(timeToDeactivate);

        bullet.SetActive(false);
        bullet.transform.position = shootPoint.position;
    }

    IEnumerator Reload(float shootCount, float reloadTime, bool reloading)
    {
        yield return new WaitForSeconds(reloadTime);

        shootCount = 0;
        reloading = false;
    }

    IEnumerator DeactivateAbility(GameObject ability, float deactivateTime)
    {
        yield return new WaitForSeconds(deactivateTime);
        ability.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangeArea);
    }
}