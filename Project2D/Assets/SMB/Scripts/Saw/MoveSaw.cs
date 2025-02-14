using System.Collections;
using UnityEngine;

public class MoveSaw : MonoBehaviour
{
    [Header("Mouvement de la scie")]
    [SerializeField] private Vector3 startPosition; // Position initiale (en haut)
    [SerializeField] private Vector3 endPosition;   // Position finale (en bas)
    [SerializeField] private float speed = 5f;      // Vitesse constante

    void Start()
    {
        // Initialiser la position de départ de la scie
        transform.position = startPosition;
    }

    void Update()
    {
        // Déplacer la scie
        Move();
    }

    private void Move()
    {
        // Déplacer vers la position cible (en bas)
        transform.position = Vector3.MoveTowards(transform.position, endPosition, speed * Time.deltaTime);

        // Si on atteint la position finale, réinitialiser la position en haut
        if (Vector3.Distance(transform.position, endPosition) < 0.8f)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        // Réinitialiser la position de la scie à la position de départ
        transform.position = startPosition;
    }
}