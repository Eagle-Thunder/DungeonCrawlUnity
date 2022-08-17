using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 1;
    private Rigidbody arrowRb;
    // Start is called before the first frame update
    void Start()
    {
        arrowRb = GetComponent<Rigidbody>();
        arrowRb.AddRelativeForce(Vector3.forward * speed, ForceMode.VelocityChange);
        Invoke("DestroyArrow", 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void DestroyArrow()
    {
        Destroy(gameObject);
    }
}
