using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError(
                "El nombre de la escena está vacío.",
                this
            );

            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                $"La escena '{sceneName}' no está agregada " +
                "al Build Profile.",
                this
            );

            return;
        }

        Debug.Log($"Cargando escena: {sceneName}");

        SceneManager.LoadScene(sceneName);
    }
}