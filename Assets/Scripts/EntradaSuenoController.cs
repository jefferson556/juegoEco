using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EntradaSuenoController : MonoBehaviour
{
    [Header("Dependencias")]
    [SerializeField] private SceneLoader sceneLoader;

    [Header("Referencias UI")]
    [SerializeField] private Image transicionNegra;
    [SerializeField] private TextMeshProUGUI textoNarrativo;
    [SerializeField] private Button botonOmitir;

    [Header("Narración")]
    [SerializeField]
    [TextArea(2, 5)]
    private string[] textosNarrativos =
    {
        "No sabía si estaba despierto...\n" +
        "o si aquel lugar llevaba años esperándome.",

        "Cuando abrió los ojos,\n" +
        "el sueño ya había comenzado."
    };

    [Header("Duraciones")]
    [SerializeField]
    [Min(0.01f)]
    private float fadeDuration = 1.5f;

    [SerializeField]
    [Min(0.001f)]
    private float typingSpeed = 0.05f;

    [SerializeField]
    [Min(0f)]
    private float delayBetweenTexts = 3f;

    [SerializeField]
    [Min(0.01f)]
    private float skipFadeDuration = 0.35f;

    [Header("Escena siguiente")]
    [SerializeField] private string nextSceneName = "Nivel1";

    private Coroutine introCoroutine;
    private bool isTransitioning;
    private bool sceneLoadStarted;

    private void Awake()
    {
        PrepareInitialState();
    }

    private void Start()
    {
        if (!ValidateReferences())
        {
            enabled = false;
            return;
        }

        botonOmitir.onClick.AddListener(SkipIntro);
        introCoroutine = StartCoroutine(RunIntroSequence());
    }

    private void Update()
    {
        if (isTransitioning || sceneLoadStarted)
        {
            return;
        }

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        bool skipPressed =
            keyboard.spaceKey.wasPressedThisFrame ||
            keyboard.enterKey.wasPressedThisFrame ||
            keyboard.numpadEnterKey.wasPressedThisFrame;

        if (skipPressed)
        {
            SkipIntro();
        }
    }

    private void OnDestroy()
    {
        if (botonOmitir != null)
        {
            botonOmitir.onClick.RemoveListener(SkipIntro);
        }
    }

    private void PrepareInitialState()
    {
        if (transicionNegra != null)
        {
            transicionNegra.gameObject.SetActive(true);
            SetTransitionAlpha(1f);
        }

        if (textoNarrativo != null)
        {
            textoNarrativo.text = string.Empty;
        }
    }

    private bool ValidateReferences()
    {
        bool isValid = true;

        if (sceneLoader == null)
        {
            Debug.LogError(
                "Falta asignar SceneLoader.",
                this
            );

            isValid = false;
        }

        if (transicionNegra == null)
        {
            Debug.LogError(
                "Falta asignar TransicionNegra.",
                this
            );

            isValid = false;
        }

        if (textoNarrativo == null)
        {
            Debug.LogError(
                "Falta asignar TextoNarrativo.",
                this
            );

            isValid = false;
        }

        if (botonOmitir == null)
        {
            Debug.LogError(
                "Falta asignar BotonOmitir.",
                this
            );

            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(nextSceneName))
        {
            Debug.LogError(
                "El nombre de la siguiente escena está vacío.",
                this
            );

            isValid = false;
        }

        return isValid;
    }

    private IEnumerator RunIntroSequence()
    {
        yield return FadeScreen(
            startAlpha: 1f,
            endAlpha: 0f,
            duration: fadeDuration
        );

        foreach (string narrativeText in textosNarrativos)
        {
            if (string.IsNullOrWhiteSpace(narrativeText))
            {
                continue;
            }

            textoNarrativo.text = string.Empty;

            yield return TypeText(narrativeText);
            yield return new WaitForSeconds(delayBetweenTexts);
        }

        isTransitioning = true;
        textoNarrativo.text = string.Empty;

        yield return FadeScreen(
            startAlpha: transicionNegra.color.a,
            endAlpha: 1f,
            duration: fadeDuration
        );

        LoadNextScene();
    }

    private IEnumerator TypeText(string text)
    {
        textoNarrativo.text = string.Empty;

        foreach (char character in text)
        {
            if (isTransitioning)
            {
                yield break;
            }

            textoNarrativo.text += character;

            yield return new WaitForSeconds(
                typingSpeed
            );
        }
    }

    private IEnumerator FadeScreen(
        float startAlpha,
        float endAlpha,
        float duration
    )
    {
        if (transicionNegra == null)
        {
            yield break;
        }

        transicionNegra.gameObject.SetActive(true);

        if (duration <= 0f)
        {
            SetTransitionAlpha(endAlpha);
        }
        else
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                float progress = Mathf.Clamp01(
                    elapsedTime / duration
                );

                float alpha = Mathf.Lerp(
                    startAlpha,
                    endAlpha,
                    progress
                );

                SetTransitionAlpha(alpha);

                yield return null;
            }

            SetTransitionAlpha(endAlpha);
        }

        if (endAlpha <= 0f)
        {
            transicionNegra.gameObject.SetActive(false);
        }
    }

    public void SkipIntro()
    {
        if (isTransitioning || sceneLoadStarted)
        {
            return;
        }

        isTransitioning = true;

        if (introCoroutine != null)
        {
            StopCoroutine(introCoroutine);
            introCoroutine = null;
        }

        StartCoroutine(SkipSequence());
    }

    private IEnumerator SkipSequence()
    {
        textoNarrativo.text = string.Empty;

        float currentAlpha = transicionNegra.color.a;

        yield return FadeScreen(
            startAlpha: currentAlpha,
            endAlpha: 1f,
            duration: skipFadeDuration
        );

        LoadNextScene();
    }

    private void LoadNextScene()
    {
        if (sceneLoadStarted)
        {
            return;
        }

        if (sceneLoader == null)
        {
            Debug.LogError(
                "SceneLoader no está asignado.",
                this
            );

            return;
        }

        sceneLoadStarted = true;
        sceneLoader.LoadScene(nextSceneName);
    }

    private void SetTransitionAlpha(float alpha)
    {
        if (transicionNegra == null)
        {
            return;
        }

        Color transitionColor = transicionNegra.color;
        transitionColor.a = Mathf.Clamp01(alpha);
        transicionNegra.color = transitionColor;
    }
}