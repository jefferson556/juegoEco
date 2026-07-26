using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private CharacterStats stats;

    [Header("Referencias")]
    [SerializeField]
    private CharacterAnimation characterAnimation;

    private Rigidbody2D rb;

    private float horizontalInput;
    private bool isRunning;
    private bool movementEnabled = true;

    public bool MovementEnabled => movementEnabled;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (characterAnimation == null)
        {
            characterAnimation =
                GetComponentInChildren<CharacterAnimation>();
        }
    }

    private void FixedUpdate()
    {
        if (!movementEnabled)
        {
            StopImmediately();
            return;
        }

        float speed = isRunning
            ? stats.RunSpeed
            : stats.WalkSpeed;

        Vector2 velocity = rb.linearVelocity;
        velocity.x = horizontalInput * speed;

        rb.linearVelocity = velocity;

        characterAnimation?.SetMovement(horizontalInput);
    }

    public void SetHorizontalMovement(
        float direction,
        bool run = false
    )
    {
        horizontalInput = Mathf.Clamp(
            direction,
            -1f,
            1f
        );

        isRunning = run;
    }

    public void Stop()
    {
        horizontalInput = 0f;
        isRunning = false;

        StopImmediately();

        characterAnimation?.SetIdle();
    }

    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;

        if (!enabled)
        {
            Stop();
        }
    }

    private void StopImmediately()
    {
        Vector2 velocity = rb.linearVelocity;
        velocity.x = 0f;

        rb.linearVelocity = velocity;
    }
}