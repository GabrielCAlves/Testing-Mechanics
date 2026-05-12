using UnityEngine;

public class ReflectCube : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private Vector3 direction;
    [SerializeField] private float forceAmount = 10f;
    //[SerializeField] private float boostAmount = 30f;
    //[SerializeField] private float originalRigidbodyVelocity;
    //[SerializeField] private float bounciness = 1.5f;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        direction = Vector3.one.normalized; // Set the initial direction to (1, 1, 1) normalized
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            //ReflectVelocity(collision);

            direction = Vector3.Reflect(direction, collision.contacts[0].normal);
            direction.y = 0; // To keep the cube moving only in the horizontal plane
            rigidbody.linearVelocity = direction * forceAmount;
        }
    }

    //private void FixedUpdate()
    //{
    //    rigidbody.linearVelocity = direction * forceAmount;
    //}

    //private void ReflectVelocity(Collision collision)
    //{
    //    Vector3 incomingVector = rigidbody.linearVelocity; //rigidbody.velocity; (Deprecated)
    //    originalRigidbodyVelocity = incomingVector.magnitude;
    //    Vector3 normalVector = collision.contacts[0].normal;
    //    Vector3 reflectedVector = Vector3.Reflect(incomingVector, normalVector);
    //    rigidbody.linearVelocity = reflectedVector.normalized * originalRigidbodyVelocity * bounciness;
    //}

    //public void Boost(float boostAmount)
    //{
    //    rigidbody.AddForce(rigidbody.linearVelocity.normalized * boostAmount, ForceMode.Impulse);
    //}
}
