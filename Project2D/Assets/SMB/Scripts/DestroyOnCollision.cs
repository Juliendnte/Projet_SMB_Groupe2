using UnityEngine;

public class DestroyBlock : MonoBehaviour
{
    // Permet de choisir le Layer via l'inspecteur
    [SerializeField] private LayerMask blockBreakLayerMask;

    void OnCollisionEnter2D(Collision2D collision)
    {
        // On vérifie que le layer de l'objet touché est dans le LayerMask
        if ((blockBreakLayerMask.value & (1 << collision.gameObject.layer)) > 0)
        {
            // Détruit uniquement l'objet touché
            Destroy(collision.gameObject);
        }
    }
}