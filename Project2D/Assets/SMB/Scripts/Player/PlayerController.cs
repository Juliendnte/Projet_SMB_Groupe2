using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;


public class PlayerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private int speed = 10;
    private IA_Player myInputAction;
    private InputAction moveAction;
    private InputAction jumpAction;
    private Rigidbody2D rb;
    [SerializeField] private float jumpForce = 5;

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

    private void OnJump(InputAction.CallbackContext callbackContext)
    {
        if (rb != null)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            Vector2 moveDir = moveAction.ReadValue<Vector2>();
            Vector2 vel = rb.linearVelocity;

            vel.x = moveDir.x * speed;

            rb.linearVelocity = vel;
        }
    }
}