using UnityEngine;

public class MoveTowardsCube : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 5f;

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }
}
