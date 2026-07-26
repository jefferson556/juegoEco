using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private static readonly int SpeedParameter =
        Animator.StringToHash("Speed");

    private static readonly int IsGroundedParameter =
        Animator.StringToHash("IsGrounded");

    private static readonly int VerticalVelocityParameter =
        Animator.StringToHash("VerticalVelocity");

    [Header("Referencias")]
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    public void SetMovement(float horizontalMovement)
    {
        float speed = Mathf.Abs(horizontalMovement);

        animator?.SetFloat(
            SpeedParameter,
            speed
        );

        if (
            spriteRenderer != null &&
            Mathf.Abs(horizontalMovement) > 0.01f
        )
        {
            spriteRenderer.flipX =
                horizontalMovement < 0f;
        }
    }

    public void SetJumpState(
        bool isGrounded,
        float verticalVelocity
    )
    {
        if (animator == null)
        {
            return;
        }

        animator.SetBool(
            IsGroundedParameter,
            isGrounded
        );

        animator.SetFloat(
            VerticalVelocityParameter,
            verticalVelocity
        );
    }

    public void SetIdle()
    {
        animator?.SetFloat(
            SpeedParameter,
            0f
        );
    }
}