using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class PlayerInputReader : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField]
    private bool inputEnabled = true;

    private CharacterController2D characterController;

    private void Awake()
    {
        characterController =
            GetComponent<CharacterController2D>();

        if (characterController == null)
        {
            Debug.LogError(
                "No se encontró CharacterController2D.",
                this
            );
        }
    }

    private void Update()
    {
        if (
            !inputEnabled ||
            characterController == null
        )
        {
            return;
        }

        ReadMovement();
        ReadJump();
    }

    private void ReadMovement()
    {
        float horizontal =
            Input.GetAxisRaw("Horizontal");

        bool isRunning =
            Input.GetKey(KeyCode.LeftShift);

        characterController.Move(
            horizontal,
            isRunning
        );
    }

    private void ReadJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            characterController.Jump();
        }
    }

    public void EnableInput()
    {
        inputEnabled = true;

        characterController?.EnableControl();
    }

    public void DisableInput()
    {
        inputEnabled = false;

        // Solo detiene la orden manual actual.
        // No desactiva CharacterMovement porque
        // la cinemática debe seguir moviéndolo.
        characterController?.Stop();
    }
}