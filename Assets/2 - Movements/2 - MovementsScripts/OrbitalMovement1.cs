using System;
using UnityEngine;

public class OrbitalMovement1 : MonoBehaviour
{
    [Header("Orbital Parameters")]
    [SerializeField] private Transform centerTransform;
    [SerializeField] private float radius;
    [SerializeField] private float orbitalSpeed = 1;
    [SerializeField] private float orbitalInclination = 0; // Orbital inclination in degrees

    [Header("Orbit Movement Option")]
    [SerializeField] private bool orbitAroundPoint = true;
    [SerializeField] private float rotationSpeed = 60f;

    [Header("Debug")]
    [SerializeField] private float angle;
    [SerializeField] private float horizontalX;
    [SerializeField] private float horizontalZ;
    [SerializeField] private float inclinationRad;

    void Update()
    {
        if (orbitAroundPoint && centerTransform != null)
        {
            horizontalX = Input.GetAxis("Horizontal");
            horizontalZ = Input.GetAxis("Vertical");

            Rotate();

            OrbitAroundPoint();
        }
    }

    private void Rotate()
    {
        float angle = horizontalX * rotationSpeed * Time.deltaTime;
        transform.Rotate(angle * Vector3.up);
    }

    // Needs to manually push the object away from the center, so the orbit works correctly, otherwise it will just rotate in place
    private void OrbitAroundPoint()
    {
        transform.RotateAround(centerTransform.position, horizontalZ * transform.right /*transform.forward*/, orbitalSpeed * Time.deltaTime);
        //Vector3.up : horizontal -> clockwise;
        //Vector3.down : horizontal -> counter-clockwise;
        //Vector3.right : vertical -> clockwise;
        //Vector3.left : vertical -> counter-clockwise;
    }


    //  Method for debug: Visualize the orientation in the Scene View
    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying && centerTransform != null)
        {
            // Draw the direction to the center (sun)
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, centerTransform.position);

            // Draw the "up" of the object (the part that points upwards on the cone)
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.up * 1.5f);

            // Draw the "forward" of the object
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, transform.forward * 1.5f);

            // Draw the "right" of the object
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.right * 1.5f);

            // Draw a visual cone to represent the funnel
            Gizmos.color = new Color(1, 1, 0, 0.3f);
            Vector3 coneDirection = (transform.position - centerTransform.position).normalized;
            Vector3 coneBase = transform.position + coneDirection * radius * 0.1f;
            for (int i = 0; i < 360; i += 30)
            {
                float rad = i * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius * 0.05f;
                Gizmos.DrawLine(coneBase, coneBase + offset);
            }
        }
    }
}