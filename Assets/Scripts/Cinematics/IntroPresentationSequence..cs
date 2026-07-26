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

    [Header("Puntos de la escena")]
    [SerializeField]
    private Transform conversationPoint;

    [SerializeField]
    private Transform exitPoint;

    [Header("Conversación temporal")]
    [SerializeField]
    [Min(0f)]
    private float conversationDuration = 4f;

    [Header("Eventos")]
    [SerializeField]
    private UnityEvent onConversationStarted;

    [SerializeField]
    private UnityEvent onConversationFinished;

    [SerializeField]
    private UnityEvent onPresentationFinished;

    private IEnumerator Start()
    {
        yield return RunPresentation();
    }

    private IEnumerator RunPresentation()
    {
        ValidateReferences();

        // 1. El protagonista entra caminando.
        yield return mainCharacter.MoveTo(
            conversationPoint
        );

        // 2. Ambos quedan quietos.
        mainCharacter.StopFollowing();
        secondaryCharacter.StopFollowing();

        // 3. Comienza la conversación.
        onConversationStarted?.Invoke();

        yield return new WaitForSeconds(
            conversationDuration
        );

        // 4. Termina la conversación.
        onConversationFinished?.Invoke();

        // 5. El secundario comienza a seguirlo.
        secondaryCharacter.StartFollowing(
            mainCharacter.transform
        );

        // 6. El principal camina hacia la salida.
        yield return mainCharacter.MoveTo(
            exitPoint
        );

        // Esperamos brevemente al secundario.
        yield return new WaitForSeconds(1f);

        secondaryCharacter.StopFollowing();

        // 7. Finaliza la presentación.
        onPresentationFinished?.Invoke();
    }

    private void ValidateReferences()
    {
        if (
            mainCharacter == null ||
            secondaryCharacter == null ||
            conversationPoint == null ||
            exitPoint == null
        )
        {
            Debug.LogError(
                "Faltan referencias en IntroPresentationSequence.",
                this
            );
        }
    }
}