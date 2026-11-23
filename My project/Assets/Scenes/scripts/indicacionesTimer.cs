using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class indicacionesTimer : MonoBehaviour
{
    [Header("Configuración del Timer")]
    public float tiempoDeEspera = 10f;
    public float duracionFade = 1f;

    [Header("Identificador Único")]
    public string idIndicacion = "Indicaciones_Nivel3";

    private static Dictionary<string, bool> estadosVistas = new Dictionary<string, bool>();

    private CanvasGroup canvasGroup;
    private bool registroActivo = false;

    void Awake()
    {
        Debug.Log($"[Indicaciones] Awake en {gameObject.name} | id={idIndicacion}");

        if (estadosVistas.ContainsKey(idIndicacion) && estadosVistas[idIndicacion])
        {
            Debug.Log("[Indicaciones] Ya estaba marcada como vista. Se desactiva.");
            gameObject.SetActive(false);
            return;
        }

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void Start()
    {
        Debug.Log($"[Indicaciones] Start -> EstaDurmiendo = {ControladorGlobalInteracciones.Instance?.EstaDurmiendo}");
        StartCoroutine(TimerYFadeOut());
    }

    IEnumerator TimerYFadeOut()
    {
        // Espera un frame tras cargar la escena para asegurar estados actualizados
        yield return null;

        // Si llegamos por la rutina de dormir (tecla 3 / nido), espera a que
        // termine el tiempo configurado del sprite/estado de dormir
        while (ControladorGlobalInteracciones.Instance != null &&
               ControladorGlobalInteracciones.Instance.EstaDurmiendo)
        {
            yield return null;
        }

        // Mostrar indicaciones
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        MarcarIndicacionActiva(true);

        // Tiempo visible configurado
        yield return new WaitForSeconds(tiempoDeEspera);

        // Fade Out
        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < duracionFade)
        {
            tiempoTranscurrido += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, tiempoTranscurrido / duracionFade);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        estadosVistas[idIndicacion] = true;
        MarcarIndicacionActiva(false);

        gameObject.SetActive(false);
    }

    public static void ResetearIndicacion(string id)
    {
        if (estadosVistas.ContainsKey(id))
        {
            estadosVistas[id] = false;
        }
    }

    private void MarcarIndicacionActiva(bool activa)
    {
        if (activa && !registroActivo)
        {
            registroActivo = true;
            timerNecesidades.RegistrarIndicacionActiva(true);
        }
        else if (!activa && registroActivo)
        {
            registroActivo = false;
            timerNecesidades.RegistrarIndicacionActiva(false);
        }
    }

    void OnDisable()
    {
        MarcarIndicacionActiva(false);
    }

    void OnDestroy()
    {
        MarcarIndicacionActiva(false);
    }
}