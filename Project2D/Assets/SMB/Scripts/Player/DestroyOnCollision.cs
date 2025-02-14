using System.Collections; // Nécessaire pour les Coroutines
using UnityEngine;

public class DestroyBlock : MonoBehaviour
{
    // Permet de choisir le Layer via l'inspecteur
    [SerializeField] private LayerMask blockBreakLayerMask;

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision détectée avec : " + collision.gameObject.name);

        // Vérifie si le Layer de l'objet fait partie du LayerMask
        if ((blockBreakLayerMask.value & (1 << collision.gameObject.layer)) > 0)
        {
            // Lance une Coroutine pour détruire l'objet après 1 seconde
            StartCoroutine(DestroyAfterDelay(collision.gameObject));
        }
    }

    // Coroutine pour attendre avant de détruire l'objet
    private IEnumerator DestroyAfterDelay(GameObject gameObject)
    {
        yield return new WaitForSeconds(0.8f); // Attente de 1 seconde
        Destroy(gameObject); // Destruction de l'objet
    }
}