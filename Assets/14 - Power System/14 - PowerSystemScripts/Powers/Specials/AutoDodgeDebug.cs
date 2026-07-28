using UnityEngine;
using System.Collections.Generic;

public class AutoDodgeDebug : MonoBehaviour
{
    [Header("Referências")]
    private PowerUser powerUser;
    private AutoDodgePower autoDodge;
    private Dictionary<Power, bool> activePowers;

    [Header("Visualização")]
    public bool showGizmos = true;
    public Color detectionColor = Color.yellow;
    public Color dodgeColor = Color.green;

    void Start()
    {
        powerUser = GetComponent<PowerUser>();
        if (powerUser != null)
        {
            // Usa reflection para acessar o dicionário privado
            var field = typeof(PowerUser).GetField("activePowers",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (field != null)
            {
                activePowers = field.GetValue(powerUser) as Dictionary<Power, bool>;
            }

            foreach (var power in powerUser.powers)
            {
                if (power is AutoDodgePower)
                {
                    autoDodge = power as AutoDodgePower;
                    Debug.Log("AutoDodgePower encontrado!");
                    break;
                }
            }
        }

        if (autoDodge == null)
        {
            Debug.LogWarning("AutoDodgePower não encontrado no PowerUser!");
        }
    }

    void Update()
    {
        // Verifica se o Auto-Dodge está ativo
        if (autoDodge != null && powerUser != null && activePowers != null)
        {
            bool isActive = activePowers.ContainsKey(autoDodge) && activePowers[autoDodge];

            // Mostra no console quando o Auto-Dodge ativa
            if (isActive && Time.frameCount % 60 == 0)
            {
                Debug.Log($"🟢 Auto-Dodge está ATIVO");
            }
        }
    }

    // Visualização do alcance de detecção no editor
    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        if (autoDodge != null)
        {
            // Círculo de detecção
            Gizmos.color = detectionColor;
            Gizmos.DrawWireSphere(transform.position, autoDodge.detectionRadius);
        }
    }
}