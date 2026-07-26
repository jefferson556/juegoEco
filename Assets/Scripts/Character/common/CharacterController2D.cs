using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(CharacterJump))]
public class CharacterController2D : MonoBehaviour
{
    private CharacterMovement movement;
    private CharacterJump jump;

    private void Awake()
    {
        FindComponents();
    }

    public void Move(
        float direction,
        bool run = false
    )
    {
        FindComponents();

        if (movement == null)
        {
            return;
        }

        movement.SetHorizontalMovement(
            direction,
            run
        );
    }

    public void Jump()
    {
        FindComponents();

        if (jump == null)
        {
            return;
        }

        jump.TryJump();
    }

    public void Stop()
    {
        FindComponents();

        if (movement == null)
        {
            return;
        }

        movement.Stop();
    }

    public void EnableControl()
    {
        FindComponents();

        if (movement != null)
        {
            movement.SetMovementEnabled(true);
        }

        if (jump != null)
        {
            jump.SetJumpEnabled(true);
        }
    }

    public void DisableControl()
    {
        FindComponents();

        if (movement != null)
        {
            movement.SetMovementEnabled(false);
        }

        if (jump != null)
        {
            jump.SetJumpEnabled(false);
        }
    }

    private void FindComponents()
    {
        if (movement == null)
        {
            movement =
                GetComponent<CharacterMovement>();
        }

        if (jump == null)
        {
            jump =
                GetComponent<CharacterJump>();
        }

        if (movement == null)
        {
            Debug.LogError(
                "No se encontró CharacterMovement.",
                this
            );
        }

        if (jump == null)
        {
            Debug.LogError(
                "No se encontró CharacterJump.",
                this
            );
        }
    }
}