using UnityEngine;

public class TurnMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float angle;
    [SerializeField] private float horizontalX;
    [SerializeField] private float horizontalZ;
    [SerializeField] private Vector3 movement;

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        horizontalX = Input.GetAxis("Horizontal");
        horizontalZ = Input.GetAxis("Vertical");

        movement = new Vector3(0, 0, horizontalZ);

        transform.Translate(movement * speed * Time.deltaTime);

        Rotate();
    }

    private void Rotate()
    {
        angle = horizontalX * speed * Time.deltaTime;
        transform.Rotate(angle * Vector3.up);
    }
}
