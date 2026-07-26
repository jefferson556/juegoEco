using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
public class CharacterController2D : MonoBehaviour
{
    private CharacterMovement movement;

    public CharacterMovement Movement => movement;

    private void Awake()
    {
        movement = GetComponent<CharacterMovement>();
    }

    public void Move(float direction, bool run = false)
    {
        movement.SetHorizontalMovement(direction, run);
    }

    public void Stop()
    {
        movement.Stop();
    }

    public void EnableControl()
    {
        movement.SetMovementEnabled(true);
    }

    public void DisableControl()
    {
        movement.SetMovementEnabled(false);
    }
}