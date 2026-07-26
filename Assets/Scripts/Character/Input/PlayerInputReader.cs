using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class PlayerInputReader : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField]
    private bool inputEnabled = true;

    [SerializeField]
    private bool showDebugLogs = true;

    private CharacterController2D characterController;

    private float lastHorizontal = float.NaN;
    private bool lastRunning;

    public bool InputEnabled => inputEnabled;

    private void Awake()
    {
        characterController =
            GetComponent<CharacterController2D>();

        if (characterController == null)
        {
            Debug.LogError(
                "[PlayerInputReader] No se encontró " +
                "CharacterController2D.",
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

        float horizontal =
            Input.GetAxisRaw("Horizontal");

        bool isRunning =
            Input.GetKey(KeyCode.LeftShift);

        if (
            showDebugLogs &&
            (
                !Mathf.Approximately(
                    horizontal,
                    lastHorizontal
                ) ||
                isRunning != lastRunning
            )
        )
        {
            Debug.Log(
                $"[PlayerInputReader:{name}] " +
                $"Horizontal={horizontal}, " +
                $"Running={isRunning}",
                this
            );

            lastHorizontal = horizontal;
            lastRunning = isRunning;
        }

        characterController.Move(
            horizontal,
            isRunning
        );

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (showDebugLogs)
            {
                Debug.Log(
                    $"[PlayerInputReader:{name}] " +
                    "Salto presionado.",
                    this
                );
            }

            characterController.Jump();
        }
    }

    public void EnableInput()
    {
        inputEnabled = true;

        characterController?.EnableControl();

        if (showDebugLogs)
        {
            Debug.Log(
                $"[PlayerInputReader:{name}] " +
                "Input habilitado.",
                this
            );
        }
    }

    // Para cinemáticas:
    // bloquea el teclado, pero permite movimiento automático.
    public void DisableInput()
    {
        inputEnabled = false;

        characterController?.Stop();

        if (showDebugLogs)
        {
            Debug.Log(
                $"[PlayerInputReader:{name}] " +
                "Input deshabilitado, " +
                "movimiento automático permitido.",
                this
            );
        }
    }

    // Para el cambio de personaje:
    // bloquea tanto el teclado como el movimiento.
    public void DisableInputAndControl()
    {
        inputEnabled = false;

        characterController?.Stop();
        characterController?.DisableControl();

        if (showDebugLogs)
        {
            Debug.Log(
                $"[PlayerInputReader:{name}] " +
                "Input y control deshabilitados.",
                this
            );
        }
    }
}