using System.Collections;
using UnityEngine;

public class CharacterSwitchController : MonoBehaviour
{
    private enum ControlledCharacter
    {
        MainCharacter,
        Shadow
    }

    [Header("Personaje principal")]
    [SerializeField]
    private GameObject mainCharacter;

    [SerializeField]
    private PlayerInputReader mainInput;

    [SerializeField]
    private CharacterAnimation mainAnimation;

    [SerializeField]
    private Transform mainInteractionPoint;

    [Header("Sombra")]
    [SerializeField]
    private GameObject shadowCharacter;

    [SerializeField]
    private PlayerInputReader shadowInput;

    [Header("Configuración del cambio")]
    [SerializeField]
    private KeyCode switchKey = KeyCode.Q;

    [SerializeField]
    [Min(1f)]
    private float shadowControlDuration = 30f;

    [SerializeField]
    private bool allowEarlyReturn = true;

    [Header("Consumo de vida")]
    [SerializeField]
    private CharacterHealth characterHealth;

    [SerializeField]
    [Min(0f)]
    private float lifeDrainPerSecond = 3.34f;

    [Header("Restricción de Invocar Sombra")]
    [SerializeField]
    [Range(0.01f, 0.5f)]
    private float minLifeNormalizedToSummon = 0.10f;

    [SerializeField]
    private TMPro.TMP_Text textoMensajeUI;

    [SerializeField]
    private string mensajeNoEco = "No puedo llamar a Eco, debo intentar salir solo.";

    [SerializeField]
    private float tiempoMostrarMensaje = 3.5f;

    [Header("Debug")]
    [SerializeField]
    private bool showDebugLogs = true;

    private ControlledCharacter controlledCharacter;
    private Coroutine shadowTimerCoroutine;
    private Coroutine mensajeCoroutine;

    public bool IsControllingShadow =>
        controlledCharacter == ControlledCharacter.Shadow;

    public float RemainingTime { get; private set; }

