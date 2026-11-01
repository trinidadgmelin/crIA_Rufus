using UnityEngine;
using System.Collections;

public class necesidadesUI : MonoBehaviour
{
    [Header("Sprite por Defecto de Rufus")]
    public SpriteRenderer rufusSpriteRenderer;
    public Sprite spriteNormal;

    [Header("Configuración de Hambre (Prioridad 1)")]
    public Sprite rufusEnojo;
    public GameObject hambreAd;

    [Header("Configuración de Caricias (Prioridad 2)")]
    public Sprite rufusTriste;
    public GameObject mimoAd;

    [Header("Configuración de Dormir (Prioridad 3)")]
    public Sprite rufusSueño;
    public GameObject sueñoAd;

    private bool bloquearActualizacion = false;

    void Start()
    {
        ActualizarVisuales();
    }

    void Update()
    {
        if (!bloquearActualizacion)
        {
            ActualizarVisuales();
        }
    }

    public void ActualizarVisuales()
    {
        timerNecesidades elTimer = timerNecesidades.Instance;

        if (elTimer == null)
        {
            elTimer = FindObjectOfType<timerNecesidades>();
            if (elTimer == null) return;
        }

        bool tieneHambre = elTimer.tieneHambre;
        bool quiereCaricias = elTimer.quiereCaricias;
        bool quiereDormir = elTimer.quiereDormir;

        // --- SISTEMA DE PRIORIDAD (Solo una necesidad activa a la vez) ---
        bool mostrarHambre = tieneHambre;
        bool mostrarCaricias = !tieneHambre && quiereCaricias;
        bool mostrarDormir = !tieneHambre && !quiereCaricias && quiereDormir;

        // 1. Activar solo el cartel de la necesidad con mayor prioridad
        if (hambreAd != null) hambreAd.SetActive(mostrarHambre);
        if (mimoAd != null) mimoAd.SetActive(mostrarCaricias);
        if (sueñoAd != null) sueñoAd.SetActive(mostrarDormir);

        // 2. Mostrar el sprite correspondiente a esa única necesidad activa
        if (rufusSpriteRenderer != null)
        {
            if (mostrarHambre && rufusEnojo != null)
            {
                rufusSpriteRenderer.sprite = rufusEnojo;
            }
            else if (mostrarCaricias && rufusTriste != null)
            {
                rufusSpriteRenderer.sprite = rufusTriste;
            }
            else if (mostrarDormir && rufusSueño != null)
            {
                rufusSpriteRenderer.sprite = rufusSueño;
            }
            else if (spriteNormal != null)
            {
                rufusSpriteRenderer.sprite = spriteNormal; // Estado normal
            }
        }
    }

    // Método que usan las interacciones para bloquear la UI y apagar avisos temporalmente
    public void MostrarSpriteTemporal(Sprite nuevoSprite, float duracion)
    {
        StopAllCoroutines();
        StartCoroutine(RutinaSpriteTemporario(nuevoSprite, duracion));
    }

    private IEnumerator RutinaSpriteTemporario(Sprite spriteTemp, float duracion)
    {
        bloquearActualizacion = true;

        if (rufusSpriteRenderer != null && spriteTemp != null)
        {
            rufusSpriteRenderer.sprite = spriteTemp;
        }

        // Apagar todos los carteles mientras dura la animación de éxito
        if (hambreAd != null) hambreAd.SetActive(false);
        if (mimoAd != null) mimoAd.SetActive(false);
        if (sueñoAd != null) sueñoAd.SetActive(false);

        yield return new WaitForSeconds(duracion);

        bloquearActualizacion = false;
    }
}