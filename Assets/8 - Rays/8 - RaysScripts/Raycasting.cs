using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using TMPro;

//More about Raycasting: https://docs.unity3d.com/ScriptReference/Physics.Raycast.html and https://www.youtube.com/watch?v=fJyi7l2tWKo&t=408s
public class Raycasting : MonoBehaviour
{
    [Header("RayCat Configurations")]
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private RaycastHit hit;

    [Header("Effects")]
    [SerializeField] private bool changeColor = false;
    [SerializeField] private bool shineColor = false;
    [SerializeField] private bool raycastAll = false;
    [SerializeField] private bool raycastNonAlloc = false;
    [SerializeField] private bool lineCast = false;
    [SerializeField] private bool colliderRaycast = false;
    [SerializeField] private bool sphereCast = false;
    [SerializeField] private bool boxCast = false;
    [SerializeField] private bool capsuleCast = false;
    [SerializeField] private bool screenPointToRay = false;
    [SerializeField] private bool viewportPointToRay = false;
    [SerializeField] private bool overlapSphereAll3D = false;

    [Header("Additional Configurations")]
    [SerializeField] private Color color = Color.black;
    [SerializeField] private Material material;
    [SerializeField] private GameObject previousObject;

    [Header("Shine Effect Settings")]
    [SerializeField] private float startIntensity = 0f;
    [SerializeField] private float shineIntensity = 7f;
    [SerializeField] private float shineDuration = 5f;
    [SerializeField] private float chargeSpeed = .05f;

    [Header("Raycast Types' Configs")]
    [SerializeField] private RaycastHit[] hits;
    [SerializeField] private RaycastHit[] hitsNonAlloc = new RaycastHit[5];
    [SerializeField] private Vector3 startPoint;
    [SerializeField] private Vector3 endPoint;
    [SerializeField] private Collider colliderToHit;
    [SerializeField] private float sphereRadius = 3f;
    [SerializeField] private Vector3 boxSize;
    [SerializeField] private Vector3 position1 = new Vector3(0,-2,0);
    [SerializeField] private Vector3 position2 = new Vector3(0, 2, 0);
    [SerializeField] private float velocity = 5f;
    [SerializeField] private bool up = false;
    [SerializeField] private bool down = false;
    [SerializeField] private bool right = false;
    [SerializeField] private bool left = false;
    [SerializeField] private Plane plane;
    [SerializeField] private TextMeshProUGUI objectLabel;
    [SerializeField] private int maxNumberOfColliders;
    [SerializeField] private Collider[] colliderOverlapSphereAll;

    [Header("Debug")]
    [SerializeField] private float elapsedTime = 0f;
    [SerializeField] private float intensity = 0f;
    [SerializeField] private float t = 0f;
    [SerializeField] private float smoothT = 0f;

    private void Start()
    {
        colliderOverlapSphereAll = new Collider[maxNumberOfColliders];
    }

