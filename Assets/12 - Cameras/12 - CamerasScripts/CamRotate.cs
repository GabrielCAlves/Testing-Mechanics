using UnityEngine;

public class CamRotate : MonoBehaviour
{
    [SerializeField] private Vector2 turn;
    [SerializeField] private float sensitivity = .5f;

    // Update is called once per frame
    void Update()
    {
        turn.y += Input.GetAxis("Mouse Y") * sensitivity;
        turn.x += Input.GetAxis("Mouse X") * sensitivity;

        transform.localRotation = Quaternion.Euler(-turn.y, turn.x, 0);
    }
}
