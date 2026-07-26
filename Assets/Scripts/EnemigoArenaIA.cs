using UnityEngine;

public interface IDamageable
{
    void RecibirDanio(float cantidad);
}

public class EnemigoArenaIA : MonoBehaviour
{
    public enum EstadoEnemigo
    {
        Patrulla,
        Persecucion,
        Ataque,
        Regreso,
        Muerto
    }

    [Header("Patrulla y Movimiento")]
    [SerializeField] private Transform puntoPatrullaA;
    [SerializeField] private Transform puntoPatrullaB;
    [SerializeField] private Transform puntoSuelo;
    [SerializeField] private Transform puntoBorde;
    [SerializeField] private LayerMask capaSuelo;
    [SerializeField] private float velocidadPatrulla = 1.5f;
    [SerializeField] private float velocidadPersecucion = 3.0f;
    [SerializeField] private float distanciaCambioPatrulla = 0.1f;

    [Header("Detección y Combate")]
    [SerializeField] private LayerMask capaJugador;
    [SerializeField] private Transform puntoAtaque;
    [SerializeField] private float distanciaDeteccion = 5.0f;
    [SerializeField] private float distanciaAtaque = 1.2f;
    [SerializeField] private float distanciaAbandono = 8.0f;
    [SerializeField] private float tiempoEntreAtaques = 1.2f;
    [SerializeField] private int dañoAtaque = 15;

    [Header("Referencias Componentes")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private EstadoEnemigo estadoActual = EstadoEnemigo.Patrulla;
    private Transform jugadorTransform;
    private float tiempoUltimoAtaque;
    private Transform puntoObjetivoPatrulla;
    private bool mirandoDerecha = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        // Set initial patrol target
        if (puntoPatrullaA != null && puntoPatrullaB != null)
        {
            // Start towards B (right side target generally)
            puntoObjetivoPatrulla = puntoPatrullaB;
        }
    }

    private void Start()
    {
        // Find player by tag
        GameObject jugadorGo = GameObject.FindWithTag("Player");
        if (jugadorGo != null)
        {
            jugadorTransform = jugadorGo.transform;
        }
    }

    private void Update()
    {
        if (estadoActual == EstadoEnemigo.Muerto) return;

        BuscarJugadorDinamico();
        ActualizarEstado();
        ActualizarAnimaciones();
    }

    private void FixedUpdate()
    {
        if (estadoActual == EstadoEnemigo.Muerto)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        switch (estadoActual)
        {
            case EstadoEnemigo.Patrulla:
                ComportamientoPatrulla();
                break;
            case EstadoEnemigo.Persecucion:
                ComportamientoPersecucion();
                break;
            case EstadoEnemigo.Ataque:
                ComportamientoAtaque();
                break;
            case EstadoEnemigo.Regreso:
                ComportamientoRegreso();
                break;
        }
    }

    private void BuscarJugadorDinamico()
    {
        if (jugadorTransform == null)
        {
            GameObject jugadorGo = GameObject.FindWithTag("Player");
            if (jugadorGo != null)
            {
                jugadorTransform = jugadorGo.transform;
            }
        }
    }

    private void ActualizarEstado()
    {
        if (jugadorTransform == null)
        {
            if (estadoActual == EstadoEnemigo.Persecucion || estadoActual == EstadoEnemigo.Ataque)
            {
                estadoActual = EstadoEnemigo.Regreso;
            }
            return;
        }

        float distanciaHorizontal = Mathf.Abs(jugadorTransform.position.x - transform.position.x);
        float distanciaVertical = Mathf.Abs(jugadorTransform.position.y - transform.position.y);
        float distanciaTotal = Vector2.Distance(transform.position, jugadorTransform.position);

        bool jugadorVisible = DetectarJugadorPorRaycast(distanciaTotal, distanciaVertical);

        if (estadoActual == EstadoEnemigo.Patrulla || estadoActual == EstadoEnemigo.Regreso)
        {
            if (jugadorVisible && distanciaHorizontal <= distanciaDeteccion)
            {
                estadoActual = EstadoEnemigo.Persecucion;
                Debug.Log("[EnemigoArenaIA] Jugador detectado! Persiguiendo...");
            }
        }
        else if (estadoActual == EstadoEnemigo.Persecucion)
        {
            if (distanciaHorizontal > distanciaAbandono || distanciaVertical > 2.5f || !jugadorVisible)
            {
                estadoActual = EstadoEnemigo.Regreso;
                Debug.Log("[EnemigoArenaIA] Jugador perdido. Regresando a patrulla...");
            }
            else if (distanciaHorizontal <= distanciaAtaque)
            {
                estadoActual = EstadoEnemigo.Ataque;
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }
        }
        else if (estadoActual == EstadoEnemigo.Ataque)
        {
            if (distanciaHorizontal > distanciaAtaque)
            {
                estadoActual = EstadoEnemigo.Persecucion;
            }
        }
    }

