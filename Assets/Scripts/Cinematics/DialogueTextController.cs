using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueTextController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField]
    private GameObject dialoguePanel;

    [SerializeField]
    private TextMeshProUGUI dialogueText;

    [Header("Configuración")]
    [SerializeField]
    [Min(0.001f)]
    private float typingSpeed = 0.04f;

    [SerializeField]
    [Min(0f)]
    private float delayBetweenLines = 1.5f;

    private bool isWriting;

    public bool IsWriting => isWriting;

    private void Awake()
    {
        HideDialogue();
    }

    public IEnumerator ShowConversation(string[] lines)
    {
        if (
            dialoguePanel == null ||
            dialogueText == null
        )
        {
            Debug.LogError(
                "Faltan referencias en DialogueTextController.",
                this
            );

            yield break;
        }

        if (lines == null || lines.Length == 0)
        {
            yield break;
        }

        isWriting = true;

        dialoguePanel.SetActive(true);
        dialogueText.text = string.Empty;

        foreach (string line in lines)
        {
            yield return WriteLine(line);

            yield return new WaitForSeconds(
                delayBetweenLines
            );
        }

        isWriting = false;
    }

    public void HideDialogue()
    {
        isWriting = false;

        if (dialogueText != null)
        {
            dialogueText.text = string.Empty;
        }

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    private IEnumerator WriteLine(string line)
    {
        dialogueText.text = string.Empty;

        if (string.IsNullOrEmpty(line))
        {
            yield break;
        }

        foreach (char character in line)
        {
            dialogueText.text += character;

            yield return new WaitForSeconds(
                typingSpeed
            );
        }
    }
}