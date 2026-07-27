using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerDamageReceiver : MonoBehaviour
{
    [Header("Vida compartida")]
    [SerializeField]
    private CharacterHealth characterHealth;

    [Header("Configuración de Enemigo")]
    [SerializeField]
    private string enemyTag = "Enemy";

    [SerializeField]
    private bool instantGameOverOnEnemyTouch = true;

    [Header("Debug")]
    [SerializeField]
    private bool showDebugLogs = true;

    private void Awake()
    {
        if (characterHealth == null)
        {
            characterHealth = GetComponentInParent<CharacterHealth>();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryCheckEnemyTouch(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryCheckEnemyTouch(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryCheckEnemyTouch(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryCheckEnemyTouch(other);
    }

    private void TryCheckEnemyTouch(Collider2D other)
    {
        if (!instantGameOverOnEnemyTouch || other == null || characterHealth == null)
        {
            return;
        }

        bool isEnemy =
            other.CompareTag(enemyTag) ||
            other.transform.root.CompareTag(enemyTag);

        if (isEnemy)
        {
            if (showDebugLogs)
            {
                Debug.Log(
                    $"[PlayerDamageReceiver] ¡EL PERSONAJE PRINCIPAL CHOCÓ CON UN ENEMIGO ({other.name})! Game Over instantáneo.",
                    this
                );
            }

            characterHealth.ConsumeLife(characterHealth.CurrentLife);
        }
    }
}