    private bool DetectarJugadorPorRaycast(float distanciaTotal, float distanciaVertical)
    {
        if (jugadorTransform == null) return false;
        if (distanciaTotal > distanciaAbandono) return false;
        if (distanciaVertical > 2.0f) return false; // same vertical sector

        // Raycast to check for obstacles on suelo/walls layer
        RaycastHit2D hit = Physics2D.Linecast(transform.position, jugadorTransform.position, capaSuelo);
        return hit.collider == null; // Visible if no wall/floor blocks the line of sight
    }

    private void ComportamientoPatrulla()
    {
        if (puntoPatrullaA == null || puntoPatrullaB == null) return;

        // Verify borders and walls
        if (DetectarBorde() || DetectarPared())
        {
            RotarPatrullaTarget();
        }

        float dir = puntoObjetivoPatrulla.position.x - transform.position.x;
        
        // Overshot target handling
        if (puntoObjetivoPatrulla == puntoPatrullaB && dir <= 0f)
        {
            puntoObjetivoPatrulla = puntoPatrullaA;
            dir = puntoObjetivoPatrulla.position.x - transform.position.x;
        }
        else if (puntoObjetivoPatrulla == puntoPatrullaA && dir >= 0f)
        {
            puntoObjetivoPatrulla = puntoPatrullaB;
            dir = puntoObjetivoPatrulla.position.x - transform.position.x;
        }

        // Close to target check
        if (Mathf.Abs(dir) <= distanciaCambioPatrulla)
        {
            RotarPatrullaTarget();
            dir = puntoObjetivoPatrulla.position.x - transform.position.x;
        }

        float velocidadX = Mathf.Sign(dir) * velocidadPatrulla;
        rb.linearVelocity = new Vector2(velocidadX, rb.linearVelocity.y);

        GirarHacia(velocidadX);
    }

    private void RotarPatrullaTarget()
    {
        puntoObjetivoPatrulla = (puntoObjetivoPatrulla == puntoPatrullaB) ? puntoPatrullaA : puntoPatrullaB;
    }

    private void ComportamientoPersecucion()
    {
        if (jugadorTransform == null) return;

        // Check Cliff/Edge to stop chasing off the ledge
        if (DetectarBorde() || DetectarPared())
        {
            // Stop to prevent falling or rubbing against wall
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            GirarHacia(jugadorTransform.position.x - transform.position.x);
            return;
        }

        float dir = jugadorTransform.position.x - transform.position.x;
        float velocidadX = Mathf.Sign(dir) * velocidadPersecucion;
        rb.linearVelocity = new Vector2(velocidadX, rb.linearVelocity.y);

        GirarHacia(dir);
    }

    private void ComportamientoAtaque()
    {
        if (jugadorTransform == null) return;

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        GirarHacia(jugadorTransform.position.x - transform.position.x);

        if (Time.time >= tiempoUltimoAtaque + tiempoEntreAtaques)
        {
            EjecutarAtaque();
        }
    }

    private void EjecutarAtaque()
    {
        tiempoUltimoAtaque = Time.time;
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // Perform Overlap check to deal damage
        if (puntoAtaque != null)
        {
            Collider2D col = Physics2D.OverlapCircle(puntoAtaque.position, 0.7f, capaJugador);
            if (col != null)
            {
                AplicarDanio(col.gameObject);
            }
        }
    }

