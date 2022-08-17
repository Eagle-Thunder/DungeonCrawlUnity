using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public float turn;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        turn += Input.GetAxis("Mouse Y");
        transform.localRotation = Quaternion.Euler(-turn, 0, 0);
    }
}
