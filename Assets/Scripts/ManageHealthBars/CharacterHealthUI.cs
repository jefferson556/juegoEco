using UnityEngine;
using UnityEngine.UI;

public class CharacterHealthUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField]
    private CharacterHealth characterHealth;

    [SerializeField]
    private Image lifeFillImage;

    private void OnEnable()
    {
        if (characterHealth != null)
        {
            characterHealth.LifeChanged += UpdateLifeBar;
        }

        UpdateLifeBar();
    }

    private void OnDisable()
    {
        if (characterHealth != null)
        {
            characterHealth.LifeChanged -= UpdateLifeBar;
        }
    }

    private void UpdateLifeBar()
    {
        if (
            characterHealth == null ||
            lifeFillImage == null
        )
        {
            return;
        }

        lifeFillImage.fillAmount =
            characterHealth.NormalizedLife;
    }
}