    void Update()
    {
        // Common Raycast
        if(Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, layerMask) && previousObject != hit.transform.gameObject)
        {
            Debug.Log($"Hit's name: {hit.transform.name}");
            Debug.Log($"Hit's position: {hit.transform.position}");
            Debug.Log($"Hit's rotation: {hit.transform.rotation}");
            Debug.Log($"Hit's localScale: {hit.transform.localScale}");
            Debug.Log($"Hit's localScale.magnitude: {hit.transform.localScale.magnitude}");
            Debug.Log($"Hit's localScale.normalized:  {hit.transform.localScale.normalized}");
            Debug.Log($"Hit's tag: {hit.transform.tag}");
            Debug.Log($"Hit's distance: {hit.distance}");
            Debug.Log($"Hit's point: {hit.point}");
            Debug.Log($"Hit's normal: {hit.normal}");
            Debug.Log($"Hit's collider: {hit.collider}");
            
            if(hit.rigidbody)
                Debug.Log($"Hit rigidbody: {hit.rigidbody}");

            if (hit.transform.gameObject.GetComponent<Renderer>().material != null)
                material = hit.transform.gameObject.GetComponent<Renderer>().material;

            if (changeColor)
                ChangeColor();

            if (shineColor)
            {
                Shine(hit.transform.gameObject);
            }

            previousObject = hit.transform.gameObject;

            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.green);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * maxDistance, Color.red);
        }

        //RaycastAll
        if (raycastAll)
        {
            hits = Physics.RaycastAll(transform.position, transform.forward); // OR ray = new Ray(transform.position, transform.forward)

            if (hits.Length > 0)
            {
                // Sometimes the hit objects in the array aren't in order of first hit
                Array.Sort(hits, (RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance));

                for (int i = 0; i < hits.Length; ++i)
                {
                    Debug.Log($"RaycastAll hit nº {i}: {hits[i].collider.gameObject.name}");
                }
            }
        }

        //RaycastNonAlloc
        if (raycastNonAlloc)
        {
            ClearHits();

            int numHits = Physics.RaycastNonAlloc(transform.position, transform.forward, hitsNonAlloc);

            // Sometimes the hit objects in the array aren't in order of first hit
            Array.Sort(hitsNonAlloc, (RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance));

            if (numHits > 0)
            {
                for (int i = 0; i < hitsNonAlloc.Length; ++i)
                {
                    Debug.Log(hitsNonAlloc[i].collider.gameObject.name);
                }
            }
        }

        // Linecast
        if (lineCast)
        {
            if (Physics.Linecast(startPoint, endPoint, out RaycastHit hit0))
            {
                Debug.Log($"LineCast caught {hit0.transform.gameObject.name}");
            }
        }

        // Collider.Raycast
        if (colliderRaycast)
        {
            Ray ray4 = new Ray(transform.position, transform.forward);
            if (colliderToHit.Raycast(ray4, out RaycastHit hit2, maxDistance))
            {
                Debug.Log($"Assigned colliderToHit was hit ({hit2.transform.gameObject.name})");
            }
        }

        // SphereCast
        if (sphereCast)
        {
            Ray ray5 = new Ray(transform.position, transform.forward);
            if (Physics.SphereCast(ray5, sphereRadius, out RaycastHit hit0)) // 0.5f is the radius of the sphere
            {
                Debug.Log($"SphereCast caught {hit0.transform.gameObject.name}");
            }
        }

        // BoxCast        // Box extending itself forward forever
        if (boxCast)
        {
            if (Physics.BoxCast(transform.position, boxSize / 2, transform.forward, out RaycastHit hit0))
            {
                Debug.Log($"BoxCast caught {hit0.transform.gameObject.name}");
                hit0.transform.position = new Vector3(hit0.transform.position.x, hit0.transform.position.y + 2, hit0.transform.position.z);
            }
        }

        // CapsuleCast    // Capsule extending itself forward forever
        if (capsuleCast)
        {
            if (Physics.CapsuleCast(position1, position2, sphereRadius, transform.forward, out RaycastHit hit0)) // 0.5f is the radius of the capsule
            {
                Debug.Log($"CapsuleCast caught {hit0.transform.gameObject.name}");
            }
        }

        // Plane.Raycast
        if (screenPointToRay)
        {
            Ray ray6 = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (up)
            {
                plane = new Plane(Vector3.up, 0);
            }else if (down)
            {
                plane = new Plane(Vector3.down, 0);
            }else if (right)
            {
                plane = new Plane(Vector3.right, 0);
            }else if (left)
            {
                plane = new Plane(Vector3.left, 0);
            }

            if (plane.Raycast(ray6, out float distance))
            {
                transform.position = Vector3.Lerp(transform.position, ray6.GetPoint(distance), velocity * Time.deltaTime);
            }
        }

        // ScreenToRay ObjectNameText
        if (Input.GetMouseButtonDown(0))
        {
            FireScreenRay();
        }

        // Viewport CenterShootAimScreen
        if (viewportPointToRay)
        {
            Ray ray7 = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0));
            if (Physics.Raycast(ray7, out RaycastHit hit3))
            {
                Debug.Log(hit3.collider.gameObject.name + " is in the Aim!");
            }
        }

        if(overlapSphereAll3D)
        {
            //Physics.OverlapSphere(transform.position, sphereRadius, layerMask) as RaycastHit[];
            int collidersHit= Physics.OverlapSphereNonAlloc(transform.position, sphereRadius, colliderOverlapSphereAll, layerMask);
            
            for(int i = 0; i < colliderOverlapSphereAll.Length; ++i)
            {
                //if(colliderOverlapSphereAll[i] != null && colliderOverlapSphereAll[i].gameObject.activeSelf)
                //{
                //    colliderOverlapSphereAll[i].gameObject.SetActive(false);
                //}
                //else if(colliderOverlapSphereAll[i] != null)
                //{
                //    colliderOverlapSphereAll[i].gameObject.SetActive(true);
                //}
                if(colliderOverlapSphereAll[i] != null)
                {
                    Shine(colliderOverlapSphereAll[i].gameObject);
                }
            }
        }
    }

    #region ChangeColor
    private void ChangeColor()
    {
        if(material.color != color)
            material.color = color;
    }
    #endregion

    #region Shine
    private void Shine(GameObject hitObject)
    {
        material.EnableKeyword("_EMISSION");

        if (previousObject != hitObject)
        {
            StartCoroutine(ShineEffect(previousObject.GetComponent<Renderer>().material, shineIntensity, startIntensity));
        }

        StartCoroutine(ShineEffect(hitObject.GetComponent<Renderer>().material, startIntensity, shineIntensity));
    }

    IEnumerator ShineEffect(Material thisMaterial, float start, float end)
    {
        elapsedTime = 0f;
        while (elapsedTime < shineDuration)
        {
            t = elapsedTime / shineDuration;
            smoothT = Mathf.SmoothStep(0, 1, t);
            intensity = Mathf.Lerp(start, end, smoothT);
            thisMaterial.SetColor("_EmissionColor", thisMaterial.color * intensity);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        thisMaterial.SetColor("_EmissionColor", thisMaterial.color * end);
    }
    #endregion

    #region ScreenRay
    private void FireScreenRay()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(cameraRay, out RaycastHit hitObject))
        {
            objectLabel.text = $"Clicked object's name: {hitObject.collider.gameObject.name}";
        }
    }
    #endregion

    #region NonAlloc Clear
    private void ClearHits()
    {
        System.Array.Clear(hitsNonAlloc, 0, hitsNonAlloc.Length);
    }
    #endregion

    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying && transform.position != null)
        {
            // Draw the "forward" of the object (the part that points upwards on the cone)
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.forward * 10f);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(startPoint, endPoint);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, sphereRadius);

            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position, boxSize / 2);

            Gizmos.color = Color.purple;
            Gizmos.DrawWireSphere(transform.position, sphereRadius);
        }
    }
}
