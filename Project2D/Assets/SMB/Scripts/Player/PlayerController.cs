using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private int speed = 10;
    [SerializeField] private int jumpForce = 5;

    [Header("Ground Check Settings")]
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Respawn Settings")]
    [SerializeField] private Transform respawnPoint; // Ajout du point de respawn

    private IA_Player myInputAction;
    private InputAction moveAction;
    private InputAction jumpAction;
    private Rigidbody2D rb;
    private Collider2D playerCollider;

    void Awake()
    {
        myInputAction = new IA_Player();
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        moveAction = myInputAction.Movements.Move;
        moveAction.Enable();
        jumpAction = myInputAction.Movements.Jump;
        jumpAction.performed += OnJump;
        jumpAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
    }

    private void OnJump(InputAction.CallbackContext callbackContext)
    {
        if (IsGrounded())
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void FixedUpdate()
    {
        Vector2 moveDir = moveAction.ReadValue<Vector2>();
        Vector2 vel = rb.linearVelocity;
        vel.x = moveDir.x * speed;
        rb.linearVelocity = vel;
    }

    private bool IsGrounded()
    {
        Vector2 rayOrigin = (Vector2)transform.position + Vector2.down * 0.5f;
        Debug.DrawRay(rayOrigin, Vector2.down * groundCheckDistance, Color.red);
        return Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance, groundLayer);
    }

    // Fonction de respawn
    private void Respawn()
    {
        transform.position = respawnPoint.position;
        Debug.Log("Le joueur a touché un obstacle et a été replacé !");
    }

    // Détection des collisions avec les obstacles
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Scie Circulaire")) // Remplace "Obstacle" par le tag de tes objets dangereux
        {
            Respawn();
        }
    }
}
