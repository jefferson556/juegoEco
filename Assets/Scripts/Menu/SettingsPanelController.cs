using UnityEngine;

public class SettingsPanelController : MonoBehaviour
{
    [Header("Popup de configuración")]
    [SerializeField]
    private GameObject settingsPopup;

    private void Awake()
    {
        if (settingsPopup == null)
        {
            Debug.LogError(
                "Debes asignar PopupConfiguracion.",
                this
            );

            enabled = false;
            return;
        }

        settingsPopup.SetActive(false);
    }

    public void Open()
    {
        if (settingsPopup == null)
        {
            return;
        }

        settingsPopup.SetActive(true);
    }

    public void Close()
    {
        if (settingsPopup == null)
        {
            return;
        }

        settingsPopup.SetActive(false);
    }
}