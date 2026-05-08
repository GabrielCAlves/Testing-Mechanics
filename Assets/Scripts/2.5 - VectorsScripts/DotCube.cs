using UnityEngine;
using System.Collections.Generic;

public class DotCube : MonoBehaviour
{
    [SerializeField] private List<GameObject> dotObjects;
    [SerializeField] private float dot2VectorDot;
    [SerializeField] private float dot3VectorDot;
    [SerializeField] private float dot4VectorDot;

    // Update is called once per frame
    void Update()
    {
        ShowDots();
    }

    private void ShowDots()
    {
        dot2VectorDot = Vector3.Dot(dotObjects[0].transform.position, transform.position);
        dot3VectorDot = Vector3.Dot(dotObjects[1].transform.position, transform.position);
        dot4VectorDot = Vector3.Dot(dotObjects[2].transform.position, transform.position);
    }
}
