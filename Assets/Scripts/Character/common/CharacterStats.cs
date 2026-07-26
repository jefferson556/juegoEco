using UnityEngine;

[CreateAssetMenu(
    fileName = "CharacterStats",
    menuName = "Game/Characters/Character Stats"
)]
public class CharacterStats : ScriptableObject
{
    [Header("Movimiento")]
    [SerializeField]
    [Min(0f)]
    private float walkSpeed = 4f;

    [SerializeField]
    [Min(0f)]
    private float runSpeed = 7f;

    [Header("Salto futuro")]
    [SerializeField]
    [Min(0f)]
    private float jumpForce = 8f;

    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;
    public float JumpForce => jumpForce;
}