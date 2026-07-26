using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemigoPatrullaNivel4 : MonoBehaviour
{
    [SerializeField] private Transform puntoA;
    [SerializeField] private Transform puntoB;
    [SerializeField] private float velocidad = 2f;

    private Rigidbody2D rb;
    private float direccion = 1f;
    private Vector3 escalaInicial;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        escalaInicial = transform.localScale;
    }

    private void Start()
    {
        if (puntoA == null || puntoB == null)
        {
            Debug.LogError(
                "Faltan los puntos de patrulla de Nivel 4.",
                this
            );

            enabled = false;
            return;
        }

        if (puntoA.position.x > puntoB.position.x)
        {
            Transform temporal = puntoA;
            puntoA = puntoB;
            puntoB = temporal;
        }

        float posicionInicialX = Mathf.Clamp(
            transform.position.x,
            puntoA.position.x,
            puntoB.position.x
        );

        transform.position = new Vector3(
            posicionInicialX,
            transform.position.y,
            transform.position.z
        );

        direccion =
            Mathf.Abs(transform.position.x - puntoB.position.x) <
            Mathf.Abs(transform.position.x - puntoA.position.x)
                ? -1f
                : 1f;

        ActualizarOrientacion();
    }

    private void FixedUpdate()
    {
        float posicionX = rb.position.x;

        if (direccion > 0f && posicionX >= puntoB.position.x)
        {
            rb.position = new Vector2(
                puntoB.position.x,
                rb.position.y
            );

            direccion = -1f;
            ActualizarOrientacion();
        }
        else if (
            direccion < 0f &&
            posicionX <= puntoA.position.x
        )
        {
            rb.position = new Vector2(
                puntoA.position.x,
                rb.position.y
            );

            direccion = 1f;
            ActualizarOrientacion();
        }

        rb.linearVelocity = new Vector2(
            direccion * velocidad,
            rb.linearVelocity.y
        );
    }

    private void ActualizarOrientacion()
    {
        float escalaX = Mathf.Abs(escalaInicial.x);

        transform.localScale = new Vector3(
            direccion > 0f ? escalaX : -escalaX,
            escalaInicial.y,
            escalaInicial.z
        );
    }

    private void OnDrawGizmos()
    {
        if (puntoA == null || puntoB == null)
            return;

        Gizmos.DrawLine(
            puntoA.position,
            puntoB.position
        );

        Gizmos.DrawWireSphere(
            puntoA.position,
            0.15f
        );

        Gizmos.DrawWireSphere(
            puntoB.position,
            0.15f
        );
    }
}
