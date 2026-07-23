using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewIntangibilityPower", menuName = "Powers/Defensive/Intangibility Power")]
public class IntangibilityPower : Power
{
    [Header("Configurações de Intangibilidade")]
    public float duration = 5f;
    public float transparencyLevel = 0.3f;
    public GameObject ghostEffectPrefab;
    public AudioClip phasingSound;

    [Header("Configurações de Movimento")]
    public bool disableCharacterController = true;
    public bool disableMovement = true;
    public bool disableCollisions = true;

    [Header("Configurações de Atravessar")]
    public bool enableNoClip = true;
    public bool useIntangibleLayer = true;

    [Header("Layers que serão atravessadas (selecione as que deseja ignorar)")]
    public LayerMask layersToIgnore = 0; // <-- Selecione manualmente no Inspector

    [Header("Layers que NÃO serão atravessadas (prioridade sobre layersToIgnore)")]
    public LayerMask layersToKeepCollision = 0; // <-- Ex: Ground, Floor

    private bool isIntangible = false;
    private float timer;
    private GameObject ghostEffect;
    private Collider[] colliders;
    private CharacterController characterController;
    private Renderer[] renderers;
    private PlayerMovement playerMovement;
    private bool originalControllerEnabled;
    private float originalMoveSpeed;
    private MonoBehaviour ownerMonoBehaviour;
    private Rigidbody rb;
    private bool originalRigidbodyKinematic;
    private CollisionDetectionMode originalCollisionMode;
    private int originalLayer;
    private int intangibleLayer = -1;
    private List<KeyValuePair<int, int>> restoredCollisions = new List<KeyValuePair<int, int>>();

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        ownerMonoBehaviour = user.GetComponent<MonoBehaviour>();
        if (ownerMonoBehaviour != null)
        {
            ownerMonoBehaviour.StartCoroutine(MakeIntangibleCoroutine(user));
        }
    }

    IEnumerator MakeIntangibleCoroutine(GameObject user)
    {
        // Salva referências
        colliders = user.GetComponentsInChildren<Collider>();
        renderers = user.GetComponentsInChildren<Renderer>();
        characterController = user.GetComponent<CharacterController>();
        playerMovement = user.GetComponent<PlayerMovement>();
        rb = user.GetComponent<Rigidbody>();
        originalLayer = user.layer;
        restoredCollisions.Clear();

        // --- CONFIGURAÇÃO PARA ATRAVESSAR OBJETOS ---
        if (enableNoClip)
        {
            if (useIntangibleLayer)
            {
                intangibleLayer = LayerMask.NameToLayer("Intangible");
                if (intangibleLayer != -1)
                {
                    user.layer = intangibleLayer;
                    SetLayerRecursively(user, intangibleLayer);
                    Debug.Log($"Layer alterada para Intangible (Layer {intangibleLayer})");

                    // Configura colisões baseado nas layers selecionadas
                    ConfigureSelectiveCollisions(user, true);
                }
                else
                {
                    Debug.LogError("Layer 'Intangible' não encontrada! Crie em Edit > Project Settings > Tags and Layers");
                }
            }
        }

        // --- CONTROLE DO CHARACTER CONTROLLER ---
        if (characterController != null && disableCharacterController)
        {
            originalControllerEnabled = characterController.enabled;
            characterController.enabled = false;
            Debug.Log($"CharacterController desabilitado");
        }

        // --- CONTROLE DOS COLLIDERS ---
        if (disableCollisions)
        {
            foreach (var col in colliders)
            {
                if (col is CharacterController) continue;
                col.enabled = false;
            }
            Debug.Log("Colliders desabilitados");
        }

        // --- CONTROLE DO RIGIDBODY ---
        if (rb != null)
        {
            originalRigidbodyKinematic = rb.isKinematic;
            originalCollisionMode = rb.collisionDetectionMode;

            rb.isKinematic = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            //rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        // --- CONTROLE DO MOVIMENTO ---
        if (playerMovement != null && disableMovement)
        {
            originalMoveSpeed = playerMovement.moveSpeed;
            playerMovement.moveSpeed = 0f;
            playerMovement.enabled = false;
            Debug.Log("PlayerMovement desabilitado");
        }

        // Torna transparente
        foreach (var rend in renderers)
        {
            Color color = rend.material.color;
            color.a = transparencyLevel;
            rend.material.color = color;
        }

        // Efeito fantasma
        if (ghostEffectPrefab != null)
        {
            ghostEffect = Instantiate(ghostEffectPrefab, user.transform);
            ghostEffect.transform.localPosition = Vector3.zero;
        }

        if (phasingSound != null)
        {
            AudioSource.PlayClipAtPoint(phasingSound, user.transform.position);
        }

        isIntangible = true;
        timer = duration;

        // Loop principal
        while (isIntangible && timer > 0)
        {
            timer -= Time.deltaTime;

            // Efeito de flicker
            if (timer < 1f && renderers != null)
            {
                float flicker = Mathf.PingPong(Time.time * 10f, 0.5f);
                foreach (var rend in renderers)
                {
                    Color color = rend.material.color;
                    color.a = transparencyLevel + flicker * 0.5f;
                    rend.material.color = color;
                }
            }

            yield return null;
        }

        Deactivate(user);
    }

    private void ConfigureSelectiveCollisions(GameObject user, bool ignore)
    {
        // Obtém todas as layers
        int maxLayers = 32; // Unity suporta até 32 layers

        for (int i = 0; i < maxLayers; i++)
        {
            string layerName = LayerMask.LayerToName(i);
            if (string.IsNullOrEmpty(layerName)) continue;

            // Verifica se é uma layer que deve ser ignorada
            bool shouldIgnore = (layersToIgnore & (1 << i)) != 0;

            // Verifica se é uma layer que NÃO deve ser ignorada (prioridade)
            bool shouldKeepCollision = (layersToKeepCollision & (1 << i)) != 0;

            // Se deve manter colisão, NÃO ignora
            if (shouldKeepCollision)
            {
                Debug.Log($"Mantendo colisão com layer: {layerName}");
                continue;
            }

            // Se deve ignorar, configura
            if (shouldIgnore)
            {
                int playerLayer = useIntangibleLayer ? intangibleLayer : originalLayer;
                if (playerLayer != -1 && playerLayer != i)
                {
                    Physics.IgnoreLayerCollision(playerLayer, i, ignore);
                    Debug.Log($"{(ignore ? "Ignorando" : "Restaurando")} colisão com layer: {layerName} (Layer {i})");
                }
            }
        }

        // Caso especial: sempre NÃO atravessa o chão se a layer Ground estiver em layersToKeepCollision
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer != -1 && (layersToKeepCollision & (1 << groundLayer)) != 0)
        {
            int playerLayer = useIntangibleLayer ? intangibleLayer : originalLayer;
            if (playerLayer != -1)
            {
                Physics.IgnoreLayerCollision(playerLayer, groundLayer, false);
                Debug.Log($"Mantendo colisão com Ground (configuração manual)");
            }
        }
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    public override void Deactivate(GameObject user)
    {
        if (!isIntangible) return;

        base.Deactivate(user);
        isIntangible = false;

        // --- RESTAURA LAYER ---
        if (enableNoClip && useIntangibleLayer)
        {
            user.layer = originalLayer;
            SetLayerRecursively(user, originalLayer);
            Debug.Log($"Layer restaurada para {originalLayer}");

            // Restaura colisões
            ConfigureSelectiveCollisions(user, false);
        }

        // --- RESTAURA CHARACTER CONTROLLER ---
        if (characterController != null && disableCharacterController)
        {
            characterController.enabled = originalControllerEnabled;
            Debug.Log($"CharacterController restaurado");
        }

        // --- RESTAURA COLLIDERS ---
        if (disableCollisions)
        {
            foreach (var col in colliders)
            {
                if (col is CharacterController) continue;
                col.enabled = true;
            }
            Debug.Log("Colliders restaurados");
        }

        // --- RESTAURA RIGIDBODY ---
        if (rb != null)
        {
            rb.isKinematic = originalRigidbodyKinematic;
            rb.collisionDetectionMode = originalCollisionMode;
            //rb.constraints = RigidbodyConstraints.None;
        }

        // --- RESTAURA MOVIMENTO ---
        if (playerMovement != null && disableMovement)
        {
            playerMovement.moveSpeed = originalMoveSpeed;
            playerMovement.enabled = true;
            Debug.Log("PlayerMovement restaurado");
        }

        // Restaura opacidade
        foreach (var rend in renderers)
        {
            Color color = rend.material.color;
            color.a = 1f;
            rend.material.color = color;
        }

        if (ghostEffect != null)
        {
            Destroy(ghostEffect);
            ghostEffect = null;
        }

        // Força atualização
        if (characterController != null && characterController.enabled)
        {
            characterController.Move(Vector3.zero);
        }

        Debug.Log("Intangibilidade desativada");
    }
}