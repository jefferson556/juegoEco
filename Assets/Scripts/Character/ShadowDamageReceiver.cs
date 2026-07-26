using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ShadowDamageReceiver : MonoBehaviour
{
    [Header("Vida compartida")]
    [SerializeField]
    private CharacterHealth characterHealth;

    [Header("Daño")]
    [SerializeField]
    [Min(0f)]
    private float damagePerHit = 10f;

    [SerializeField]
    [Min(0.05f)]
    private float damageInterval = 0.75f;

    [SerializeField]
    private string enemyTag = "Enemy";

    [Header("Debug")]
    [SerializeField]
    private bool showDebugLogs = true;

    private float nextDamageTime;

    private void Awake()
    {
        if (characterHealth == null)
        {
            Debug.LogError(
                "[ShadowDamageReceiver] " +
                "Falta asignar CharacterHealth.",
                this
            );
        }
    }

    private void OnCollisionEnter2D(
        Collision2D collision
    )
    {
        if (showDebugLogs)
        {
            Debug.Log(
                $"[Eco] CollisionEnter con " +
                $"{collision.gameObject.name}, " +
                $"tag {collision.gameObject.tag}",
                this
            );
        }

        TryReceiveDamage(
            collision.collider
        );
    }

    private void OnCollisionStay2D(
        Collision2D collision
    )
    {
        TryReceiveDamage(
            collision.collider
        );
    }

    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        if (showDebugLogs)
        {
            Debug.Log(
                $"[Eco] TriggerEnter con " +
                $"{other.name}, tag {other.tag}",
                this
            );
        }

        TryReceiveDamage(other);
    }

    private void OnTriggerStay2D(
        Collider2D other
    )
    {
        TryReceiveDamage(other);
    }

    private void TryReceiveDamage(
        Collider2D other
    )
    {
        if (
            other == null ||
            characterHealth == null
        )
        {
            return;
        }

        bool isEnemy =
            other.CompareTag(enemyTag) ||
            other.transform.root.CompareTag(enemyTag);

        if (!isEnemy)
        {
            return;
        }

        if (Time.time < nextDamageTime)
        {
            return;
        }

        nextDamageTime =
            Time.time + damageInterval;

        characterHealth.ConsumeLife(
            damagePerHit
        );

        if (showDebugLogs)
        {
            Debug.Log(
                $"[Eco] Recibió {damagePerHit} " +
                $"de daño. Vida restante: " +
                $"{characterHealth.CurrentLife}",
                this
            );
        }
    }
}