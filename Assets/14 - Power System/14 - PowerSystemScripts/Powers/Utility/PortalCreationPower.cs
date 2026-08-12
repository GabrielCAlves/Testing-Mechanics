using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewPortalCreationPower", menuName = "Powers/Utility/Portal Creation Power")]
public class PortalCreationPower : Power
{
    [Header("Configurações de Portal")]
    public float portalRange = 10f;
    public float portalRadius = 2f;
    public float portalLifetime = 30f;
    public float defaultDistance = 5f;
    public Vector3 offsetTeleportPosition;
    public float teleportCooldown = 0.5f;  // Tempo entre teleportes do mesmo objeto
    public GameObject portalPrefab;
    public Material portalMaterial;
    public Color portalColor = Color.cyan;
    public AudioClip portalSound;

    private GameObject portal1;
    private GameObject portal2;
    private Vector3 portal1Position;
    private Vector3 portal2Position;
    private bool hasPortal1 = false;
    private bool hasPortal2 = false;
    private bool isConnecting = false;
    private float portal1Timer = 0f;
    private float portal2Timer = 0f;
    private bool isActive = false;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        isActive = true;

        // Verifica se os portais ainda existem (se foram destruídos externamente)
        ValidatePortals();

        CreatePortal(user);
    }

    public override void UpdatePower(GameObject user)
    {
        if (!isActive || user == null) return;

        // Atualiza os timers dos portais
        UpdatePortalTimers();
    }

    void UpdatePortalTimers()
    {
        // Atualiza timer do portal 1
        if (hasPortal1 && portal1 != null)
        {
            portal1Timer -= Time.deltaTime;
            if (portal1Timer <= 0)
            {
                RemovePortal1();
                Debug.Log("Portal 1 expirou!");
            }
        }

        // Atualiza timer do portal 2
        if (hasPortal2 && portal2 != null)
        {
            portal2Timer -= Time.deltaTime;
            if (portal2Timer <= 0)
            {
                RemovePortal2();
                Debug.Log("Portal 2 expirou!");
            }
        }

        // Se ambos os portais foram removidos, reseta a conexão
        if (!hasPortal1 && !hasPortal2)
        {
            isConnecting = false;
        }
    }

    void ValidatePortals()
    {
        // Verifica se o portal1 foi destruído
        if (portal1 == null && hasPortal1)
        {
            hasPortal1 = false;
            Debug.Log("Portal1 foi destruído externamente, resetando estado");
        }

        // Verifica se o portal2 foi destruído
        if (portal2 == null && hasPortal2)
        {
            hasPortal2 = false;
            Debug.Log("Portal2 foi destruído externamente, resetando estado");
        }
    }

    void CreatePortal(GameObject user)
    {
        if (user == null) return;

        Vector3 position;
        Quaternion rotation;
        bool hitSomething = false;

        // Tenta o raycast
        Ray ray = new Ray(user.transform.position, user.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, portalRange))
        {
            // Raycast atingiu algo - cria portal na superfície
            position = hit.point + hit.normal * 0.5f;
            rotation = Quaternion.LookRotation(user.transform.position);
            hitSomething = true;
            Debug.Log($"Portal criado na superfície: {hit.collider.gameObject.name}");
        }
        else
        {
            // Raycast não atingiu nada - cria portal na frente do jogador
            position = user.transform.position + user.transform.forward * defaultDistance;

            // Ajusta a posição Y para o nível do jogador
            position.y = user.transform.position.y;

            // Rotaciona para olhar na direção do jogador
            rotation = Quaternion.LookRotation(user.transform.position);
            Debug.Log($"Portal criado no ar à {defaultDistance}m de distância");
        }

        // --- CRIAÇÃO DO PORTAL 1 ---
        if (!hasPortal1 || portal1 == null)
        {
            // Remove portal antigo se existir
            if (portal1 != null)
            {
                Destroy(portal1);
                portal1 = null;
            }

            // Cria portal 1
            portal1 = Instantiate(portalPrefab, position, rotation);
            portal1.transform.localScale = Vector3.one * portalRadius;
            portal1Position = position;
            hasPortal1 = true;
            portal1Timer = portalLifetime;

            // Configura material
            var renderer = portal1.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = portalMaterial;
                renderer.material.color = portalColor;
            }

            // Adiciona identificador
            var identifier = portal1.GetComponent<PortalIdentifier>();
            if (identifier == null)
                identifier = portal1.AddComponent<PortalIdentifier>();
            identifier.portalNumber = 1;

            // Adiciona o script de teleporte
            if (portal1.GetComponent<PortalTeleporter>() == null)
                portal1.AddComponent<PortalTeleporter>();

            // Se não atingiu nada, adiciona um efeito visual de "portal flutuante"
            //if (!hitSomething)
            //{
            //    AddFloatingEffect(portal1);
            //}

            Debug.Log($"Portal 1 criado! Vida: {portalLifetime}s");
        }
        // --- CRIAÇÃO DO PORTAL 2 ---
        else if (!hasPortal2 || portal2 == null)
        {
            // Remove portal antigo se existir
            if (portal2 != null)
            {
                Destroy(portal2);
                portal2 = null;
            }

            // Cria portal 2
            portal2 = Instantiate(portalPrefab, position, rotation);
            portal2.transform.localScale = Vector3.one * portalRadius;
            portal2Position = position;
            hasPortal2 = true;
            portal2Timer = portalLifetime;

            // Configura material
            var renderer = portal2.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = portalMaterial;
                renderer.material.color = portalColor;
            }

            // Adiciona identificador
            var identifier = portal2.GetComponent<PortalIdentifier>();
            if (identifier == null)
                identifier = portal2.AddComponent<PortalIdentifier>();
            identifier.portalNumber = 2;

            // Adiciona o script de teleporte
            if (portal2.GetComponent<PortalTeleporter>() == null)
                portal2.AddComponent<PortalTeleporter>();

            // Se não atingiu nada, adiciona um efeito visual de "portal flutuante"
            //if (!hitSomething)
            //{
            //    AddFloatingEffect(portal2);
            //}

            // Conecta os portais
            ConnectPortals();

            Debug.Log($"Portal 2 criado! Vida: {portalLifetime}s");
        }
        // --- ATUALIZA PORTAL 1 (move para nova posição) ---
        else
        {
            // Move portal 1 para nova posição
            if (portal1 != null)
            {
                portal1.transform.position = position;
                portal1.transform.rotation = rotation;
                portal1Position = position;
                portal1Timer = portalLifetime; // Reseta o timer
                Debug.Log($"Portal 1 movido e timer resetado");
            }
            else
            {
                // Se portal1 foi destruído, reseta o estado
                hasPortal1 = false;
                CreatePortal(user);
                return;
            }
        }

        // Efeito sonoro
        if (portalSound != null)
        {
            AudioSource.PlayClipAtPoint(portalSound, position);
        }
    }

    void AddFloatingEffect(GameObject portal)
    {
        // Adiciona um efeito de flutuação para portais no ar
        var floating = portal.AddComponent<PortalFloatingEffect>();
        floating.floatSpeed = 0.5f;
        floating.floatHeight = 0.3f;
        floating.rotationSpeed = 20f;
    }

    void ConnectPortals()
    {
        if (portal1 == null || portal2 == null) return;

        //if (isConnecting) return;

        isConnecting = true;

        // Configura o teleporter do portal 1
        var teleporter1 = portal1.GetComponent<PortalTeleporter>();
        if (teleporter1 != null)
        {
            teleporter1.targetPortal = portal2;
            teleporter1.targetPosition = portal2Position;
            teleporter1.offsetTeleportPosition = offsetTeleportPosition;
            teleporter1.teleportCooldown = teleportCooldown;
            teleporter1.portalPower = this;
        }

        // Configura o teleporter do portal 2
        var teleporter2 = portal2.GetComponent<PortalTeleporter>();
        if (teleporter2 != null)
        {
            teleporter2.targetPortal = portal1;
            teleporter2.targetPosition = portal1Position;
            teleporter2.offsetTeleportPosition = offsetTeleportPosition;
            teleporter2.teleportCooldown = teleportCooldown;
            teleporter2.portalPower = this;
        }

        Debug.Log("Portais conectados!");
    }

    void DisconnectPortals()
    {
        if (portal1 != null)
        {
            var teleporter1 = portal1.GetComponent<PortalTeleporter>();
            if (teleporter1 != null)
            {
                teleporter1.targetPortal = null;
            }
        }

        if (portal2 != null)
        {
            var teleporter2 = portal2.GetComponent<PortalTeleporter>();
            if (teleporter2 != null)
            {
                teleporter2.targetPortal = null;
            }
        }

        isConnecting = false;
        Debug.Log("Portais desconectados");
    }

    public void RemovePortal1()
    {
        if (portal1 != null)
        {
            Destroy(portal1);
            portal1 = null;
        }
        hasPortal1 = false;
        portal1Timer = 0f;
        Debug.Log("Portal 1 removido");
    }

    public void RemovePortal2()
    {
        if (portal2 != null)
        {
            Destroy(portal2);
            portal2 = null;
        }
        hasPortal2 = false;
        portal2Timer = 0f;
        Debug.Log("Portal 2 removido");
    }

    public void RemoveAllPortals()
    {
        DisconnectPortals();
        RemovePortal1();
        RemovePortal2();
        isConnecting = false;
        Debug.Log("Todos os portais removidos");
    }

    public override void Deactivate(GameObject user)
    {
        base.Deactivate(user);
        isActive = false;

        // Apenas desativa o poder, os portais continuam com seus timers
        Debug.Log("PortalCreationPower desativado (portais mantidos)");
    }

    public bool IsPortalValid(GameObject portal)
    {
        return portal != null;
    }
}

