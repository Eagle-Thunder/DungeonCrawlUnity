using UnityEngine;

public class Arrow : MonoBehaviour
{
    const float speed = 1;
    Rigidbody arrowRb;

    void Start()
    {
        arrowRb = GetComponent<Rigidbody>();
        arrowRb.AddRelativeForce(Vector3.forward * speed, ForceMode.VelocityChange);
        Invoke("DestroyArrow", 1);
    }
    private void DestroyArrow() => Destroy(gameObject);
}
