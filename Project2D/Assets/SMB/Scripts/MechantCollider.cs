using UnityEngine;

public class MechantCollider : MonoBehaviour
{

    public Transform respawnPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Vérifie si l'objet touché est le joueur
        if (other.CompareTag("Player"))
        {
            // Replace le joueur à la position spécifiée
            other.transform.position = respawnPoint.position;

            // Optionnel : Ajoutez un effet sonore ou visuel ici
            Debug.Log("Le joueur a touché l'obstacle et a été replacé !");
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