// --- PORTAL FLOATING EFFECT (para portais no ar) ---
public class PortalFloatingEffect : MonoBehaviour
{
    public float floatSpeed = 0.5f;
    public float floatHeight = 0.3f;
    public float rotationSpeed = 20f;

    private Vector3 startPosition;
    private float floatTimer = 0f;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Efeito de flutuação
        floatTimer += Time.deltaTime * floatSpeed;
        float offsetY = Mathf.Sin(floatTimer) * floatHeight;
        transform.position = startPosition + new Vector3(0, offsetY, 0);

        // Rotação suave
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}

// --- PORTAL IDENTIFIER ---
public class PortalIdentifier : MonoBehaviour
{
    public int portalNumber;
}

// --- PORTAL TELEPORTER ---
public class PortalTeleporter : MonoBehaviour
{
    public GameObject targetPortal;
    public Vector3 targetPosition;
    public Vector3 offsetTeleportPosition;
    public float teleportDelay = 0.1f;
    public PortalCreationPower portalPower;

    // --- Sistema de cooldown para evitar loops ---
    public float teleportCooldown = 0.5f; // Tempo mínimo entre teleportes do mesmo objeto
    private Dictionary<GameObject, float> lastTeleportTimes = new Dictionary<GameObject, float>();

    void OnTriggerEnter(Collider other)
    {
        // Ignora triggers
        if (other.isTrigger) return;
        if (other.gameObject == gameObject) return;
        if (targetPortal == null) return;
        if (portalPower != null && !portalPower.IsPortalValid(targetPortal)) return;

        // --- VERIFICA COOLDOWN ---
        if (lastTeleportTimes.ContainsKey(other.gameObject))
        {
            float timeSinceLastTeleport = Time.time - lastTeleportTimes[other.gameObject];
            if (timeSinceLastTeleport < teleportCooldown)
            {
                Debug.Log($"Objeto {other.gameObject.name} em cooldown de teleporte. Aguarde {teleportCooldown - timeSinceLastTeleport:F2}s");
                return;
            }
        }

        // Verifica se o objeto tem CharacterController ou Rigidbody
        if (other.GetComponent<CharacterController>() == null && other.GetComponent<Rigidbody>() == null)
        {
            return;
        }

        // Marca o tempo do teleporte
        lastTeleportTimes[other.gameObject] = Time.time;

        StartCoroutine(TeleportObject(other.gameObject));
    }

