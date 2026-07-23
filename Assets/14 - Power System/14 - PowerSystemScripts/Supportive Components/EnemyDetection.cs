using UnityEngine;
using System.Collections.Generic;

public class EnemyDetection : MonoBehaviour
{
    [Header("Configurações de Detecção")]
    public float detectionRange = 10f;
    public float detectionAngle = 60f;
    public LayerMask enemyLayers;
    public LayerMask obstacleLayers;
    public bool useLineOfSight = true;

    [Header("Referências")]
    public List<Transform> detectedEnemies = new List<Transform>();
    public Transform currentTarget;
    public Transform closestEnemy;

    [Header("Eventos")]
    public bool onDetectionEvent = false;

    void Update()
    {
        DetectEnemies();
    }

    void DetectEnemies()
    {
        detectedEnemies.Clear();

        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRange, enemyLayers);

        foreach (var enemy in enemies)
        {
            if (enemy.gameObject == gameObject) continue;

            Vector3 directionToEnemy = enemy.transform.position - transform.position;
            float distance = directionToEnemy.magnitude;

            // Verifica se está no cone de visão
            float angle = Vector3.Angle(transform.forward, directionToEnemy);

            if (angle <= detectionAngle / 2)
            {
                // Verifica linha de visão (sem obstáculos)
                if (useLineOfSight)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, directionToEnemy.normalized, out hit, distance, obstacleLayers))
                    {
                        if (hit.collider.gameObject == enemy.gameObject)
                        {
                            detectedEnemies.Add(enemy.transform);
                        }
                    }
                }
                else
                {
                    detectedEnemies.Add(enemy.transform);
                }
            }
        }

        // Encontra o inimigo mais próximo
        if (detectedEnemies.Count > 0)
        {
            closestEnemy = detectedEnemies[0];
            float closestDistance = Vector3.Distance(transform.position, closestEnemy.position);

            foreach (var enemy in detectedEnemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }

            currentTarget = closestEnemy;
        }
        else
        {
            currentTarget = null;
            closestEnemy = null;
        }

        // Evento de detecção (para sistemas de alerta)
        if (onDetectionEvent && detectedEnemies.Count > 0)
        {
            // Disparar evento aqui se necessário
        }
    }

    public Transform GetClosestEnemy()
    {
        return closestEnemy;
    }

    public List<Transform> GetDetectedEnemies()
    {
        return detectedEnemies;
    }

    public bool IsEnemyDetected(Transform enemy)
    {
        return detectedEnemies.Contains(enemy);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Cone de visão
        Vector3 forward = transform.forward * detectionRange;
        Vector3 right = Quaternion.Euler(0, detectionAngle / 2, 0) * forward;
        Vector3 left = Quaternion.Euler(0, -detectionAngle / 2, 0) * forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + forward);
        Gizmos.DrawLine(transform.position, transform.position + right);
        Gizmos.DrawLine(transform.position, transform.position + left);
    }
}