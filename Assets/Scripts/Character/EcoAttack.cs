using UnityEngine;

public class EcoAttack : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    [SerializeField]
    private Transform attackPoint;

    [SerializeField]
    [Min(0.1f)]
    private float attackRadius = 0.6f;

    [SerializeField]
    private LayerMask enemyLayer = ~0;

    [SerializeField]
    private string enemyTag = "Enemy";

    [Header("Referencias")]
    [SerializeField]
    private CharacterAnimation characterAnimation;

    [SerializeField]
    private CharacterMovement characterMovement;

    [SerializeField]
    private CharacterJump characterJump;

    [Header("Penalización por Eliminar Enemigo")]
    [SerializeField]
    private CharacterStats normalStats;

    [SerializeField]
    private CharacterStats penalizedStats;

    [SerializeField]
    [Min(1f)]
    private float penaltyDuration = 5f;

    [Header("Debug")]
    [SerializeField]
    private bool showDebugLogs = true;

    private Coroutine penaltyCoroutine;

    private void Awake()
    {
        if (characterAnimation == null)
        {
            characterAnimation = GetComponentInChildren<CharacterAnimation>();
        }

        if (characterMovement == null)
        {
            characterMovement = GetComponent<CharacterMovement>();
        }

        if (characterJump == null)
        {
            characterJump = GetComponent<CharacterJump>();
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PerformAttack();
        }
    }

    public void PerformAttack()
    {
        characterAnimation?.SetAttack();

        Transform origin = attackPoint != null ? attackPoint : transform;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            origin.position,
            attackRadius,
            enemyLayer
        );

        int hitCount = 0;

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (
                enemyCollider.CompareTag(enemyTag) ||
                enemyCollider.transform.root.CompareTag(enemyTag)
            )
            {
                EnemyHealth enemyHealth = enemyCollider.GetComponentInParent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    bool wasDefeated = enemyHealth.TakeHit(1);
                    hitCount++;

                    if (wasDefeated)
                    {
                        ApplyDefeatPenalty();
                    }
                }
            }
        }

        if (showDebugLogs)
        {
            Debug.Log(
                $"[EcoAttack] Ataque ejecutado. Enemigos alcanzados: {hitCount}",
                this
            );
        }
    }

    public void ApplyDefeatPenalty()
    {
        if (penalizedStats == null)
        {
            Debug.LogWarning(
                "[EcoAttack] ¡Enemigo eliminado! Pero no hay PenalizedStats asignado en el Inspector.",
                this
            );

            return;
        }

        if (characterMovement != null)
        {
            characterMovement.SetStats(penalizedStats);
        }

        if (characterJump != null)
        {
            characterJump.SetStats(penalizedStats);
        }

        if (showDebugLogs)
        {
            Debug.Log(
                $"[EcoAttack] ¡ENEMIGO ELIMINADO! Penalización aplicada por {penaltyDuration}s: " +
                $"Velocidad = {penalizedStats.WalkSpeed}, Salto = {penalizedStats.JumpForce}",
                this
            );
        }

        if (penaltyCoroutine != null)
        {
            StopCoroutine(penaltyCoroutine);
        }

        penaltyCoroutine = StartCoroutine(PenaltyTimerCoroutine());
    }

    private System.Collections.IEnumerator PenaltyTimerCoroutine()
    {
        yield return new WaitForSeconds(penaltyDuration);

        if (normalStats != null)
        {
            if (characterMovement != null)
            {
                characterMovement.SetStats(normalStats);
            }

            if (characterJump != null)
            {
                characterJump.SetStats(normalStats);
            }

            if (showDebugLogs)
            {
                Debug.Log(
                    $"[EcoAttack] Penalización finalizada ({penaltyDuration}s). Estadísticas restauradas.",
                    this
                );
            }
        }

        penaltyCoroutine = null;
    }

    private void OnDrawGizmosSelected()
    {
        Transform origin = attackPoint != null ? attackPoint : transform;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origin.position, attackRadius);
    }
}
