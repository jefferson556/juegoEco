using System.Collections;
using UnityEngine;

public class ParedDestructible : MonoBehaviour
{
    [Header("Resistencia")]
    [SerializeField] private int resistenciaMaxima = 3;

    [Header("Referencias")]
    [SerializeField] private SpriteRenderer visualPared;
    [SerializeField] private Collider2D colliderPared;
    [SerializeField] private GameObject contenedorFragmentos;
    [SerializeField] private ParticleSystem particulasImpacto;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoGolpe;
    [SerializeField] private AudioClip sonidoDestruccion;

    [Header("Efectos")]
    [SerializeField] private float fuerzaFragmentos = 3f;
    [SerializeField] private float tiempoEliminar = 2f;

    private int resistenciaActual;
    private bool destruida;
    private Vector3 posicionOriginal;

    private void Awake()
    {
        resistenciaActual = resistenciaMaxima;
        posicionOriginal = transform.localPosition;

        if (contenedorFragmentos != null)
            contenedorFragmentos.SetActive(false);
    }

    public void RecibirGolpe(int daño)
    {
        if (destruida || daño <= 0)
            return;

        resistenciaActual -= daño;
        Debug.Log($"[ParedDestructible] Recibió {daño} de daño. Resistencia actual: {resistenciaActual}");

        if (particulasImpacto != null)
            particulasImpacto.Play();

        if (audioSource != null && sonidoGolpe != null)
            audioSource.PlayOneShot(sonidoGolpe);

        StartCoroutine(Vibracion());

        if (resistenciaActual <= 0)
            DestruirPared();
    }

    public void DestruirPared()
    {
        if (destruida)
            return;

        destruida = true;
        Debug.Log("[ParedDestructible] Pared destruida!");

        if (colliderPared != null)
            colliderPared.enabled = false;

        if (visualPared != null)
            visualPared.enabled = false;

        if (audioSource != null && sonidoDestruccion != null)
            audioSource.PlayOneShot(sonidoDestruccion);

        if (contenedorFragmentos != null)
        {
            contenedorFragmentos.SetActive(true);

            // Find all Rigidbody2D components in fragments and add random force
            Rigidbody2D[] rbs = contenedorFragmentos.GetComponentsInChildren<Rigidbody2D>(true);
            foreach (Rigidbody2D fragmento in rbs)
            {
                fragmento.gameObject.SetActive(true);
                Vector2 direccion = new Vector2(
                    Random.Range(-1f, 1f),
                    Random.Range(0.5f, 1.3f)
                ).normalized;

                fragmento.AddForce(
                    direccion * fuerzaFragmentos,
                    ForceMode2D.Impulse
                );

                fragmento.AddTorque(
                    Random.Range(-4f, 4f),
                    ForceMode2D.Impulse
                );

                // Auto destroy fragment after a delay
                Destroy(fragmento.gameObject, Random.Range(1.5f, 2.0f));
            }
        }

        Destroy(gameObject, tiempoEliminar);
    }

    private IEnumerator Vibracion()
    {
        float duracion = 0.12f;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;

            transform.localPosition =
                posicionOriginal +
                (Vector3)Random.insideUnitCircle * 0.04f;

            yield return null;
        }

        transform.localPosition = posicionOriginal;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
            RecibirGolpe(1);
    }
}