    private void AplicarDanio(GameObject jugador)
    {
        // 1. Check for RecibirDanio on Player directly
        Component healthComp = jugador.GetComponent("PlayerHealth") 
            ?? jugador.GetComponent("CharacterHealth") 
            ?? jugador.GetComponent("HealthSystem")
            ?? jugador.GetComponent("InterfazJugador"); // if UI is on player

        if (healthComp != null)
        {
            var method = healthComp.GetType().GetMethod("RecibirDanio") ?? healthComp.GetType().GetMethod("TakeDamage");
            if (method != null)
            {
                method.Invoke(healthComp, new object[] { (float)dañoAtaque });
                Debug.Log($"[EnemigoArenaIA] Daño {dañoAtaque} aplicado a {jugador.name} usando {healthComp.GetType().Name}");
                return;
            }
        }

        // 2. Fallback to finding InterfazJugador globally in the scene
        var interfaz = Object.FindAnyObjectByType<InterfazJugador>();
        if (interfaz != null)
        {
            interfaz.RecibirDanio((float)dañoAtaque);
            Debug.Log($"[EnemigoArenaIA] Daño {dañoAtaque} aplicado mediante InterfazJugador global");
            return;
        }

        // 3. Fallback to IDamageable interface
        var damageable = jugador.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.RecibirDanio(dañoAtaque);
            Debug.Log($"[EnemigoArenaIA] Daño {dañoAtaque} aplicado usando IDamageable");
            return;
        }

        Debug.LogWarning("[EnemigoArenaIA] No se pudo encontrar un componente de vida compatible para dañar al jugador.");
    }

    private void ComportamientoRegreso()
    {
        if (puntoPatrullaA == null || puntoPatrullaB == null) return;

        // Move to whichever patrol point is closer
        float distA = Mathf.Abs(puntoPatrullaA.position.x - transform.position.x);
        float distB = Mathf.Abs(puntoPatrullaB.position.x - transform.position.x);
        Transform destino = (distA < distB) ? puntoPatrullaA : puntoPatrullaB;

        // If we are inside the patrol area (between A and B), resume patrolling
        float minX = Mathf.Min(puntoPatrullaA.position.x, puntoPatrullaB.position.x);
        float maxX = Mathf.Max(puntoPatrullaA.position.x, puntoPatrullaB.position.x);
        
        if (transform.position.x >= minX - 0.1f && transform.position.x <= maxX + 0.1f)
        {
            estadoActual = EstadoEnemigo.Patrulla;
            puntoObjetivoPatrulla = destino;
            Debug.Log("[EnemigoArenaIA] Regresó a zona de patrulla. Reanudando Patrulla...");
            return;
        }

        if (DetectarBorde() || DetectarPared())
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        float dir = destino.position.x - transform.position.x;
        float velocidadX = Mathf.Sign(dir) * velocidadPatrulla;
        rb.linearVelocity = new Vector2(velocidadX, rb.linearVelocity.y);

        GirarHacia(dir);
    }

    private bool DetectarBorde()
    {
        if (puntoBorde == null) return false;
        // Raycast down from edge check point
        RaycastHit2D hit = Physics2D.Raycast(puntoBorde.position, Vector2.down, 0.5f, capaSuelo);
        return hit.collider == null; // No ground means edge detected
    }

    private bool DetectarPared()
    {
        // Raycast forward from current center
        Vector2 direccion = mirandoDerecha ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direccion, 0.6f, capaSuelo);
        return hit.collider != null; // Hit means wall ahead
    }

    private void GirarHacia(float horizontalDir)
    {
        if (horizontalDir > 0.01f && !mirandoDerecha)
        {
            mirandoDerecha = true;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        else if (horizontalDir < -0.01f && mirandoDerecha)
        {
            mirandoDerecha = false;
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    private void ActualizarAnimaciones()
    {
        if (animator == null) return;

        float velocidadHorizontal = Mathf.Abs(rb.linearVelocity.x);
        animator.SetFloat("Speed", velocidadHorizontal);
        animator.SetBool("IsChasing", estadoActual == EstadoEnemigo.Persecucion || estadoActual == EstadoEnemigo.Ataque);
    }

    private void OnDrawGizmosSelected()
    {
        // 33. Draw Gizmos
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, distanciaAbandono);

        if (puntoPatrullaA != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(puntoPatrullaA.position, 0.2f);
        }
        if (puntoPatrullaB != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(puntoPatrullaB.position, 0.2f);
        }

        if (puntoAtaque != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(puntoAtaque.position, 0.7f);
        }

        if (jugadorTransform != null && estadoActual == EstadoEnemigo.Persecucion)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, jugadorTransform.position);
        }
    }
}
