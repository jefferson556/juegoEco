using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class IntroPresentationSequence : MonoBehaviour
{
    [Header("Personaje principal")]
    [SerializeField]
    private CharacterAutoMovement mainCharacter;

    [SerializeField]
    private PlayerInputReader mainCharacterInput;

    [Header("Eco")]
    [SerializeField]
    private GameObject ecoCharacterObject;

    [Header("Puntos de la escena")]
    [SerializeField]
    private Transform conversationPoint;

    [SerializeField]
    private Transform ecoTeleportPoint;

    [Header("Controlador del diálogo")]
    [SerializeField]
    private DialogueTextController dialogueController;

    [Header("Texto antes de que aparezca Eco")]
    [SerializeField]
    [TextArea(2, 5)]
    private string[] dialogueBeforeEcoAppears =
    {
        "¿Hay alguien ahí...?",
        "Siento que alguien me observa."
    };

    [Header("Texto después de que aparezca Eco")]
    [SerializeField]
    [TextArea(2, 5)]
    private string[] dialogueAfterEcoAppears =
    {
        "¿Quién eres...?",
        "Puedes llamarme Eco.",
        "Encuéntrame más adelante."
    };

    [Header("Tiempos")]
    [SerializeField]
    [Min(0f)]
    private float delayAfterMainStops = 1f;

    [SerializeField]
    [Min(0f)]
    private float delayAfterFirstDialogue = 1.5f;

    [SerializeField]
    [Min(0f)]
    private float delayAfterEcoAppears = 1f;

    [SerializeField]
    [Min(0f)]
    private float delayBeforeEcoTeleports = 1.5f;

    [SerializeField]
    [Min(0f)]
    private float delayBeforeEcoReappears = 0.5f;

    [Header("Eventos opcionales")]
    [SerializeField]
    private UnityEvent onMainCharacterStopped;

    [SerializeField]
    private UnityEvent onFirstDialogueStarted;

    [SerializeField]
    private UnityEvent onEcoAppeared;

    [SerializeField]
    private UnityEvent onSecondDialogueStarted;

    [SerializeField]
    private UnityEvent onEcoDisappeared;

    [SerializeField]
    private UnityEvent onEcoTeleported;

    [SerializeField]
    private UnityEvent onPlayerControlEnabled;

    private void Awake()
    {
        // El jugador no puede controlar al protagonista
        // durante la presentación.
        if (ecoCharacterObject != null)
        {
            ecoCharacterObject.SetActive(false);
        }
    }

    private IEnumerator Start()
    {
        yield return null;

        mainCharacterInput.DisableInput();

        yield return RunSequence();
    }

    private IEnumerator RunSequence()
    {
        if (!HasValidReferences())
        {
            yield break;
        }

        // 1. El protagonista camina automáticamente.
        yield return mainCharacter.MoveTo(
            conversationPoint
        );

        // 2. Se detiene frente al lugar donde aparecerá Eco.
        mainCharacter.StopMovement();
        onMainCharacterStopped?.Invoke();

        yield return new WaitForSeconds(
            delayAfterMainStops
        );

        // 3. Primer texto mientras Eco sigue oculto.
        onFirstDialogueStarted?.Invoke();

        yield return dialogueController.ShowConversation(
            dialogueBeforeEcoAppears
        );

        yield return new WaitForSeconds(
            delayAfterFirstDialogue
        );

        // 4. Eco aparece.
        ecoCharacterObject.SetActive(true);
        onEcoAppeared?.Invoke();

        yield return new WaitForSeconds(
            delayAfterEcoAppears
        );

        // 5. Segundo grupo de textos.
        onSecondDialogueStarted?.Invoke();

        yield return dialogueController.ShowConversation(
            dialogueAfterEcoAppears
        );

        yield return new WaitForSeconds(
            delayBeforeEcoTeleports
        );

        // Ocultamos el panel cuando termina la conversación.
        dialogueController.HideDialogue();

        // 6. Eco desaparece.
        ecoCharacterObject.SetActive(false);
        onEcoDisappeared?.Invoke();

        // 7. Se mueve al punto de teleportación mientras está oculto.
        ecoCharacterObject.transform.position =
            ecoTeleportPoint.position;

        yield return new WaitForSeconds(
            delayBeforeEcoReappears
        );

        // 8. Eco reaparece en el nuevo lugar.
        ecoCharacterObject.SetActive(true);
        onEcoTeleported?.Invoke();

        // 9. El jugador obtiene el control.
        mainCharacterInput.EnableInput();
        onPlayerControlEnabled?.Invoke();
    }

    private bool HasValidReferences()
    {
        if (
            mainCharacter == null ||
            mainCharacterInput == null ||
            ecoCharacterObject == null ||
            conversationPoint == null ||
            ecoTeleportPoint == null ||
            dialogueController == null
        )
        {
            Debug.LogError(
                "Faltan referencias en IntroPresentationSequence.",
                this
            );

            return false;
        }

        return true;
    }
}