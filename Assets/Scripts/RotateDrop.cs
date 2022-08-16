using UnityEngine;

public class RotateDrop : MonoBehaviour
{
    private const float rotationSpeed = 0.7f;
    void Update() => transform.Rotate(0, rotationSpeed, 0);
}
