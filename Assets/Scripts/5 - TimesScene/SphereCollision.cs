using UnityEngine;

public class SphereCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            ListOfRunners.instance.AddRunner(this.gameObject);
        }
    }
}
