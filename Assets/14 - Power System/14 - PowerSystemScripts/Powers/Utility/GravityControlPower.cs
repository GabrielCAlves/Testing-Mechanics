using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "NewGravityControlPower", menuName = "Powers/Utility/Gravity Control Power")]
public class GravityControlPower : Power
{
    [Header("Configurações de Gravidade")]
    public float gravityMultiplier = 0.2f;
    public float antiGravityRange = 8f;
    public float pullForce = 20f;
    public float pushForce = 30f;
    public GameObject gravityFieldEffect;
    public bool affectEnemies = true;
    public bool affectProjectiles = true;
    public LayerMask enemyLayer;
    public bool withoutGravity = false;

    [Header("Modo Anti-Gravidade")]
    public AntiGravityMode antiGravityMode = AntiGravityMode.ContinuousLevitation;
    public float suspendDuration = 3f;
    public float suspendHeight = 2f;
    public float pushBackForce = 15f;
    public float levitationSpeed = 2f;

    public enum AntiGravityMode
    {
        ContinuousLevitation,
        TemporarySuspension,
        LevitationWithPush
    }

    private bool isActive = false;
    private GameObject gravityField;
    [SerializeField] private List<GameObject> affectedObjects = new List<GameObject>();
    private enum GravityMode { Normal, Anti, Push, Pull }
    private GravityMode currentMode = GravityMode.Normal;