    private void Awake()
    {
        ValidateReferences();

        if (textoMensajeUI != null)
        {
            textoMensajeUI.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        InitializeCharacters();
    }

    private void Update()
    {
        if (IsControllingShadow)
        {
            DrainLife();
        }

        if (!Input.GetKeyDown(switchKey))
        {
            return;
        }

        if (IsControllingShadow)
        {
            if (allowEarlyReturn)
            {
                ReturnToMainCharacter();
            }

            return;
        }

        SwitchToShadow();
    }

    private void InitializeCharacters()
    {
        controlledCharacter =
            ControlledCharacter.MainCharacter;

        RemainingTime = 0f;

        if (mainCharacter != null)
        {
            mainCharacter.SetActive(true);
        }

        if (mainAnimation != null)
        {
            mainAnimation.SetCrouching(false);
            mainAnimation.SetIdle();
        }

        if (mainInput != null)
        {
            mainInput.EnableInput();
        }

        if (shadowInput != null)
        {
            shadowInput.DisableInputAndControl();
        }

        if (shadowCharacter != null)
        {
            shadowCharacter.SetActive(false);
        }

        Log("Sistema iniciado. Control: personaje principal.");
    }

    public void SwitchToShadow()
    {
        if (IsControllingShadow)
        {
            return;
        }

        if (characterHealth != null)
        {
            if (!characterHealth.HasLife)
            {
                Log("No se puede usar la sombra: no queda vida.");
                return;
            }

            if (characterHealth.NormalizedLife <= minLifeNormalizedToSummon)
            {
                MostrarMensajeRestriccion();
                Log("Invocación bloqueada: Vida menor o igual al 10%.");
                return;
            }
        }

        if (
            mainInput == null ||
            mainAnimation == null ||
            mainInteractionPoint == null ||
            shadowCharacter == null ||
            shadowInput == null
        )
        {
            Debug.LogError(
                "[CharacterSwitchController] " +
                "No se puede cambiar a la sombra: " +
                "faltan referencias.",
                this
            );

            return;
        }

        controlledCharacter =
            ControlledCharacter.Shadow;

        mainInput.DisableInputAndControl();
        mainAnimation.SetCrouching(true);

        SpawnShadow();

        shadowCharacter.SetActive(true);
        shadowInput.EnableInput();

        if (shadowTimerCoroutine != null)
        {
            StopCoroutine(shadowTimerCoroutine);
        }

        shadowTimerCoroutine =
            StartCoroutine(ShadowControlTimer());

        Log("Control cambiado a la sombra.");
    }

    public void ReturnToMainCharacter()
    {
        if (!IsControllingShadow)
        {
            return;
        }

        if (shadowTimerCoroutine != null)
        {
            StopCoroutine(shadowTimerCoroutine);
            shadowTimerCoroutine = null;
        }

        controlledCharacter =
            ControlledCharacter.MainCharacter;

        RemainingTime = 0f;

        if (shadowInput != null)
        {
            shadowInput.DisableInputAndControl();
        }

        if (shadowCharacter != null)
        {
            shadowCharacter.SetActive(false);
        }

        if (mainAnimation != null)
        {
            mainAnimation.SetCrouching(false);
            mainAnimation.SetIdle();
        }

        if (mainInput != null)
        {
            mainInput.EnableInput();
        }

        Log("Control devuelto al personaje principal.");
    }

    private void DrainLife()
    {
        if (characterHealth == null)
        {
            return;
        }

        characterHealth.ConsumeLife(
            lifeDrainPerSecond * Time.deltaTime
        );

        if (!characterHealth.HasLife)
        {
            ReturnToMainCharacter();
        }
    }

    private void SpawnShadow()
    {
        Transform shadowTransform =
            shadowCharacter.transform;

        shadowTransform.position =
            mainInteractionPoint.position;

        shadowTransform.rotation =
            Quaternion.identity;
    }

    private IEnumerator ShadowControlTimer()
    {
        RemainingTime = shadowControlDuration;

        while (RemainingTime > 0f)
        {
            RemainingTime -= Time.deltaTime;
            yield return null;
        }

        RemainingTime = 0f;
        shadowTimerCoroutine = null;

        ReturnToMainCharacter();
    }

    private void ValidateReferences()
    {
        if (mainCharacter == null)
        {
            Debug.LogError(
                "Falta asignar Main Character.",
                this
            );
        }

        if (mainInput == null)
        {
            Debug.LogError(
                "Falta asignar Main Input.",
                this
            );
        }

        if (mainAnimation == null)
        {
            Debug.LogError(
                "Falta asignar Main Animation.",
                this
            );
        }

        if (mainInteractionPoint == null)
        {
            Debug.LogError(
                "Falta asignar Main Interaction Point.",
                this
            );
        }

        if (shadowCharacter == null)
        {
            Debug.LogError(
                "Falta asignar Shadow Character.",
                this
            );
        }

        if (shadowInput == null)
        {
            Debug.LogError(
                "Falta asignar Shadow Input.",
                this
            );
        }

        if (characterHealth == null)
        {
            Debug.LogWarning(
                "No se asignó CharacterHealth. " +
                "La sombra funcionará sin consumir vida.",
                this
            );
        }
    }

    private void MostrarMensajeRestriccion()
    {
        Debug.Log($"[CharacterSwitchController] {mensajeNoEco}", this);

        if (textoMensajeUI != null)
        {
            if (mensajeCoroutine != null)
            {
                StopCoroutine(mensajeCoroutine);
            }

            mensajeCoroutine = StartCoroutine(MostrarMensajeCoroutine());
        }
    }

    private IEnumerator MostrarMensajeCoroutine()
    {
        textoMensajeUI.text = mensajeNoEco;
        textoMensajeUI.gameObject.SetActive(true);

        yield return new WaitForSeconds(tiempoMostrarMensaje);

        textoMensajeUI.gameObject.SetActive(false);
        mensajeCoroutine = null;
    }

    private void Log(string message)
    {
        if (!showDebugLogs)
        {
            return;
        }

        Debug.Log(
            $"[CharacterSwitchController] {message}",
            this
        );
    }
}
