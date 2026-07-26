using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterJump : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField]
    private CharacterStats stats;

    [Header("Detección del suelo")]
    [SerializeField]
    private Transform groundCheck;

    [SerializeField]
    [Min(0.01f)]
    private float groundCheckRadius = 0.15f;

    [SerializeField]
    private LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool jumpEnabled = true;

    public bool IsGrounded { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (stats == null)
        {
            Debug.LogError(
                "Falta asignar CharacterStats.",
                this
            );
        }

        if (groundCheck == null)
        {
            Debug.LogError(
                "Falta asignar GroundCheck.",
                this
            );
        }
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    public void TryJump()
    {
        if (
            !jumpEnabled ||
            !IsGrounded ||
            stats == null
        )
        {
            return;
        }

        Vector2 velocity = rb.linearVelocity;
        velocity.y = stats.JumpForce;
        rb.linearVelocity = velocity;
    }

    public void SetJumpEnabled(bool enabled)
    {
        jumpEnabled = enabled;
    }

    private void CheckGround()
    {
        if (groundCheck == null)
        {
            IsGrounded = false;
            return;
        }

        IsGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius
        );
    }
}