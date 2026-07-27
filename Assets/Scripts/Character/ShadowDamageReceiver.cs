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

    [Header("Contacto con Personaje Principal")]
    [SerializeField]
    private bool touchPlayerGameOver = true;

    [SerializeField]
    private string playerTag = "Player";

    [SerializeField]
    [Min(0f)]
    private float spawnGracePeriod = 0.4f;

    [Header("Debug")]
    [SerializeField]
    private bool showDebugLogs = true;

    private float nextDamageTime;
    private float enableTime;

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

    private void OnEnable()
    {
        enableTime = Time.time;
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

        TryCheckPlayerTouch(collision.collider);
        TryReceiveDamage(collision.collider);
    }

    private void OnCollisionStay2D(
        Collision2D collision
    )
    {
        TryCheckPlayerTouch(collision.collider);
        TryReceiveDamage(collision.collider);
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

        TryCheckPlayerTouch(other);
        TryReceiveDamage(other);
    }

    private void OnTriggerStay2D(
        Collider2D other
    )
    {
        TryCheckPlayerTouch(other);
        TryReceiveDamage(other);
    }

    private void TryCheckPlayerTouch(Collider2D other)
    {
        if (!touchPlayerGameOver || other == null || characterHealth == null)
        {
            return;
        }

        if (Time.time < enableTime + spawnGracePeriod)
        {
            return;
        }

        bool isPlayer =
            other.CompareTag(playerTag) ||
            other.transform.root.CompareTag(playerTag);

        if (isPlayer)
        {
            if (showDebugLogs)
            {
                Debug.Log(
                    $"[ShadowDamageReceiver] ¡LA SOMBRA TOCÓ AL PERSONAJE PRINCIPAL! Game Over instantáneo.",
                    this
                );
            }

            characterHealth.ConsumeLife(characterHealth.CurrentLife);
        }
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