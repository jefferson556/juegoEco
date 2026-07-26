using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("Escenas")]
    [SerializeField]
    private string introSceneName = "EntradaSueno";

    [Header("Dependencias")]
    [SerializeField]
    private SceneLoader sceneLoader;

    [SerializeField]
    private SettingsPanelController settingsPanelController;

    public void PlayGame()
    {
        if (sceneLoader == null)
        {
            Debug.LogError(
                "SceneLoader no está asignado.",
                this
            );

            return;
        }

        sceneLoader.LoadScene(introSceneName);
    }

    public void OpenSettings()
    {
        if (settingsPanelController == null)
        {
            Debug.LogError(
                "SettingsPanelController no está asignado.",
                this
            );

            return;
        }

        settingsPanelController.Open();
    }

    public void QuitGame()
    {
        PlayerPrefs.Save();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}