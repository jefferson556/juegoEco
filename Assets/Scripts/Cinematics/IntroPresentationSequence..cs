using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class IntroPresentationSequence : MonoBehaviour
{
    [Header("Personaje principal")]
    [SerializeField]
    private CharacterAutoMovement mainCharacter;

    [Header("Personaje secundario")]
    [SerializeField]
    private CharacterAutoMovement secondaryCharacter;

    [SerializeField]
    private GameObject secondaryCharacterObject;

    [Header("Puntos de la escena")]
    [SerializeField]
    private Transform conversationPoint;

    [SerializeField]
    private Transform exitPoint;

    [Header("Tiempos")]
    [SerializeField]
    [Min(0f)]
    private float delayAfterMainStops = 2f;

    [SerializeField]
    [Min(0f)]
    private float delayAfterEcoAppears = 1f;

    [SerializeField]
    [Min(0f)]
    private float delayAfterConversation = 1f;

    [Header("Diálogo")]
    [SerializeField]
    private DialogueTextController dialogueController;

    [SerializeField]
    [TextArea(2, 5)]
    private string[] dialogueLines =
    {
        "¿Quién eres...?",
        "Eso no importa todavía.",
        "Tenemos que salir de este lugar."
    };

    [Header("Eventos")]
    [SerializeField]
    private UnityEvent onMainCharacterStopped;

    [SerializeField]
    private UnityEvent onSecondaryAppeared;

    [SerializeField]
    private UnityEvent onConversationStarted;

    [SerializeField]
    private UnityEvent onConversationFinished;

    [SerializeField]
    private UnityEvent onPresentationFinished;

    private void Awake()
    {
        if (secondaryCharacterObject != null)
        {
            secondaryCharacterObject.SetActive(false);
        }
    }

    private IEnumerator Start()
    {
        yield return RunPresentation();
    }

    private IEnumerator RunPresentation()
    {
        if (!HasValidReferences())
        {
            yield break;
        }

        // 1. El protagonista entra caminando.
        yield return mainCharacter.MoveTo(
            conversationPoint
        );

        // 2. El protagonista se detiene.
        mainCharacter.StopFollowing();

        onMainCharacterStopped?.Invoke();

        // 3. Espera antes de que aparezca Eco.
        yield return new WaitForSeconds(
            delayAfterMainStops
        );

        // 4. Eco aparece.
        secondaryCharacterObject.SetActive(true);

        onSecondaryAppeared?.Invoke();

        // 5. Espera antes de comenzar el diálogo.
        yield return new WaitForSeconds(
            delayAfterEcoAppears
        );

        // 6. Comienza el diálogo.
        onConversationStarted?.Invoke();

        yield return dialogueController.ShowConversation(
            dialogueLines
        );

        onConversationFinished?.Invoke();

        // 7. Espera después de terminar el diálogo.
        yield return new WaitForSeconds(
            delayAfterConversation
        );

        dialogueController.HideDialogue();

        // 8. Eco comienza a seguir al protagonista.
        secondaryCharacter.StartFollowing(
            mainCharacter.transform
        );

        // 9. El protagonista camina hacia la salida.
        yield return mainCharacter.MoveTo(
            exitPoint
        );

        yield return new WaitForSeconds(1f);

        secondaryCharacter.StopFollowing();

        onPresentationFinished?.Invoke();
    }

    private bool HasValidReferences()
    {
        if (
            mainCharacter == null ||
            secondaryCharacter == null ||
            secondaryCharacterObject == null ||
            conversationPoint == null ||
            exitPoint == null ||
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