    System.Collections.IEnumerator TeleportObject(GameObject obj)
    {
        yield return new WaitForSeconds(teleportDelay);

        if (obj == null) yield break;
        if (targetPortal == null) yield break;

        targetPortal.GetComponent<Collider>().enabled = false; // Desativa o collider do portal de destino temporariamente
        Vector3 targetPos = targetPosition + offsetTeleportPosition + targetPortal.transform.forward * 1f;

        // --- 1. LIDA COM CHARACTER CONTROLLER ---
        CharacterController controller = obj.GetComponent<CharacterController>();
        if (controller != null)
        {
            // Salva a velocidade atual
            Vector3 velocity = controller.velocity;

            // Desativa o CharacterController
            controller.enabled = false;

            // Teleporta
            obj.transform.position = targetPos;

            // Reativa o CharacterController
            controller.enabled = true;

            // Aplica um pequeno movimento para frente para evitar colisões
            controller.Move(targetPortal.transform.forward * 0.2f);
        }

        // --- 2. LIDA COM RIGIDBODY ---
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Se não tem CharacterController, teleporta normalmente
            if (controller == null)
            {
                rb.position = targetPos;
            }

            // Mantém o momentum
            rb.linearVelocity = targetPortal.transform.forward * rb.linearVelocity.magnitude;
        }
        else if (controller == null)
        {
            // Se não tem nem CharacterController nem Rigidbody, teleporta via transform
            obj.transform.position = targetPos;
        }

        // --- 3. LIDA COM NAVMESHAGENT ---
        UnityEngine.AI.NavMeshAgent agent = obj.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null && agent.isActiveAndEnabled)
        {
            // Força o warp para o NavMesh
            agent.Warp(obj.transform.position);

            // Reseta o destino para evitar que o agente tente voltar
            agent.ResetPath();
        }

        // --- 4. LIDA COM PLAYERMOVEMENT (se existir) ---
        var playerMovement = obj.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            // Força uma atualização do movimento
            playerMovement.enabled = false;
            playerMovement.enabled = true;
        }

        yield return new WaitForSeconds(teleportCooldown);
        targetPortal.GetComponent<Collider>().enabled = true; // Reativa o collider do portal de destino

        Debug.Log($"Teleporte concluído para: {obj.name} em {targetPos}");
    }

    void OnDestroy()
    {
        // Limpa o dicionário quando o portal é destruído
        lastTeleportTimes.Clear();
    }
}