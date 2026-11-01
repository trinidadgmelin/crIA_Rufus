using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class indicacionesTimer : MonoBehaviour
{
    [Header("Configuración del Timer")]
    public float tiempoDeEspera = 10f; // Segundos antes de empezar a desaparecer
    public float duracionFade = 1f;    // Lo que tarda en desvanecerse por completo

    private CanvasGroup canvasGroup;

    void Awake()
    {
        // Buscamos el CanvasGroup, y si no lo tiene, se lo agregamos solos
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
        // 1. Espera el tiempo principal (ej. 10 segundos)
        yield return new WaitForSeconds(tiempoDeEspera);

        // 2. Realiza el desvanecimiento progresivo (Fade Out)
        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < duracionFade)
        {
            tiempoTranscurrido += Time.deltaTime;
            // Va bajando la opacidad de 1 (visible) a 0 (invisible)
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, tiempoTranscurrido / duracionFade);
            yield return null; // Espera al siguiente frame
        }

        // Nos aseguramos de que quede totalmente invisible al final
        canvasGroup.alpha = 0f;

        // 3. Desactiva el objeto completo
        gameObject.SetActive(false);
    }
}