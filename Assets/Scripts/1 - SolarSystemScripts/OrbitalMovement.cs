using System;
using UnityEngine;

public class OrbitalMovement : MonoBehaviour
{
    [Header("Orbital Parameters")]
    [SerializeField] private Transform centerTransform;
    [SerializeField] private float radius;
    [SerializeField] private float speed;
    [SerializeField] private float inclination = 0; // Inclinação em graus

    [Header("Debug")]
    [SerializeField] private float x = 0;
    [SerializeField] private float y = 0; // Mantém a órbita no plano horizontal
    [SerializeField] private float z = 0;
    [SerializeField] private float angle;
    [SerializeField] private float horizontalX;
    [SerializeField] private float horizontalZ;
    [SerializeField] private float inclinationRad;

    // Update is called once per frame
    void Update()
    {
        OrbitAroundPointBetter(centerTransform, radius, speed);
        //OrbitAroundPoint();
    }

    private void OrbitAroundPoint() //Precisa afastar o objeto do centro manualmente, para que a órbita funcione corretamente
    {
        if (centerTransform != null)
        {
            // Rotate around the center object at a speed of 20 degrees per second
            transform.RotateAround(centerTransform.position, Vector3.right + Vector3.up, 20 * Time.deltaTime);
            //Vector3.up : horizontal -> sentido horário;
            //Vector3.down : horizontal -> sentido anti-horário;
            //Vector3.right : vertical -> sentido horário;
            //Vector3.left : vertical -> sentido anti-horário;
        }
    }

    private void OrbitAroundPointBetter(Transform center, float radius, float speed)
    {
        angle = Time.time * speed; // Calcula o ângulo com base no tempo e na velocidade

        // Posição no plano XZ (horizontal)
        horizontalX = Mathf.Cos(angle) * radius;
        horizontalZ = Mathf.Sin(angle) * radius;

        // Aplica inclinação convertendo graus para radianos
        inclinationRad = inclination * Mathf.Deg2Rad;

        // Rotaciona o vetor no eixo X para criar a inclinação
        x = horizontalX * Mathf.Cos(inclinationRad);
        z = horizontalZ;
        y = horizontalX * Mathf.Sin(inclinationRad);

        Vector3 offset = new Vector3(x, y, z);

        transform.position = center.position + offset;

        angle %= 360; // Limita o valor do ângulo para evitar overflow
    }
}