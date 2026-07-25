using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class EntradaSuenoController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image transicionNegra;
    [SerializeField] private TextMeshProUGUI textoNarrativo;
    [SerializeField] private Button botonOmitir;

    [Header("Transition Settings")]
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float delayBetweenTexts = 3.0f;
    [SerializeField] private string nextSceneName = "Nivel1";

    private bool isTransitioning = false;
    private Coroutine introCoroutine;

    private string[] narrativeTexts = new string[]
    {
        "No sabía si estaba despierto...\no si aquel lugar llevaba años esperándome.",
        "Cuando abrió los ojos,\nel sueño ya había comenzado."
    };

    private void Start()
    {
        // Ensure starting state
        if (transicionNegra != null)
        {
            Color c = transicionNegra.color;
            c.a = 1.0f; // Start fully black
            transicionNegra.color = c;
            transicionNegra.gameObject.SetActive(true);
        }

        if (textoNarrativo != null)
        {
            textoNarrativo.text = "";
        }

        if (botonOmitir != null)
        {
            botonOmitir.onClick.AddListener(SkipIntro);
        }

        // Start the sequence
        introCoroutine = StartCoroutine(RunIntroSequence());
    }

    private void Update()
    {
        // Allow skipping using Space or Enter keys
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SkipIntro();
        }
    }

    private IEnumerator RunIntroSequence()
    {
        // 1. Fade from black to transparent
        yield return StartCoroutine(FadeScreen(1.0f, 0.0f));

        // 2. Display first text
        yield return StartCoroutine(TypeText(narrativeTexts[0]));
        yield return new WaitForSeconds(delayBetweenTexts);

        // 3. Display second text (clear first text first)
        if (textoNarrativo != null) textoNarrativo.text = "";
        yield return StartCoroutine(TypeText(narrativeTexts[1]));
        yield return new WaitForSeconds(delayBetweenTexts);

        // 4. Fade to black
        yield return StartCoroutine(FadeScreen(0.0f, 1.0f));

        // 5. Load next scene
        LoadNextScene();
    }

    private IEnumerator FadeScreen(float startAlpha, float endAlpha)
    {
        if (transicionNegra == null) yield break;

        transicionNegra.gameObject.SetActive(true);
        float elapsed = 0.0f;
        Color c = transicionNegra.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            transicionNegra.color = c;
            yield return null;
        }

        c.a = endAlpha;
        transicionNegra.color = c;

        if (endAlpha <= 0.0f)
        {
            transicionNegra.gameObject.SetActive(false);
        }
    }

    private IEnumerator TypeText(string text)
    {
        if (textoNarrativo == null) yield break;

        textoNarrativo.text = "";
        foreach (char letter in text.ToCharArray())
        {
            textoNarrativo.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void SkipIntro()
    {
        if (isTransitioning) return;

        Debug.Log("[EntradaSuenoController] Skipping intro...");
        if (introCoroutine != null)
        {
            StopCoroutine(introCoroutine);
        }

        StartCoroutine(SkipSequence());
    }

    private IEnumerator SkipSequence()
    {
        isTransitioning = true;

        // Fade quickly to black and load scene
        yield return StartCoroutine(FadeScreen(transicionNegra != null ? transicionNegra.color.a : 0.0f, 1.0f));
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        // Prevent loading scene multiple times
        if (isTransitioning && SceneManager.GetActiveScene().name == nextSceneName) return; 
        
        isTransitioning = true;
        Debug.Log($"[EntradaSuenoController] Loading scene: {nextSceneName}");
        SceneManager.LoadScene(nextSceneName);
    }
}
