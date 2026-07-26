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

    [Header("Debug")]
    [SerializeField]
    private bool showDebugLogs = true;

    private void Awake()
    {
        if (characterAnimation == null)
        {
            characterAnimation = GetComponentInChildren<CharacterAnimation>();
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
                    enemyHealth.TakeHit(1);
                    hitCount++;
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

    private void OnDrawGizmosSelected()
    {
        Transform origin = attackPoint != null ? attackPoint : transform;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origin.position, attackRadius);
    }
}
