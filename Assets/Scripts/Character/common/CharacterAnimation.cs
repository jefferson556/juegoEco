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

    private static readonly int IsRunningParameter =
        Animator.StringToHash("IsRunning");

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

    public void SetMovement(float horizontalMovement, bool isRunning = false)
    {
        float speed = Mathf.Abs(horizontalMovement);

        SetFloatIfExists(SpeedParameter, speed);
        SetBoolIfExists(IsRunningParameter, isRunning);

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
        SetBoolIfExists(IsGroundedParameter, isGrounded);
        SetFloatIfExists(VerticalVelocityParameter, verticalVelocity);
    }

    public void SetCrouching(bool isCrouching)
    {
        SetBoolIfExists(IsCrouchingParameter, isCrouching);

        if (isCrouching)
        {
            SetFloatIfExists(SpeedParameter, 0f);
        }
    }

    public void SetAttack()
    {
        SetTriggerIfExists(AttackTriggerParameter);
    }

    public void SetIdle()
    {
        SetFloatIfExists(SpeedParameter, 0f);
    }

    private void SetBoolIfExists(int parameterHash, bool value)
    {
        if (animator != null && HasParameter(parameterHash))
        {
            animator.SetBool(parameterHash, value);
        }
    }

    private void SetFloatIfExists(int parameterHash, float value)
    {
        if (animator != null && HasParameter(parameterHash))
        {
            animator.SetFloat(parameterHash, value);
        }
    }

    private void SetTriggerIfExists(int parameterHash)
    {
        if (animator != null && HasParameter(parameterHash))
        {
            animator.SetTrigger(parameterHash);
        }
    }

    private bool HasParameter(int parameterHash)
    {
        if (animator == null) return false;
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.nameHash == parameterHash)
            {
                return true;
            }
        }
        return false;
    }
}