using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField]
    private CharacterStats stats;

    [Header("Referencias")]
    [SerializeField]
    private CharacterAnimation characterAnimation;

    [Header("Debug")]
    [SerializeField]
    private bool showDebugLogs = true;

    private Rigidbody2D rb;

    private float horizontalInput;
    private bool isRunning;
    private bool movementEnabled = true;

    private float previousInput = float.NaN;
    private bool previousRunning;
    private bool previousMovementEnabled;

    public bool MovementEnabled => movementEnabled;
    public float HorizontalInput => horizontalInput;

    [SerializeField]
    private CharacterJump characterJump;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (characterJump == null)
        {
            characterJump =
                GetComponent<CharacterJump>();
        }
        if (characterAnimation == null)
        {
            characterAnimation =
                GetComponentInChildren<CharacterAnimation>();
        }

        previousMovementEnabled = movementEnabled;

        Debug.Log(
            $"[CharacterMovement:{name}] Inicio | " +
            $"Rigidbody: {rb != null} | " +
            $"Stats: {stats != null} | " +
            $"Animation: {characterAnimation != null}",
            this
        );
    }

    private void Update()
    {
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    public void SetHorizontalMovement(
        float direction,
        bool run = false
    )
    {
        if (!movementEnabled)
        {
            horizontalInput = 0f;
            isRunning = false;

            LogMovementChange("Movimiento rechazado: está deshabilitado");
            return;
        }

        horizontalInput = Mathf.Clamp(
            direction,
            -1f,
            1f
        );

        isRunning = run;

        LogMovementChange("SetHorizontalMovement");
    }

    public void Stop()
    {
        horizontalInput = 0f;
        isRunning = false;

        StopImmediately();
        characterAnimation?.SetIdle();

        LogMovementChange("Stop");
    }

    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;

        if (
            showDebugLogs &&
            movementEnabled != previousMovementEnabled
        )
        {
            Debug.Log(
                $"[CharacterMovement:{name}] " +
                $"MovementEnabled = {movementEnabled}",
                this
            );

            previousMovementEnabled = movementEnabled;
        }

        if (!enabled)
        {
            Stop();
        }
    }

    private void ApplyMovement()
    {
        if (!movementEnabled)
        {
            StopImmediately();
            return;
        }

        if (stats == null)
        {
            Debug.LogError(
                $"[CharacterMovement:{name}] " +
                "Falta asignar CharacterStats.",
                this
            );

            StopImmediately();
            return;
        }

        float speed = isRunning
            ? stats.RunSpeed
            : stats.WalkSpeed;

        Vector2 velocity = rb.linearVelocity;
        velocity.x = horizontalInput * speed;

        rb.linearVelocity = velocity;
    }

    private void UpdateAnimation()
    {
        if (!movementEnabled)
        {
            characterAnimation?.SetIdle();
            return;
        }

        characterAnimation?.SetMovement(
            horizontalInput
        );
        if (
       characterAnimation != null &&
       characterJump != null
   )
        {
            characterAnimation.SetJumpState(
                characterJump.IsGrounded,
                rb.linearVelocity.y
            );
        }
    }

    private void StopImmediately()
    {
        Vector2 velocity = rb.linearVelocity;
        velocity.x = 0f;

        rb.linearVelocity = velocity;
    }

    private void LogMovementChange(string source)
    {
        if (!showDebugLogs)
        {
            return;
        }

        bool inputChanged =
            !Mathf.Approximately(
                horizontalInput,
                previousInput
            );

        bool runningChanged =
            isRunning != previousRunning;

        if (!inputChanged && !runningChanged)
        {
            return;
        }

        /*Debug.Log(
            $"[CharacterMovement:{name}] {source} | " +
            $"Input={horizontalInput} | " +
            $"Running={isRunning}",
            this
        );*/

        previousInput = horizontalInput;
        previousRunning = isRunning;
    }
}