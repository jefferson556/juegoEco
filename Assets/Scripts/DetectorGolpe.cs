using UnityEngine;

public class DetectorGolpe : MonoBehaviour
{
    [SerializeField] private ParedDestructible pared;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            Debug.Log($"[DetectorGolpe] Trigger enter with PlayerAttack tag from object: {other.gameObject.name}");
            if (pared != null)
            {
                pared.RecibirGolpe(1);
            }
            else
            {
                Debug.LogWarning("[DetectorGolpe] ParedDestructible reference is not assigned!");
            }
        }
    }
}
