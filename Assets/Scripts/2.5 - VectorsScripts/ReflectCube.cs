using UnityEngine;

public class ReflectCube : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private float boostAmount = 30f;
    [SerializeField] private float originalRigidbodyVelocity;
    [SerializeField] private float bounciness = 1.5f;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Wall"))
        {
            ReflectVelocity(collision);
        }
    }

    private void ReflectVelocity(Collision collision)
    {
        Vector3 incomingVector = rigidbody.linearVelocity; //rigidbody.velocity; (Deprecated)
        originalRigidbodyVelocity = incomingVector.magnitude;
        Vector3 normalVector = collision.contacts[0].normal;
        Vector3 reflectedVector = Vector3.Reflect(incomingVector, normalVector);
        rigidbody.linearVelocity = reflectedVector.normalized * originalRigidbodyVelocity * bounciness;
    }

    public void Boost(float boostAmount)
    {
        rigidbody.AddForce(rigidbody.linearVelocity.normalized * boostAmount, ForceMode.Impulse);
    }
}
