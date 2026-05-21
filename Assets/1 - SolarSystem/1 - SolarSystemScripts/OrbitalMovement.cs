using System;
using UnityEngine;

public class OrbitalMovement : MonoBehaviour
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

    [Header("Tilt Parameters (Funil/Conic)")]
    [SerializeField] private float coneTiltAngle = 30f; // Angle of the cone tilt (e.g., 30 degrees)
    [SerializeField] private bool keepBasePointingToSun = false; // Keeps the base pointing to the sun

    [Header("Spin Parameters")]
    [SerializeField] private float spinSpeed = 20f; // Spin speed around its own axis
    [SerializeField] private bool spinOnOwnAxis = true; // If true, spins around its own axis

    void Update()
    {
        // Orbital movement
        OrbitAroundPointBetter(centerTransform, radius, orbitalSpeed);

        // Apply conic tilt (always pointing the base to the sun)
        ApplyConicTilt();

        // Spin around its own axis (optional)
        if (spinOnOwnAxis)
        {
            SpinOnAxis();
        }
    }

    // Original method using RotateAround
    // Needs to manually push the object away from the center, so the orbit works correctly, otherwise it will just rotate in place
    private void OrbitAroundPoint()
    {
        if (centerTransform != null)
        {
            // Rotate around the center object at a speed of 20 degrees per second
            transform.RotateAround(centerTransform.position, Vector3.right + Vector3.up, 20 * Time.deltaTime);
            //Vector3.up : horizontal -> clockwise;
            //Vector3.down : horizontal -> counter-clockwise;
            //Vector3.right : vertical -> clockwise;
            //Vector3.left : vertical -> counter-clockwise;
        }
    }

    private void OrbitAroundPointBetter(Transform center, float radius, float speed)
    {
        if (center == null) return;

        angle = Time.time * speed; // Calculates the angle based on time and speed

        // Position in the XZ plane (horizontal)
        horizontalX = Mathf.Cos(angle) * radius;
        horizontalZ = Mathf.Sin(angle) * radius;

        // Apply orbital inclination by converting degrees to radians
        inclinationRad = orbitalInclination * Mathf.Deg2Rad;

        // Rotate the vector around the X axis to create the orbital inclination
        x = horizontalX * Mathf.Cos(inclinationRad);
        z = horizontalZ;
        y = horizontalX * Mathf.Sin(inclinationRad);

        Vector3 offset = new Vector3(x, y, z);
        transform.position = center.position + offset;

        angle %= 360; // Limits the angle value to avoid overflow
    }

    private void ApplyConicTilt()
    {
        if (centerTransform == null) return;

        // Direction from the object to the center (sun)
        Vector3 directionToCenter = (centerTransform.position - transform.position).normalized;

        if (keepBasePointingToSun)
        {
            // Option 1: The base of the object (bottom part) always points to the sun
            // This makes the object "lie down" on the cone, with the base facing the center

            // Calculate the rotation needed for the object's "down" vector to point to the sun
            Quaternion targetRotation = Quaternion.FromToRotation(-transform.up, directionToCenter);
            transform.rotation = targetRotation * transform.rotation;

            // Now apply the additional cone tilt (if necessary)
            // The cone tilt angle represents the inclination of the funnel wall
            if (coneTiltAngle != 0)
            {
                // Find the axis perpendicular to the plane formed by the object and the sun
                Vector3 tiltAxis = Vector3.Cross(transform.up, directionToCenter);
                if (tiltAxis.magnitude > 0.001f)
                {
                    tiltAxis.Normalize();
                    Quaternion coneTilt = Quaternion.AngleAxis(coneTiltAngle, tiltAxis);
                    transform.rotation = coneTilt * transform.rotation;
                }
            }
        }
        else
        {
            // Option 2: Maintains a fixed orientation relative to the sun (like a rigid funnel)
            // The object always maintains the same angle relative to the radial line

            // Calculate the vector perpendicular to the orbital plane
            Vector3 orbitPlaneNormal = Vector3.up;

            // The object should be tilted towards the sun based on the coneTiltAngle
            // This creates the "funnel" effect
            Vector3 radialDirection = directionToCenter;

            // Calculate the rotation axis (perpendicular to the plane formed by the radial and the up)
            Vector3 rotationAxis = Vector3.Cross(radialDirection, orbitPlaneNormal);
            if (rotationAxis.magnitude > 0.001f)
            {
                rotationAxis.Normalize();
                Quaternion tiltRotation = Quaternion.AngleAxis(coneTiltAngle, rotationAxis);

                // Apply the rotation so that the object is tilted like on the funnel wall
                transform.rotation = tiltRotation;
            }
        }
    }

    private void SpinOnAxis()
    {
        // Spins on its own local axis (the axis that points upwards/direction of the cone)
        transform.Rotate(transform.up, spinSpeed * Time.deltaTime, Space.World);
    }

    //// Public method to adjust the cone tilt angle in real-time
    //public void SetConeTiltAngle(float angle)
    //{
    //    coneTiltAngle = Mathf.Clamp(angle, -90, 90);
    //}

    //// Public method to adjust the spin speed in real-time
    //public void SetSpinSpeed(float speed)
    //{
    //    spinSpeed = Mathf.Max(0, speed);
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