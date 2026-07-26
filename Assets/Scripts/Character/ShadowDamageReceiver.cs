using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ShadowDamageReceiver : MonoBehaviour
{
    [Header("Vida compartida")]
    [SerializeField]
    private CharacterHealth characterHealth;

    [Header("Daño enemigo")]
    [SerializeField]
    [Min(0f)]
    private float damagePerHit = 15f;

    [SerializeField]
    [Min(0f)]
    private float hitCooldown = 0.75f;

    [SerializeField]
    private string enemyTag = "Enemy";

    [Header("Debug")]
    [SerializeField]
    private bool showDebugLogs;

    private float nextAllowedHitTime;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryReceiveDamage(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryReceiveDamage(other);
    }

    private void TryReceiveDamage(Collider2D other)
    {
        if (
            other == null ||
            characterHealth == null ||
            Time.time < nextAllowedHitTime
        )
        {
            return;
        }

        GameObject hitObject = other.gameObject;

        bool isEnemy =
            hitObject.CompareTag(enemyTag) ||
            hitObject.transform.root.CompareTag(enemyTag);

        if (!isEnemy)
        {
            return;
        }

        nextAllowedHitTime =
            Time.time + hitCooldown;

        characterHealth.ConsumeLife(
            damagePerHit
        );

        if (showDebugLogs)
        {
            Debug.Log(
                $"[ShadowDamageReceiver:{name}] " +
                $"Daño recibido: {damagePerHit}. " +
                $"Vida restante: {characterHealth.CurrentLife}.",
                this
            );
        }
    }

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
}
