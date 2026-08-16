// ElementalControlPower.cs
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewElementalControlPower", menuName = "Powers/Utility/Elemental Control Power")]
public class ElementalControlPower : Power
{
    public enum Element { Fire, Water, Earth, Air, Lightning, Light, Dark }
    public List<ElementalAbilities> elementalAbilitiesList = new List<ElementalAbilities>();
    //public ElementalAbilities[] elementalAbilitiesList;

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
    private PowerUser powerUser;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        isActive = true;

        if(powerUser == null)
            powerUser = user.GetComponent<PowerUser>();

        SwitchElement(user, currentElement);
    }

    public override void UpdatePower(GameObject user)
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
        if (Input.GetKeyDown(KeyCode.Alpha6) && switchTimer <= 0)
        {
            SwitchElement(user, Element.Light);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7) && switchTimer <= 0)
        {
            SwitchElement(user, Element.Dark);
        }

        //GameObject g = Instantiate(currentEffect, user.transform.position, Quaternion.identity);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (elementalAbilitiesList != null && currentElementIndex < elementalAbilitiesList.Count)
            {
                elementalAbilitiesList[currentElementIndex].VerifyPowerUser(powerUser);
                elementalAbilitiesList[currentElementIndex].ShortAttack(user);
                powerUser.DeactivateAbilityCoroutine(elementalAbilitiesList[currentElementIndex].shortRangeAttack, elementalAbilitiesList[currentElementIndex].shortAttackTime);
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (elementalAbilitiesList != null && currentElementIndex < elementalAbilitiesList.Count)
            {
                elementalAbilitiesList[currentElementIndex].VerifyPowerUser(powerUser);
                elementalAbilitiesList[currentElementIndex].MidAttack(user);
                powerUser.DeactivateAbilityCoroutine(elementalAbilitiesList[currentElementIndex].midRangeAttack, elementalAbilitiesList[currentElementIndex].midAttackTime);
            }
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (elementalAbilitiesList != null && currentElementIndex < elementalAbilitiesList.Count)
            {
                elementalAbilitiesList[currentElementIndex].VerifyPowerUser(powerUser);
                elementalAbilitiesList[currentElementIndex].LongAttack(user);
                powerUser.DeactivateAbilityCoroutine(elementalAbilitiesList[currentElementIndex].longRangeAttack, elementalAbilitiesList[currentElementIndex].longAttackTime);
            }
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (elementalAbilitiesList != null && currentElementIndex < elementalAbilitiesList.Count)
            {
                elementalAbilitiesList[currentElementIndex].VerifyPowerUser(powerUser);
                elementalAbilitiesList[currentElementIndex].ShootAttack(user);
            }
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (elementalAbilitiesList != null && currentElementIndex < elementalAbilitiesList.Count)
            {
                elementalAbilitiesList[currentElementIndex].VerifyPowerUser(powerUser);
                elementalAbilitiesList[currentElementIndex].AreaAttack(user);
                powerUser.DeactivateAbilityCoroutine(elementalAbilitiesList[currentElementIndex].areaAttack, elementalAbilitiesList[currentElementIndex].areaAttackTime);
            }
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("Defensive Protection Activated");
            Debug.Log("elementalAbilitiesList != null -> " + (elementalAbilitiesList != null));
            Debug.Log("currentElementIndex < elementalAbilitiesList.Count -> " + (currentElementIndex < elementalAbilitiesList.Count));
            if (elementalAbilitiesList != null && currentElementIndex < elementalAbilitiesList.Count)
            {
                elementalAbilitiesList[currentElementIndex].VerifyPowerUser(powerUser);
                elementalAbilitiesList[currentElementIndex].DefensiveProtection(user);
                powerUser.DeactivateAbilityCoroutine(elementalAbilitiesList[currentElementIndex].defensiveProtection, elementalAbilitiesList[currentElementIndex].defensiveProtectionTime);
            }
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

        Debug.Log("Switched to element: " + currentElement.ToString());
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

//[Serializable]
[System.Serializable]
public class ElementalAbilities /* : MonoBehaviour*/ // O MonoBehaviour foi removido para permitir que esta classe seja serializável e usada como um campo dentro do ScriptableObject. Isso permite que configurar diferentes habilidades elementais diretamente no editor do Unity.
{
    [Header("Reference to PowerUser")]
    public PowerUser powerUser;

    [Header("Short Range Attack Configuration")]
    public GameObject shortRangeAttack;
    public Vector3 shortAttackOffset;
    public float shortAttackDamage;
    public float shortAttackTime;
    private GameObject shortAttackInstance;

    [Header("Mid Range Attack Configuration")]
    public GameObject midRangeAttack;
    public Vector3 midAttackOffset;
    public float midAttackDamage;
    public float midAttackTime;
    private GameObject midAttackInstance;

    [Header("Long Range Attack Configuration")]
    public GameObject longRangeAttack;
    public Vector3 longAttackOffset;
    public float longAttackDamage;
    public float longAttackTime;
    private GameObject longAttackInstance;

    #region ShootAttack
    [Header("Shooting Attack Configuration")]
    public GameObject bulletPrefab; // Needs to put the bullet script on the prefab
    public Transform shootPoint;
    public float shootDamage;
    public float shootForce;

    [Header("Pool Object")]
    public int poolSize = 10;
    public List<GameObject> bulletPool;
    public float timeToDeactivate = 2f;

    [Header("Reload Waiting Effect")]
    public bool useReloadTime = false;
    public float reloadTime = 2f;
    private bool initialized = false;
    private int shootCount = 0;
    private bool reloading = false;
    #endregion

    [Header("Area Attack Configuration")]
    public GameObject areaAttack;
    public Vector3 areaAttackOffset;
    public float areaAttackRadius;
    public LayerMask areaAttackLayerMask;
    public float areaAttackDamage;
    public float areaAttackTime;
    private GameObject areaAttackInstance;

    [Header("Defensive Protection Configuration")]
    public GameObject defensiveProtection;
    public Vector3 defensiveOffset;
    public float defensiveProtectionRadius;
    public float defensiveProtectionTime;
    private GameObject defensiveProtectionInstance;

    public void VerifyPowerUser(PowerUser powerUser)
    {
        Debug.Log("(Before) this.powerUser == null -> "+ (this.powerUser == null));
        if(this.powerUser == null)
        {
            this.powerUser = powerUser;
        }
        Debug.Log("(After) this.powerUser == null -> " + (this.powerUser == null));
    }

    public void ShortAttack(GameObject user)
    {
        if (shortRangeAttack != null)
        {
            //Instantiate(shortRangeAttack, shortAttackPosition.position, Quaternion.identity);
            if (shortAttackInstance == null)
            {
                shortAttackInstance = powerUser.InstantiateObject(shortRangeAttack, shortAttackOffset, Quaternion.identity);
                shortAttackInstance.transform.parent = user.transform; // Make the short attack a child of the user so it follows the user's position
            }

            if (shortAttackInstance.transform.position != user.transform.position)
                shortAttackInstance.transform.position = user.transform.position + shortAttackOffset;
            //shortRangeAttack.transform.position = shortAttackPosition;
            //shortRangeAttack.transform.rotation = Quaternion.identity;
            shortAttackInstance.SetActive(true); // OR shortAttackInstance.GetComponent<Animator>().SetTrigger("Activate");
            //StartCoroutine(DeactivateAbility(shortAttackInstance, shortAttackTime));
            powerUser.DeactivateAbilityCoroutine(shortAttackInstance, shortAttackTime);
        }
    }

    public void MidAttack(GameObject user)
    {
        if (midRangeAttack != null)
        {
            //Instantiate(midRangeAttack, midAttackPosition.position, Quaternion.identity);
            if (midAttackInstance == null)
            {
                midAttackInstance = powerUser.InstantiateObject(midRangeAttack, midAttackOffset, Quaternion.identity);
                midAttackInstance.transform.parent = user.transform; // Make the mid attack a child of the user so it follows the user's position
            }

            if (midAttackInstance.transform.position != user.transform.position)
                midAttackInstance.transform.position = user.transform.position + midAttackOffset;
            //midRangeAttack.transform.position = midAttackPosition;
            //midRangeAttack.transform.rotation = Quaternion.identity;
            midAttackInstance.SetActive(true); // OR midAttackInstance.GetComponent<Animator>().SetTrigger("Activate");
            //StartCoroutine(DeactivateAbility(midAttackInstance, midAttackTime));
            powerUser.DeactivateAbilityCoroutine(midAttackInstance, midAttackTime);
        }
    }

    public void LongAttack(GameObject user)
    {
        if (longRangeAttack != null)
        {
            //Instantiate(longRangeAttack, longAttackPosition.position, Quaternion.identity);
            if (longAttackInstance == null)
            {
                longAttackInstance = powerUser.InstantiateObject(longRangeAttack, longAttackOffset, Quaternion.identity);
                longAttackInstance.transform.parent = user.transform; // Make the long attack a child of the user so it follows the user's position
            }

            if (longAttackInstance.transform.position != user.transform.position)
                longAttackInstance.transform.position = user.transform.position + longAttackOffset;
            //longRangeAttack.transform.position = longAttackPosition;
            //longRangeAttack.transform.rotation = Quaternion.identity;
            longAttackInstance.SetActive(true); // OR longAttackInstance.GetComponent<Animator>().SetTrigger("Activate");
            //StartCoroutine(DeactivateAbility(longAttackInstance, longAttackTime));
            powerUser.DeactivateAbilityCoroutine(longAttackInstance, longAttackTime);
        }
    }

    public void ShootAttack(GameObject user)
    {
        if (useReloadTime && shootCount == bulletPool.Count && !reloading)
        {
            reloading = true;

            //StartCoroutine(Reload());
            powerUser.ReloadCoroutine(shootCount, reloadTime, reloading);
        }
        else if (useReloadTime && shootCount == bulletPool.Count && reloading)
        {
            Debug.Log("Reloading projectiles...");
        }
        else
        {
            if (!initialized)
            {
                for (int i = 0; i < poolSize; ++i)
                {
                    GameObject bullet = bulletPrefab /*Instantiate(bulletPrefab)*/;
                    bullet.SetActive(false);
                    bulletPool.Add(bullet);
                }

                initialized = true;
            }

            ShootPool();
        }
    }

    public void AreaAttack(GameObject user)
    {
        if (areaAttack != null)
        {
            //Instantiate(areaAttack, areaAttackPosition.position, Quaternion.identity);
            if (areaAttackInstance == null)
            {
                areaAttackInstance = powerUser.InstantiateObject(areaAttack, areaAttackOffset, Quaternion.identity);
                areaAttackInstance.transform.parent = user.transform; // Make the area attack a child of the user so it follows the user's position
            }

            if (areaAttackInstance.transform.position != user.transform.position)
                areaAttackInstance.transform.position = user.transform.position + areaAttackOffset;
            //areaAttack.transform.position = areaAttackPosition;
            //areaAttack.transform.rotation = Quaternion.identity;
            areaAttackInstance.SetActive(true); // OR areaAttackInstance.GetComponent<Animator>().SetTrigger("Activate");
            Collider[] hits = Physics.OverlapSphere(areaAttackInstance.transform.position, areaAttackRadius, areaAttackLayerMask);
            if (hits.Length > 0)
            {
                foreach (Collider hit in hits)
                {
                    if (hit.gameObject != user && hit.GetComponent<Health>())
                    {
                        // Apply damage or effects to the hit object
                        Debug.Log($"Hit object: {hit.gameObject.name}");

                        Health enemyHealth = hit.GetComponent<Health>();
                        enemyHealth.TakeDamage(areaAttackDamage);
                    }
                }
            }
            //StartCoroutine(DeactivateAbility(areaAttack, areaAttackTime));
            powerUser.DeactivateAbilityCoroutine(areaAttackInstance, areaAttackTime);
        }
    }

    public void DefensiveProtection(GameObject user)
    {
        Debug.Log("defensiveProtection != null -> "+ (defensiveProtection != null));
        if (defensiveProtection != null)
        {
            if(defensiveProtectionInstance == null)
            {
                defensiveProtectionInstance = powerUser.InstantiateObject(defensiveProtection, defensiveOffset, Quaternion.identity);
                defensiveProtectionInstance.transform.parent = user.transform; // Make the defensive protection a child of the user so it follows the user's position
            }

            if(defensiveProtectionInstance.transform.position != user.transform.position)
                defensiveProtectionInstance.transform.position = user.transform.position + defensiveOffset;

            //defensiveProtection.transform.rotation = Quaternion.identity;
            defensiveProtectionInstance.SetActive(true); // OR defensiveProtectionInstance.GetComponent<Animator>().SetTrigger("Activate");
            //StartCoroutine(DeactivateAbility(defensiveProtection, defensiveProtectionTime));
            //Instantiate(defensiveProtection, defensivePosition.position, Quaternion.identity);
            powerUser.DeactivateAbilityCoroutine(defensiveProtectionInstance, defensiveProtectionTime);
        }
    }

    private void ShootPool()
    {
        foreach (GameObject bullet in bulletPool)
        {
            if (!bullet.activeInHierarchy)
            {
                bullet.transform.position = shootPoint.transform.position;
                bullet.transform.rotation = shootPoint.transform.rotation;
                bullet.SetActive(true); // OR bullet.GetComponent<Animator>().SetTrigger("Activate");
                ApplyForce(bullet);

                if (useReloadTime)
                    ++shootCount;

                return;
            }
        }
    }

    private void ApplyForce(GameObject bullet)
    {
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        Vector3 shootDirection = shootPoint.transform.forward;

        // AddForce with Impulse mode can cause issues with the bullet's velocity when reusing it from the pool, as it may not reset properly. Instead, we can directly set the velocity of the bullet to ensure consistent behavior.
        //bulletRb.AddForce(shootDirection * shootForce, ForceMode.Impulse);
        bulletRb.linearVelocity = shootDirection * shootForce;

        //StartCoroutine(DeactivateBullet(bullet));
        powerUser.DeactivateBulletCoroutine(bullet, timeToDeactivate, shootPoint);
    }

    //IEnumerator DeactivateBullet(GameObject bullet)
    //{
    //    yield return new WaitForSeconds(timeToDeactivate);

    //    bullet.SetActive(false);
    //    bullet.transform.position = shootPoint.transform.position;
    //}

    //IEnumerator Reload()
    //{
    //    yield return new WaitForSeconds(reloadTime);

    //    shootCount = 0;
    //    reloading = false;
    //}

    //IEnumerator DeactivateAbility(GameObject ability, float deactivateTime)
    //{
    //    yield return new WaitForSeconds(deactivateTime);
    //    ability.SetActive(false);
    //}
}