using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private static readonly int SpeedParameter =
        Animator.StringToHash("Speed");

    [Header("Referencias")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

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

        if (animator != null)
        {
            animator.SetFloat(SpeedParameter, speed);
        }

        if (
            spriteRenderer != null &&
            Mathf.Abs(horizontalMovement) > 0.01f
        )
        {
            spriteRenderer.flipX = horizontalMovement < 0f;
        }
    }

    public void SetIdle()
    {
        if (animator != null)
        {
            animator.SetFloat(SpeedParameter, 0f);
        }
    }
}