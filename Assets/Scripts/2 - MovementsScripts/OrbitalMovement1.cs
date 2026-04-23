using System;
using UnityEngine;

public class OrbitalMovement1 : MonoBehaviour
{
    [Header("Orbital Parameters")]
    [SerializeField] private Transform centerTransform;
    [SerializeField] private float radius;
    [SerializeField] private float orbitalSpeed = 1;
    [SerializeField] private float orbitalInclination = 0; // Orbital inclination in degrees

    [Header("Debug")]
    [SerializeField] private float x = 0;
    [SerializeField] private float y = 0;
    [SerializeField] private float z = 0;
    [SerializeField] private float angle;
    [SerializeField] private float horizontalX;
    [SerializeField] private float horizontalZ;
    [SerializeField] private float inclinationRad;

    [Header("Orbit Movement Option")]
    [SerializeField] private bool orbitAroundPoint = true;
    [SerializeField] private float rotationSpeed = 60f; // Speed of the object's own rotation
    //[SerializeField] private bool orbitAroundPointBetter;
    //[SerializeField] private GameObject lookObject;

    private void Start()
    {
        //if(lookObject != null)
        //{
        //    transform.LookAt(lookObject.transform);
        //}
    }

    void Update()
    {
        if (orbitAroundPoint)
        {
            horizontalX = Input.GetAxis("Horizontal");
            horizontalZ = Input.GetAxis("Vertical");

            Rotate();
            OrbitAroundPoint();
        }

        //if (orbitAroundPointBetter)
        //{
        //    // Orbital movement
        //    OrbitAroundPointBetter(centerTransform, radius, orbitalSpeed);
        //}
    }

    private void Rotate()
    {
        if(horizontalX != 0)
        {
            // Calculate the angle of rotation based on the vertical input
            float angle = horizontalX * rotationSpeed * Time.deltaTime;
            // Rotate the object around its own axis (for example, to simulate spinning)
            transform.Rotate(angle * Vector3.up);
        }
        // Rotate the object around its own axis (for example, to simulate spinning)
        //transform.Rotate(0, horizontalZ * rotationSpeed * Time.deltaTime, 0);
        //transform.Rotate(horizontalZ * Vector3.right, rotationSpeed * Time.deltaTime);
    }

    // Needs to manually push the object away from the center, so the orbit works correctly, otherwise it will just rotate in place
    private void OrbitAroundPoint()
    {
        if (centerTransform != null)
        {
            // Rotate around the center object at a speed of 20 degrees per second
            transform.RotateAround(centerTransform.position,/* horizontalX * Vector3.up +*/ horizontalZ * Vector3.up, orbitalSpeed * Time.deltaTime);
            //Vector3.up : horizontal -> clockwise;
            //Vector3.down : horizontal -> counter-clockwise;
            //Vector3.right : vertical -> clockwise;
            //Vector3.left : vertical -> counter-clockwise;
        }
    }

    //private void OrbitAroundPointBetter(Transform center, float radius, float speed)
    //{
    //    if (center == null) return;

    //    angle = Time.time * speed; // Calculates the angle based on time and speed

    //    // Position in the XZ plane (horizontal)
    //    horizontalX = Input.GetAxis("Horizontal") * Mathf.Cos(angle) * radius;
    //    horizontalZ = Input.GetAxis("Vertical") * Mathf.Sin(angle) * radius;

    //    // Apply orbital inclination by converting degrees to radians
    //    inclinationRad = orbitalInclination * Mathf.Deg2Rad;

    //    // Rotate the vector around the X axis to create the orbital inclination
    //    x = horizontalX * Mathf.Cos(inclinationRad);
    //    z = horizontalZ;
    //    y = horizontalX * Mathf.Sin(inclinationRad);

    //    Vector3 offset = new Vector3(x, y, z);
    //    transform.position = center.position + offset;

    //    angle %= 360; // Limits the angle value to avoid overflow
    //}

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