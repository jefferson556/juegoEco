using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Configuración de Resistencia")]
    [SerializeField]
    [Min(1)]
    private int maxHits = 3;

    [SerializeField]
    private bool showDebugLogs = true;

    private int currentHits;

    public event System.Action OnEnemyDefeated;

    private void Awake()
    {
        currentHits = maxHits;
    }

    public bool TakeHit(int damage = 1)
    {
        if (currentHits <= 0)
        {
            return false;
        }

        currentHits -= damage;

        if (showDebugLogs)
        {
            Debug.Log(
                $"[EnemyHealth:{name}] Recibió golpe. " +
                $"Golpes restantes: {currentHits}/{maxHits}",
                this
            );
        }

        if (currentHits <= 0)
        {
            Defeat();
            return true;
        }

        return false;
    }

    private void Defeat()
    {
        OnEnemyDefeated?.Invoke();

        if (showDebugLogs)
        {
            Debug.Log(
                $"[EnemyHealth:{name}] Enemigo derrotado " +
                $"tras aguantar {maxHits} golpes.",
                this
            );
        }

        Destroy(gameObject);
    }
}
