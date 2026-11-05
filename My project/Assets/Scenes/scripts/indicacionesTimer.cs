using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class indicacionesTimer : MonoBehaviour
{
    [Header("Configuración del Timer")]
    public float tiempoDeEspera = 10f; // Segundos antes de empezar a desaparecer
    public float duracionFade = 1f;    // Lo que tarda en desvanecerse por completo

    [Header("Identificador Único")]
    [Tooltip("Pon un nombre diferente en cada escena u objeto para que lleven cuentas separadas.")]
    public string idIndicacion = "Indicaciones_Nivel1";

    // Diccionario estático que guarda qué IDs ya terminaron de verse en esta sesión
    private static Dictionary<string, bool> estadosVistas = new Dictionary<string, bool>();

    private CanvasGroup canvasGroup;

    void Awake()
    {
        // 1. Verificamos si este ID específico ya terminó de verse en la sesión actual
        if (estadosVistas.ContainsKey(idIndicacion) && estadosVistas[idIndicacion])
        {
            gameObject.SetActive(false);
            return;
        }

        // 2. Buscamos el CanvasGroup, y si no lo tiene, se lo agregamos
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    void Start()
    {
        StartCoroutine(TimerYFadeOut());
    }

    IEnumerator TimerYFadeOut()
    {
        // 1. Espera el tiempo principal
        yield return new WaitForSeconds(tiempoDeEspera);

        // 2. Realiza el desvanecimiento progresivo (Fade Out)
        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < duracionFade)
        {
            tiempoTranscurrido += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, tiempoTranscurrido / duracionFade);
            yield return null;
        }

        // Nos aseguramos de que quede totalmente invisible al final
        canvasGroup.alpha = 0f;

        // 3. 🎯 Guardamos que ESTE ID en específico ya terminó de verse
        estadosVistas[idIndicacion] = true;

        // 4. Desactiva el objeto completo
        gameObject.SetActive(false);
    }
}