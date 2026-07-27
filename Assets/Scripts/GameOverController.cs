using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [Header("Referencias de Salud")]
    [SerializeField]
    private CharacterHealth characterHealth;

    [Header("UI de Game Over")]
    [SerializeField]
    private GameObject panelGameOver;

    [SerializeField]
    private Button botonReiniciar;

    [SerializeField]
    private Button botonMenuPrincipal;

    [SerializeField]
    private string sceneMenuPrincipal = "MainMenu";

    [Header("Debug")]
    [SerializeField]
    private bool showDebugLogs = true;

    private bool isGameOver;

    private void Awake()
    {
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (characterHealth != null)
        {
            characterHealth.LifeDepleted += TriggerGameOver;
        }
    }

    private void OnDisable()
    {
        if (characterHealth != null)
        {
            characterHealth.LifeDepleted -= TriggerGameOver;
        }
    }

    public void TriggerGameOver()
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;

        if (showDebugLogs)
        {
            Debug.Log(
                "[GameOverController] ¡GAME OVER! La vida del personaje llegó a 0.",
                this
            );
        }

        // Pausar físicas y tiempo del juego
        Time.timeScale = 0f;

        // Activar la pantalla UI de derrota
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(true);
        }

        // Configurar escuchadores de botones
        if (botonReiniciar != null)
        {
            botonReiniciar.onClick.RemoveAllListeners();
            botonReiniciar.onClick.AddListener(ReiniciarNivel);
        }

        if (botonMenuPrincipal != null)
        {
            botonMenuPrincipal.onClick.RemoveAllListeners();
            botonMenuPrincipal.onClick.AddListener(IrAlMenuPrincipal);
        }
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }

    public void IrAlMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneMenuPrincipal);
    }
}
