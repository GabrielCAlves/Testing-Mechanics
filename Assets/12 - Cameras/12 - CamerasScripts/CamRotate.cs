using UnityEngine;

public class CamRotate : MonoBehaviour
{
    [SerializeField] private Vector2 turn;
    [SerializeField] private float sensitivity = .5f;
    [SerializeField] private bool allowYAxis = true;
    [SerializeField] private bool allowXAxis = true;

    // Update is called once per frame
    void Update()
    {
        turn.y += Input.GetAxis("Mouse Y") * sensitivity;
        turn.x += Input.GetAxis("Mouse X") * sensitivity;

        if(allowXAxis && allowYAxis)
        {
            transform.localRotation = Quaternion.Euler(-turn.y, turn.x, 0);
        }
        else if(allowYAxis)
        {
            transform.localRotation = Quaternion.Euler(0, turn.x, 0);
        }
        else if(allowXAxis)
        {
            transform.localRotation = Quaternion.Euler(-turn.y, 0, 0);
        }
    }
}
