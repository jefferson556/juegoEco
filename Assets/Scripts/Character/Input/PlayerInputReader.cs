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
        FindCharacterController();
    }

    private void Update()
    {
        if (!inputEnabled)
        {
            return;
        }

        FindCharacterController();

        if (characterController == null)
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
        FindCharacterController();

        inputEnabled = true;

        if (characterController == null)
        {
            return;
        }

        characterController.EnableControl();
    }

    public void DisableInput()
    {
        FindCharacterController();

        inputEnabled = false;

        if (characterController == null)
        {
            return;
        }

        characterController.Stop();
        characterController.DisableControl();
    }

    private void FindCharacterController()
    {
        if (characterController != null)
        {
            return;
        }

        characterController =
            GetComponent<CharacterController2D>();

        if (characterController == null)
        {
            Debug.LogError(
                "No se encontró CharacterController2D " +
                "en el mismo objeto.",
                this
            );
        }
    }
}