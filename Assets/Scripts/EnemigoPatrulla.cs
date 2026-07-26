using UnityEngine;

public class EnemigoPatrulla : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private Transform puntoA;
    [SerializeField] private Transform puntoB;

    [Header("Movement Settings")]
    [SerializeField] private float velocidad = 2f;
    [SerializeField] private float distanciaCambio = 0.15f;

    private Rigidbody2D rb;
    private Transform puntoDestino;
    private float originalScaleX;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScaleX = transform.localScale.x;

        if (puntoA == null || puntoB == null)
        {
            Debug.LogError("[EnemigoPatrulla] Puntos de patrulla no asignados.");
            enabled = false;
            return;
        }

        // Start patrolling towards B (right)
        puntoDestino = puntoB;
        Flip(true);
    }

    private void FixedUpdate()
    {
        if (puntoDestino == null) return;

        // Calculate direction to destination
        float directionX = (puntoDestino.position.x - transform.position.x) > 0 ? 1f : -1f;

        // Move horizontally maintaining vertical velocity
        rb.linearVelocity = new Vector2(directionX * velocidad, rb.linearVelocity.y);

        // Check if close to destination (horizontally)
        float distance = Mathf.Abs(transform.position.x - puntoDestino.position.x);
        if (distance < distanciaCambio)
        {
            // Switch destination
            if (puntoDestino == puntoB)
            {
                puntoDestino = puntoA;
                Flip(false); // Face left
            }
            else
            {
                puntoDestino = puntoB;
                Flip(true); // Face right
            }
        }
    }

    private void Flip(bool movingRight)
    {
        Vector3 scale = transform.localScale;
        // Flip scale.x based on direction, keeping original absolute scale
        scale.x = Mathf.Abs(originalScaleX) * (movingRight ? 1f : -1f);
        transform.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        if (puntoA != null && puntoB != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(puntoA.position, puntoB.position);
            Gizmos.DrawSphere(puntoA.position, 0.15f);
            Gizmos.DrawSphere(puntoB.position, 0.15f);
        }
    }
}
