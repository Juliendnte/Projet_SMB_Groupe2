using System.Collections;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")] [SerializeField]
    private int speed = 10;

    [SerializeField] private int jumpForce = 5;

    [Header("Ground Check Settings")] [SerializeField]
    private float groundCheckDistance = .1f;

    [SerializeField] private LayerMask groundLayer;

    [Header("Wall Jump Settings")] [SerializeField]
    private float wallCheckDistance = .1f;

    [SerializeField] private float wallJumpForceX = 15;
    [SerializeField] private float wallJumpForceY = 7;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask wallBreakLayer;

    [Header("Respawn Settings")] [SerializeField]
    private GameObject respawnPoint;

    [Header("Advanced Settings")] [SerializeField]
    private float coyoteTimeDuration = .1f;

    [SerializeField] private float jumpBufferTime = .1f;

    private IA_Player myInputAction;
    private InputAction moveAction;
    private InputAction jumpAction;
    private Rigidbody2D rb;

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

        return hit.collider != null;
    }

    private void PerformWallJump()
    {
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
    }

    private bool IsTouchingWallLeft()
    {
        Vector2 leftRayOrigin = (Vector2)transform.position + Vector2.left * 0.5f;
        RaycastHit2D hitLeft = Physics2D.Raycast(leftRayOrigin, Vector2.left, wallCheckDistance, wallLayer);
        if (hitLeft.collider == null)
        {
            hitLeft = Physics2D.Raycast(leftRayOrigin, Vector2.left, wallCheckDistance, wallBreakLayer);
        }

        return hitLeft.collider != null;
    }

    private bool IsTouchingWallRight()
    {
        Vector2 rightRayOrigin = (Vector2)transform.position + Vector2.right * 0.5f;
        RaycastHit2D hitRight = Physics2D.Raycast(rightRayOrigin, Vector2.right, wallCheckDistance, wallLayer);
        if (hitRight.collider == null)
        {
            hitRight = Physics2D.Raycast(rightRayOrigin, Vector2.right, wallCheckDistance, wallBreakLayer);
        }


        return hitRight.collider != null;
    }

    private bool IsTouchingWall()
    {
        return IsTouchingWallLeft() || IsTouchingWallRight();
    }

    // Fonction de respawn
    private void Respawn()
    {
        transform.position = respawnPoint.transform.position;
    }

    private void FinishLevel()
    {
        Debug.Log("Niveau terminé !");
        StartCoroutine(FinishLevelWithDelay()); // Démarrer une coroutine avec un délai
    }

    private IEnumerator FinishLevelWithDelay()
    {
        // Empêcher le joueur de bouger
        moveAction.Disable(); // Désactiver les mouvements
        rb.linearVelocity = Vector2.zero; // Réinitialiser la vélocité du joueur

        yield return new WaitForSeconds(2f); // Attendre 2 secondes

        // Recharger la scène
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    // Détection des collisions avec les obstacles
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Scie Circulaire"))
        {
            Respawn();
        }

        if (other.CompareTag("Finish"))
        {
            FinishLevel();
        }
    }
}