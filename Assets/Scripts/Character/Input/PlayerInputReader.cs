using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class PlayerInputReader : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private bool inputEnabled = true;

    private CharacterController2D characterController;

    private void Awake()
    {
        characterController =
            GetComponent<CharacterController2D>();
    }

    private void Update()
    {
        if (!inputEnabled)
        {
            return;
        }

        float horizontal =
            Input.GetAxisRaw("Horizontal");

        bool isRunning =
            Input.GetKey(KeyCode.LeftShift);

        characterController.Move(
            horizontal,
            isRunning
        );
    }

    public void EnableInput()
    {
        inputEnabled = true;
    }

    public void DisableInput()
    {
        inputEnabled = false;
        characterController.Stop();
    }
}