using System.Collections;
using TMPro;
using UnityEngine;
//using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EntradaSuenoController : MonoBehaviour
{
    [SerializeField]
    private SceneLoader sceneLoader;
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
        if (isTransitioning)
        {
            return;
        }

        bool skipPressed =
            Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter);

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
            SetTransitionAlpha(1f);
            transicionNegra.gameObject.SetActive(true);
        }

        if (textoNarrativo != null)
        {
            textoNarrativo.text = string.Empty;
        }
    }

    private bool ValidateReferences()
    {
        if (transicionNegra == null)
        {
            Debug.LogError(
                "Falta asignar TransicionNegra.",
                this
            );

            return false;
        }

        if (textoNarrativo == null)
        {
            Debug.LogError(
                "Falta asignar TextoNarrativo.",
                this
            );

            return false;
        }

        if (botonOmitir == null)
        {
            Debug.LogError(
                "Falta asignar BotonOmitir.",
                this
            );

            return false;
        }

        return true;
    }

    private IEnumerator RunIntroSequence()
    {
        yield return FadeScreen(1f, 0f, fadeDuration);

        foreach (string narrativeText in textosNarrativos)
        {
            textoNarrativo.text = string.Empty;

            yield return TypeText(narrativeText);
            yield return new WaitForSeconds(delayBetweenTexts);
        }

        isTransitioning = true;

        yield return FadeScreen(
            transicionNegra.color.a,
            1f,
            fadeDuration
        );

        LoadNextScene();
    }

    private IEnumerator TypeText(string text)
    {
        textoNarrativo.text = string.Empty;

        foreach (char character in text)
        {
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
        transicionNegra.gameObject.SetActive(true);

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
        if (textoNarrativo != null)
        {
            textoNarrativo.text = string.Empty;
        }

        float currentAlpha = transicionNegra.color.a;

        yield return FadeScreen(
            currentAlpha,
            1f,
            0.35f
        );

        LoadNextScene();
    }

    private void LoadNextScene()
    {
        if (sceneLoadStarted)
        {
            return;
        }

        sceneLoadStarted = true;

        if (sceneLoader == null)
        {
            Debug.LogError(
                "SceneLoader no está asignado.",
                this
            );

            sceneLoadStarted = false;
            return;
        }

        sceneLoader.LoadScene(nextSceneName);
    }

    private void SetTransitionAlpha(float alpha)
    {
        Color transitionColor = transicionNegra.color;
        transitionColor.a = Mathf.Clamp01(alpha);
        transicionNegra.color = transitionColor;
    }
}