using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateDrop : MonoBehaviour
{
    private float rotationSpeed = 0.7f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, rotationSpeed, 0);
    }
}
