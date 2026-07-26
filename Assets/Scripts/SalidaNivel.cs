using UnityEngine;
using UnityEngine.SceneManagement;

public class SalidaNivel : MonoBehaviour
{
    [SerializeField] private string siguienteEscena = "Nivel4-jugable";

    private bool cargando;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (cargando)
            return;

        if (!other.CompareTag("Player"))
            return;

        cargando = true;

        Debug.Log(
            "Cargando la escena: " + siguienteEscena,
            this
        );

        SceneManager.LoadScene(siguienteEscena);
    }
}
