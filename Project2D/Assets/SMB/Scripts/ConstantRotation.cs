using UnityEngine;

public class ConstantRotation2D : MonoBehaviour
{
    // Vitesse de rotation en degrés par seconde (autour de l'axe Z)
    public float rotationSpeed = 100f;

    void Update()
    {
        // Rotation constante autour de l'axe Z
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}