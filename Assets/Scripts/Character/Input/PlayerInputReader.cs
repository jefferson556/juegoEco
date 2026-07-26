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

    private void Awake()
    {
        characterController =
            GetComponent<CharacterController2D>();

        Debug.Log(
            $"[PlayerInputReader] Awake. " +
            $"Controller encontrado: {characterController != null}",
            this
        );
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
           /* Debug.Log(
                $"[PlayerInputReader] " +
                $"Horizontal: {horizontal}, " +
                $"Running: {isRunning}",
                this
            );
           */
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
                    "[PlayerInputReader] Salto presionado.",
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

        Debug.Log(
            "[PlayerInputReader] Input habilitado.",
            this
        );
    }

    public void DisableInput()
    {
        inputEnabled = false;

        characterController?.Stop();

        Debug.Log(
            "[PlayerInputReader] Input deshabilitado.",
            this
        );
    }
}