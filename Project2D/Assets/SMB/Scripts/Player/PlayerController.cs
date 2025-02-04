using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")] [SerializeField]
    private int speed = 10;

    [SerializeField] private int jumpForce = 7;

    [Header("Ground Check Settings")] [SerializeField]
    private float groundCheckDistance = .1f;

    [SerializeField] private LayerMask groundLayer; // Assigné dans l'inspecteur (par exemple, layer "FIRST")

    [Header("Wall Jump Settings")] [SerializeField]
    private float wallCheckDistance = .1f;

    [SerializeField] private float wallJumpForceX = 15;
    [SerializeField] private float wallJumpForceY = 10;
    [SerializeField] private LayerMask wallLayer; // Assigné dans l'inspecteur (par exemple, layer "WALL")

    [Header("Respawn Settings")]
    [SerializeField] private GameObject respawnPoint; // Ajout du point de respawn
    [Header("Advanced Settings")] [SerializeField]
    private float coyoteTimeDuration = .1f; // Temps de grâce pour le saut

    [SerializeField] private float jumpBufferTime = .1f; // Temps de buffer pour les sauts

    private IA_Player myInputAction;
    private InputAction moveAction;
    private InputAction jumpAction;
    private Rigidbody2D rb;

    private bool isWallJumping;
    private bool isCoyoteTimeActive;
    private float coyoteTimeCounter;
    private bool jumpBuffered;
    private float jumpBufferCounter;

    void Awake()
    {
        myInputAction = new IA_Player();
        rb = GetComponent<Rigidbody2D>();
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

    void FixedUpdate()
    {
        Vector2 moveDir = moveAction.ReadValue<Vector2>();
        Vector2 vel = rb.linearVelocity;

        if (moveDir.x != 0)
        {
            vel.x = moveDir.x * (IsGrounded() ? speed : speed * 0.8f); // Contrôle réduit en l'air
        }
        else
        {
            vel.x = Mathf.Lerp(vel.x, 0, Time.fixedDeltaTime * 10); // Ralentissement rapide
        }

        rb.linearVelocity = vel;

        // Gestion du Coyote Time
        if (IsGrounded())
        {
            isCoyoteTimeActive = true;
            coyoteTimeCounter = coyoteTimeDuration;
        }
        else
        {
            coyoteTimeCounter -= Time.fixedDeltaTime;
            if (coyoteTimeCounter <= 0)
            {
                isCoyoteTimeActive = false;
            }
        }

        // Gestion du jump buffer
        if (jumpBuffered)
        {
            jumpBufferCounter -= Time.fixedDeltaTime;
            if (jumpBufferCounter <= 0)
            {
                jumpBuffered = false;
            }
        }
    }

    private void OnJump(InputAction.CallbackContext callbackContext)
    {
        // Enregistrer le saut dans le buffer
        jumpBuffered = true;
        jumpBufferCounter = jumpBufferTime;

        if (IsGrounded() || isCoyoteTimeActive)
        {
            jumpBuffered = false;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        else if (IsTouchingWall())
        {
            jumpBuffered = false;
            PerformWallJump();
        }
    }

    private bool IsGrounded()
    {
        Vector2 rayOrigin = (Vector2)transform.position + Vector2.down * (0.5f + groundCheckDistance);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance);
        Debug.DrawRay(rayOrigin, Vector2.down * groundCheckDistance, Color.red);

        return hit.collider != null;
    }

    private void PerformWallJump()
    {
        isWallJumping = true;

        // Déterminer la direction du saut
        Vector2 wallJumpDirection;

        if (IsTouchingWallLeft())
        {
            wallJumpDirection = new Vector2(wallJumpForceX, wallJumpForceY); // Diagonale droite
        }
        else if (IsTouchingWallRight())
        {
            wallJumpDirection = new Vector2(-wallJumpForceX, wallJumpForceY); // Diagonale gauche
        }
        else
        {
            return;
        }

        // Réinitialiser la vélocité pour un saut propre
        rb.linearVelocity = Vector2.zero;

        // Appliquer la force diagonale pour le saut mural
        rb.AddForce(wallJumpDirection, ForceMode2D.Impulse);

        // Désactiver temporairement les contrôles
        Invoke(nameof(ResetWallJump), 0.2f);
    }

    private bool IsTouchingWallLeft()
    {
        Vector2 leftRayOrigin = (Vector2)transform.position + Vector2.left * 0.5f;
        RaycastHit2D hitLeft = Physics2D.Raycast(leftRayOrigin, Vector2.left, wallCheckDistance, wallLayer);

        Debug.DrawRay(leftRayOrigin, Vector2.left * wallCheckDistance, Color.blue);

        return hitLeft.collider != null;
    }

    private bool IsTouchingWallRight()
    {
        Vector2 rightRayOrigin = (Vector2)transform.position + Vector2.right * 0.5f;
        RaycastHit2D hitRight = Physics2D.Raycast(rightRayOrigin, Vector2.right, wallCheckDistance, wallLayer);

        Debug.DrawRay(rightRayOrigin, Vector2.right * wallCheckDistance, Color.green);

        return hitRight.collider != null;
    }

    private bool IsTouchingWall()
    {
        return IsTouchingWallLeft() || IsTouchingWallRight();
    }

    private void ResetWallJump()
    {
        isWallJumping = false;
    }
        return Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance, groundLayer);
    }

    // Fonction de respawn
    private void Respawn()
    {
        transform.position = respawnPoint.transform.position;
        Debug.Log("Le joueur a touché un obstacle et a été replacé !");
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || rb == null) return;

        // Rayon pour vérifier le sol
        Gizmos.color = Color.red; // Rouge pour le sol
        Vector2 rayOrigin = (Vector2)transform.position + Vector2.down * (0.5f + groundCheckDistance);
        Gizmos.DrawLine(rayOrigin, rayOrigin + Vector2.down * groundCheckDistance);

        // Rayon pour vérifier le mur à gauche
        Gizmos.color = Color.blue; // Bleu pour le mur à gauche
        Vector2 leftRayOrigin = (Vector2)transform.position + Vector2.left * 0.5f;
        Gizmos.DrawLine(leftRayOrigin, leftRayOrigin + Vector2.left * wallCheckDistance);

        // Rayon pour vérifier le mur à droite
        Gizmos.color = Color.green; // Vert pour le mur à droite
        Vector2 rightRayOrigin = (Vector2)transform.position + Vector2.right * 0.5f;
        Gizmos.DrawLine(rightRayOrigin, rightRayOrigin + Vector2.right * wallCheckDistance);
    // Détection des collisions avec les obstacles
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Scie Circulaire")) // Remplace "Obstacle" par le tag de tes objets dangereux
        {
            Respawn();
        }
    }
}