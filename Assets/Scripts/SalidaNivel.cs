using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SalidaNivel : MonoBehaviour
{
    [Header("Configuración de Nivel / Carga")]
    [SerializeField] private bool esFinDeDemo = false;
    [SerializeField] private string siguienteEscena = "Nivel2-jugable";
    [SerializeField] private string playerTag = "Player";

    [Header("Fin de Demo UI (Opcional)")]
    [SerializeField] private GameObject panelFinDemo;
    [SerializeField] private TMPro.TMP_Text textoFinDemo;
    [SerializeField] private string mensajeFinDemo = "¡Gracias por jugar esta demo!";
    [SerializeField] private Button botonMenuPrincipal;
    [SerializeField] private string sceneMenuPrincipal = "MainMenu";

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private bool activado;

    private void Awake()
    {
        if (panelFinDemo != null)
        {
            panelFinDemo.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activado)
        {
            return;
        }

        bool isPlayer =
            other.CompareTag(playerTag) ||
            other.transform.root.CompareTag(playerTag);

        if (!isPlayer)
        {
            return;
        }

        activado = true;

        if (esFinDeDemo || (panelFinDemo != null && string.IsNullOrWhiteSpace(siguienteEscena)))
        {
            MostrarFinDeDemo();
        }
        else
        {
            CargarSiguienteEscena();
        }
    }

    private void MostrarFinDeDemo()
    {
        if (showDebugLogs)
        {
            Debug.Log($"[SalidaNivel] {mensajeFinDemo}", this);
        }

        Time.timeScale = 0f;

        if (textoFinDemo != null)
        {
            textoFinDemo.text = mensajeFinDemo;
        }

        if (panelFinDemo != null)
        {
            panelFinDemo.SetActive(true);
        }

        if (botonMenuPrincipal != null)
        {
            botonMenuPrincipal.onClick.RemoveAllListeners();
            botonMenuPrincipal.onClick.AddListener(IrAlMenuPrincipal);
        }
    }

    private void CargarSiguienteEscena()
    {
        if (string.IsNullOrWhiteSpace(siguienteEscena))
        {
            Debug.LogWarning("[SalidaNivel] El nombre de la siguiente escena está vacío.", this);
            return;
        }

        if (showDebugLogs)
        {
            Debug.Log($"[SalidaNivel] Cargando escena: {siguienteEscena}", this);
        }

        SceneManager.LoadScene(siguienteEscena);
    }

    public void IrAlMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneMenuPrincipal);
    }
}
