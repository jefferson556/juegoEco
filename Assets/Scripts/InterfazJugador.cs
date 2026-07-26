using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InterfazJugador : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image rellenoVida;
    [SerializeField] private Image rellenoCorrupcion;
    [SerializeField] private TMP_Text textoCristales;
    [SerializeField] private TMP_Text textoTiempo;

    [Header("Player Stats")]
    [SerializeField] private float vidaMaxima = 100f;
    [SerializeField] private float vidaActual = 100f;
    [SerializeField] private float corrupcionMaxima = 100f;
    [SerializeField] private float corrupcionActual = 0f;
    [SerializeField] private int cristales = 0;
    [SerializeField] private float tiempoRestante = 90f;

    private bool timerRunning = true;

    private void Start()
    {
        UpdateVisuals();
    }

    private void Update()
    {
        if (timerRunning)
        {
            if (tiempoRestante > 0f)
            {
                tiempoRestante -= Time.deltaTime;
                if (tiempoRestante <= 0f)
                {
                    tiempoRestante = 0f;
                    timerRunning = false;
                    TiempoAgotado();
                }
            }
            UpdateTiempoText();
        }

        // Keep updating fill amounts and crystals in case they change dynamically
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (rellenoVida != null)
        {
            rellenoVida.fillAmount = Mathf.Clamp01(vidaActual / vidaMaxima);
        }

        if (rellenoCorrupcion != null)
        {
            rellenoCorrupcion.fillAmount = Mathf.Clamp01(corrupcionActual / corrupcionMaxima);
        }

        if (textoCristales != null)
        {
            textoCristales.text = cristales.ToString();
        }
    }

    private void UpdateTiempoText()
    {
        if (textoTiempo != null)
        {
            int minutes = Mathf.FloorToInt(tiempoRestante / 60f);
            int seconds = Mathf.FloorToInt(tiempoRestante % 60f);
            textoTiempo.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void TiempoAgotado()
    {
        Debug.Log("[InterfazJugador] Tiempo agotado!");
    }

    // Public Methods
    public void RecibirDanio(float cantidad)
    {
        vidaActual = Mathf.Clamp(vidaActual - cantidad, 0f, vidaMaxima);
        UpdateVisuals();
    }

    public void Curar(float cantidad)
    {
        vidaActual = Mathf.Clamp(vidaActual + cantidad, 0f, vidaMaxima);
        UpdateVisuals();
    }

    public void AumentarCorrupcion(float cantidad)
    {
        corrupcionActual = Mathf.Clamp(corrupcionActual + cantidad, 0f, corrupcionMaxima);
        UpdateVisuals();
    }

    public void ReducirCorrupcion(float cantidad)
    {
        corrupcionActual = Mathf.Clamp(corrupcionActual - cantidad, 0f, corrupcionMaxima);
        UpdateVisuals();
    }

    public void AgregarCristal(int cantidad)
    {
        cristales = Mathf.Max(0, cristales + cantidad);
        UpdateVisuals();
    }

    public void RestarCristal(int cantidad)
    {
        cristales = Mathf.Max(0, cristales - cantidad);
        UpdateVisuals();
    }

    public void AgregarTiempo(float cantidad)
    {
        tiempoRestante = Mathf.Max(0f, tiempoRestante + cantidad);
        if (tiempoRestante > 0f)
        {
            timerRunning = true;
        }
        UpdateTiempoText();
    }
}