    private Dictionary<GameObject, bool> originalNavMeshState = new Dictionary<GameObject, bool>();
    private Dictionary<GameObject, bool> isBeingControlled = new Dictionary<GameObject, bool>();
    private Dictionary<GameObject, float> suspendTimers = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, float> originalHeights = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, Vector3> suspendStartPositions = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, bool> hasBeenLifted = new Dictionary<GameObject, bool>();
    private Dictionary<GameObject, float> rangeCheckTimers = new Dictionary<GameObject, float>();

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        currentMode = GravityMode.Anti;
        ActivateAntiGravity(user);
    }

    void ActivateAntiGravity(GameObject user)
    {
        isActive = true;
        ClearDictionaries();

        // Cria campo de gravidade
        if (gravityFieldEffect != null)
        {
            gravityField = Instantiate(gravityFieldEffect, user.transform.position, Quaternion.identity);
            gravityField.transform.localScale = Vector3.one * antiGravityRange;
        }

        // Afeta objetos na área
        Collider[] colliders = Physics.OverlapSphere(user.transform.position, antiGravityRange, enemyLayer);
        foreach (var col in colliders)
        {
            if (col.gameObject == user) continue;

            if (affectEnemies && col.CompareTag("Enemy"))
            {
                AddObjectToControl(col.gameObject, user);
            }

            if (affectProjectiles && col.CompareTag("Projectile"))
            {
                var rb = col.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = false;
                    isBeingControlled[col.gameObject] = true;
                    if (!affectedObjects.Contains(col.gameObject))
                        affectedObjects.Add(col.gameObject);
                }
            }
        }

        // Aplica o modo atual UMA VEZ
        ApplyCurrentMode(user);
    }

    void AddObjectToControl(GameObject obj, GameObject user)
    {
        var rb = obj.GetComponent<Rigidbody>();
        if (rb == null) return;

        // Salva o estado original do NavMeshAgent
        var agent = obj.GetComponent<NavMeshAgent>();
        if (agent != null && agent.isActiveAndEnabled)
        {
            originalNavMeshState[obj] = agent.enabled;
            agent.enabled = false;
        }

        isBeingControlled[obj] = true;
        originalHeights[obj] = obj.transform.position.y;
        suspendTimers[obj] = suspendDuration;
        suspendStartPositions[obj] = obj.transform.position;
        hasBeenLifted[obj] = false;
        rangeCheckTimers[obj] = 0.5f; // Verifica a cada 0.5 segundos

        if (withoutGravity)
            rb.useGravity = false;

        if (!affectedObjects.Contains(obj))
            affectedObjects.Add(obj);

        Debug.Log($"Objeto adicionado ao controle: {obj.name}");
    }

    void ClearDictionaries()
    {
        originalNavMeshState.Clear();
        isBeingControlled.Clear();
        suspendTimers.Clear();
        originalHeights.Clear();
        suspendStartPositions.Clear();
        hasBeenLifted.Clear();
        rangeCheckTimers.Clear();
    }

    void ApplyCurrentMode(GameObject user)
    {
        switch (currentMode)
        {
            case GravityMode.Anti:
                ApplyAntiGravity(user);
                break;
            case GravityMode.Push:
                ApplyPush(user);
                break;
            case GravityMode.Pull:
                ApplyPull(user);
                break;
            case GravityMode.Normal:
                ApplyNormal(user);
                break;
        }
    }

    void ApplyAntiGravity(GameObject user)
    {
        foreach (var obj in affectedObjects)
        {
            if (obj == null) continue;

            var rb = obj.GetComponent<Rigidbody>();
            if (rb == null) continue;

            bool isControlled = isBeingControlled.ContainsKey(obj) && isBeingControlled[obj];
            if (!isControlled) continue;

            switch (antiGravityMode)
            {
                case AntiGravityMode.ContinuousLevitation:
                    rb.AddForce(-Physics.gravity * gravityMultiplier, ForceMode.Impulse);
                    break;

                case AntiGravityMode.TemporarySuspension:
                    suspendTimers[obj] = suspendDuration;
                    hasBeenLifted[obj] = false;
                    suspendStartPositions[obj] = obj.transform.position;
                    break;

                case AntiGravityMode.LevitationWithPush:
                    Vector3 pushDirection = (obj.transform.position - user.transform.position).normalized;
                    pushDirection.y = 1f;

                    rb.AddForce(-Physics.gravity * gravityMultiplier * 1.5f, ForceMode.Impulse);
                    rb.AddForce(pushDirection.normalized * pushBackForce, ForceMode.Impulse);
                    rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
                    break;
            }
        }
    }

    void ApplyPush(GameObject user)
    {
        foreach (var obj in affectedObjects)
        {
            if (obj == null) continue;

            var rb = obj.GetComponent<Rigidbody>();
            if (rb == null) continue;

            bool isControlled = isBeingControlled.ContainsKey(obj) && isBeingControlled[obj];
            if (!isControlled) continue;

            Vector3 direction = obj.transform.position - user.transform.position;
            rb.AddForce(direction.normalized * pushForce, ForceMode.Impulse);
        }
    }

    void ApplyPull(GameObject user)
    {
        foreach (var obj in affectedObjects)
        {
            if (obj == null) continue;

            var rb = obj.GetComponent<Rigidbody>();
            if (rb == null) continue;

            bool isControlled = isBeingControlled.ContainsKey(obj) && isBeingControlled[obj];
            if (!isControlled) continue;

            Vector3 direction = obj.transform.position - user.transform.position;
            rb.AddForce(-direction.normalized * pullForce, ForceMode.Impulse);
        }
    }

    void ApplyNormal(GameObject user)
    {
        foreach (var obj in affectedObjects)
        {
            if (obj == null) continue;

            var rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                rb.linearVelocity = Vector3.zero;
            }

            var agent = obj.GetComponent<NavMeshAgent>();
            if (agent != null && !agent.enabled)
            {
                RestoreAgent(obj);
            }

            isBeingControlled[obj] = false;
            suspendTimers.Remove(obj);
            originalHeights.Remove(obj);
            suspendStartPositions.Remove(obj);
            hasBeenLifted.Remove(obj);
            rangeCheckTimers.Remove(obj);
        }
    }

    public override void UpdatePower(GameObject user)
    {
        if (!isActive) return;

        // Mantém campo centralizado
        if (gravityField != null)
        {
            gravityField.transform.position = user.transform.position;
        }

        // --- VERIFICAÇÃO CONTÍNUA DE ALCANCE ---
        CheckRangeForAllObjects(user);

        // Verifica objetos que entram no alcance
        CheckNewObjectsInRange(user);

        // Atualização específica para Temporary Suspension
        if (currentMode == GravityMode.Anti && antiGravityMode == AntiGravityMode.TemporarySuspension)
        {
            UpdateTemporarySuspension(user);
        }

        // Modos de gravidade
        if (Input.GetKeyDown(KeyCode.G))
        {
            SwitchGravityMode(user);
        }
    }

    // --- NOVO MÉTODO: Verifica alcance de todos os objetos ---
    void CheckRangeForAllObjects(GameObject user)
    {
        List<GameObject> toRemove = new List<GameObject>();

        foreach (var obj in affectedObjects)
        {
            if (obj == null)
            {
                toRemove.Add(obj);
                continue;
            }

            // Atualiza o timer de verificação
            if (!rangeCheckTimers.ContainsKey(obj))
                rangeCheckTimers[obj] = 0.5f;

            rangeCheckTimers[obj] -= Time.deltaTime;
            if (rangeCheckTimers[obj] > 0) continue;

            rangeCheckTimers[obj] = 0.5f; // Reseta o timer

            // Verifica se o objeto ainda está no alcance
            if (!IsObjectInRange(obj, user))
            {
                // Objeto saiu do alcance - RESTAURA
                Debug.Log($"Objeto SAIU do alcance: {obj.name} - Restaurando...");
                RestoreObjectCompletely(obj);
                toRemove.Add(obj);
            }
        }

        // Remove objetos que saíram do alcance
        foreach (var obj in toRemove)
        {
            if (obj != null && affectedObjects.Contains(obj))
            {
                affectedObjects.Remove(obj);
            }
        }
    }

    // --- NOVO MÉTODO: Verifica novos objetos que entram no alcance ---
    void CheckNewObjectsInRange(GameObject user)
    {
        Collider[] newColliders = Physics.OverlapSphere(user.transform.position, antiGravityRange, enemyLayer);

        foreach (var col in newColliders)
        {
            if (col.gameObject == user) continue;

            if (!affectedObjects.Contains(col.gameObject))
            {
                if (affectEnemies && col.CompareTag("Enemy"))
                {
                    Debug.Log($"Novo objeto ENTROU no alcance: {col.gameObject.name}");
                    AddObjectToControl(col.gameObject, user);

                    // Aplica o modo atual no novo objeto
                    ApplyCurrentModeToObject(col.gameObject, user);
                }
            }
        }
    }

    // --- NOVO MÉTODO: Aplica o modo atual a um único objeto ---
    void ApplyCurrentModeToObject(GameObject obj, GameObject user)
    {
        switch (currentMode)
        {
            case GravityMode.Anti:
                var rb = obj.GetComponent<Rigidbody>();
                if (rb != null && antiGravityMode == AntiGravityMode.LevitationWithPush)
                {
                    Vector3 pushDirection = (obj.transform.position - user.transform.position).normalized;
                    pushDirection.y = 1f;
                    rb.AddForce(-Physics.gravity * gravityMultiplier * 1.5f, ForceMode.Impulse);
                    rb.AddForce(pushDirection.normalized * pushBackForce, ForceMode.Impulse);
                }
                break;
            case GravityMode.Push:
                ApplyPush(user);
                break;
            case GravityMode.Pull:
                ApplyPull(user);
                break;
        }
    }

    // --- NOVO MÉTODO: Restaura completamente um objeto ---
    void RestoreObjectCompletely(GameObject obj)
    {
        var rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
        }

        var agent = obj.GetComponent<NavMeshAgent>();
        if (agent != null && !agent.enabled)
        {
            RestoreAgent(obj);
        }

        isBeingControlled[obj] = false;
        suspendTimers.Remove(obj);
        originalHeights.Remove(obj);
        suspendStartPositions.Remove(obj);
        hasBeenLifted.Remove(obj);
        rangeCheckTimers.Remove(obj);

        Debug.Log($"Objeto restaurado completamente: {obj.name}");
    }

    void UpdateTemporarySuspension(GameObject user)
    {
        foreach (var obj in affectedObjects)
        {
            if (obj == null) continue;

            var rb = obj.GetComponent<Rigidbody>();
            if (rb == null) continue;

            bool isControlled = isBeingControlled.ContainsKey(obj) && isBeingControlled[obj];
            if (!isControlled) continue;

            if (!suspendTimers.ContainsKey(obj)) continue;

            suspendTimers[obj] -= Time.deltaTime;

            if (suspendTimers[obj] > 0)
            {
                float targetHeight = originalHeights.ContainsKey(obj) ? originalHeights[obj] + suspendHeight : suspendHeight;

                if (obj.transform.position.y < targetHeight)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, levitationSpeed, rb.linearVelocity.z);
                    hasBeenLifted[obj] = true;
                }
                else if (hasBeenLifted.ContainsKey(obj) && hasBeenLifted[obj])
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                    float floatOffset = Mathf.Sin(Time.time * 2f + obj.GetInstanceID()) * 0.05f;
                    Vector3 pos = obj.transform.position;
                    pos.y += floatOffset * Time.deltaTime;
                    obj.transform.position = pos;
                }
            }
            else
            {
                if (obj.transform.position.y > originalHeights[obj] + 0.1f)
                {
                    rb.AddForce(-Physics.gravity * 0.5f, ForceMode.Acceleration);
                }
                else
                {
                    RestoreObjectCompletely(obj);
                }
            }
        }
    }

    bool IsObjectInRange(GameObject obj, GameObject user)
    {
        if (obj == null || user == null) return false;
        float distance = Vector3.Distance(obj.transform.position, user.transform.position);
        return distance <= antiGravityRange;
    }

    void RestoreAgent(GameObject obj)
    {
        var agent = obj.GetComponent<NavMeshAgent>();
        if (agent != null && originalNavMeshState.ContainsKey(obj))
        {
            agent.enabled = originalNavMeshState[obj];
            if (agent.enabled)
            {
                agent.Warp(obj.transform.position);
            }
        }
        else if (agent != null)
        {
            agent.enabled = true;
        }
    }

    void SwitchGravityMode(GameObject user)
    {
        switch (currentMode)
        {
            case GravityMode.Normal:
                currentMode = GravityMode.Anti;
                foreach (var obj in affectedObjects)
                {
                    if (obj != null && IsObjectInRange(obj, user))
                    {
                        isBeingControlled[obj] = true;
                        originalHeights[obj] = obj.transform.position.y;
                        suspendTimers[obj] = suspendDuration;
                        suspendStartPositions[obj] = obj.transform.position;
                        hasBeenLifted[obj] = false;

                        var agent = obj.GetComponent<NavMeshAgent>();
                        if (agent != null && agent.enabled)
                        {
                            agent.enabled = false;
                        }
                    }
                }
                break;
            case GravityMode.Anti:
                currentMode = GravityMode.Push;
                break;
            case GravityMode.Push:
                currentMode = GravityMode.Pull;
                break;
            case GravityMode.Pull:
                currentMode = GravityMode.Normal;
                foreach (var obj in affectedObjects)
                {
                    if (obj != null)
                    {
                        RestoreObjectCompletely(obj);
                    }
                }
                break;
        }

        Debug.Log("Modo de gravidade alterado para: " + currentMode);

        if (gravityField != null)
        {
            var renderer = gravityField.GetComponent<Renderer>();
            if (renderer != null)
            {
                Color color = renderer.material.color;
                switch (currentMode)
                {
                    case GravityMode.Anti: color = Color.blue; break;
                    case GravityMode.Push: color = Color.red; break;
                    case GravityMode.Pull: color = Color.green; break;
                    case GravityMode.Normal: color = Color.white; break;
                }
                renderer.material.color = color;
            }
        }

        ApplyCurrentMode(user);
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        isActive = false;

        foreach (var obj in affectedObjects)
        {
            if (obj != null)
            {
                RestoreObjectCompletely(obj);
            }
        }

        affectedObjects.Clear();
        ClearDictionaries();

        if (gravityField != null)
        {
            Destroy(gravityField);
        }

        Debug.Log("GravityControlPower desativado");
    }
}