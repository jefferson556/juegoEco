using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Escenas")]
    [SerializeField] private string firstLevelSceneName = "Level1";

    [Header("Panel de configuración")]
    [SerializeField] private GameObject settingsPopup;

    [Header("Opciones")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle fullscreenToggle;

    private const string VolumeKey = "MasterVolume";

    private void Awake()
    {
        LoadSettings();

        if (settingsPopup != null)
        {
            settingsPopup.SetActive(false);
        }
    }

    public void PlayGame()
    {
        if (string.IsNullOrWhiteSpace(firstLevelSceneName))
        {
            Debug.LogError(
                "No has configurado el nombre de la escena del nivel 1.",
                this
            );

            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(firstLevelSceneName))
        {
            Debug.LogError(
                $"La escena '{firstLevelSceneName}' no está agregada " +
                "a la lista de escenas del Build.",
                this
            );

            return;
        }

        SceneManager.LoadScene(firstLevelSceneName);
    }

    public void OpenSettings()
    {
        if (settingsPopup == null)
        {
            Debug.LogError(
                "No has asignado el popup de configuración.",
                this
            );

            return;
        }

        settingsPopup.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPopup != null)
        {
            settingsPopup.SetActive(false);
        }
    }

    public void SetVolume(float value)
    {
        value = Mathf.Clamp01(value);

        AudioListener.volume = value;

        PlayerPrefs.SetFloat(VolumeKey, value);
        PlayerPrefs.Save();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        // Permite probar el botón Salir dentro del editor.
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void LoadSettings()
    {
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);

        AudioListener.volume = savedVolume;

        if (volumeSlider != null)
        {
            volumeSlider.SetValueWithoutNotify(savedVolume);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.SetIsOnWithoutNotify(
                Screen.fullScreen
            );
        }
    }
}