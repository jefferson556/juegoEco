using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    private const string VolumePreferenceKey =
        "MasterVolume";

    [Header("Slider de volumen")]
    [SerializeField]
    private Slider volumeSlider;

    [Header("Configuración")]
    [SerializeField]
    [Range(0f, 1f)]
    private float defaultVolume = 1f;

    private void Awake()
    {
        if (volumeSlider == null)
        {
            Debug.LogError(
                "Debes asignar el Slider de volumen.",
                this
            );

            enabled = false;
            return;
        }

        ConfigureSlider();
        LoadSavedVolume();
    }

    private void ConfigureSlider()
    {
        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;
        volumeSlider.wholeNumbers = false;
    }

    private void LoadSavedVolume()
    {
        float savedVolume = PlayerPrefs.GetFloat(
            VolumePreferenceKey,
            defaultVolume
        );

        savedVolume = Mathf.Clamp01(savedVolume);

        AudioListener.volume = savedVolume;

        volumeSlider.SetValueWithoutNotify(
            savedVolume
        );
    }

    public void SetVolume(float value)
    {
        float normalizedVolume = Mathf.Clamp01(
            value
        );

        AudioListener.volume = normalizedVolume;

        PlayerPrefs.SetFloat(
            VolumePreferenceKey,
            normalizedVolume
        );

        PlayerPrefs.Save();
    }
}