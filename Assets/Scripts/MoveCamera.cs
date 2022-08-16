using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public float turn;
    void Update()
    {
        turn += Input.GetAxis("Mouse Y");
        transform.localRotation = Quaternion.Euler(-turn, 0, 0);
    }
}
