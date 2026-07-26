using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioMenuController : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = true;
        audioSource.loop = true;
        audioSource.mute = false;
        audioSource.spatialBlend = 0f;
    }

    private void Start()
    {
        AudioListener.pause = false;
        AudioListener.volume = 1f;

        if (audioSource.clip == null)
        {
            Debug.LogError(
                "Audio_menu no tiene AudioClip asignado.",
                this
            );

            return;
        }

        if (!audioSource.isPlaying)
            audioSource.Play();
    }
}
