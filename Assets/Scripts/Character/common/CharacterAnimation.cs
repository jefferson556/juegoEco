using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private static readonly int SpeedParameter =
        Animator.StringToHash("Speed");

    private static readonly int IsGroundedParameter =
        Animator.StringToHash("IsGrounded");

    private static readonly int VerticalVelocityParameter =
        Animator.StringToHash("VerticalVelocity");

    private static readonly int IsCrouchingParameter =
        Animator.StringToHash("IsCr");

    private static readonly int AttackTriggerParameter =
        Animator.StringToHash("Attack");

    [Header("Referencias")]
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Transform interactionPoint;

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

        if (interactionPoint == null)
        {
            Transform foundPoint = transform.parent != null
                ? transform.parent.Find("InteractionPoint")
                : transform.Find("InteractionPoint");

            if (foundPoint != null)
            {
                interactionPoint = foundPoint;
            }
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
            bool facingLeft = horizontalMovement < 0f;
            spriteRenderer.flipX = facingLeft;

            if (interactionPoint != null)
            {
                Vector3 pos = interactionPoint.localPosition;
                pos.x = facingLeft
                    ? -Mathf.Abs(pos.x)
                    : Mathf.Abs(pos.x);

                interactionPoint.localPosition = pos;
            }
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

    public void SetCrouching(bool isCrouching)
    {
        if (animator == null)
        {
            return;
        }

        animator.SetBool(
            IsCrouchingParameter,
            isCrouching
        );

        if (isCrouching)
        {
            animator.SetFloat(
                SpeedParameter,
                0f
            );
        }
    }

    public void SetAttack()
    {
        animator?.SetTrigger(
            AttackTriggerParameter
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