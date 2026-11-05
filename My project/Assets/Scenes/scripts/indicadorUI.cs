using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class indicadorUI : MonoBehaviour
{
    [Header("Identificador de Necesidad")]
    [Tooltip("Debe coincidir exactamente con: 'Hambre', 'Dormir' o 'Caricias'")]
    public string nombreNecesidad;

    [Header("Sprites Progresivos")]
    [Tooltip("Coloca los sprites en orden: desde el estado inicial hasta el estado más crítico o urgente.")]
    public Sprite[] spritesProgresivos;

    private Image imagenUI;

    void Awake()
    {
        imagenUI = GetComponent<Image>();
    }

    void Update()
    {
        timerNecesidades elTimer = timerNecesidades.Instance;
        if (elTimer == null) return;

        // Comprobamos si esta necesidad es la que está activa actualmente en la cabeza de la cola
        bool estaEnFoco = (elTimer.colaNecesidades.Count > 0 && elTimer.colaNecesidades[0] == nombreNecesidad);

        if (estaEnFoco)
        {
            // Obtenemos los datos de la necesidad correspondiente
            timerNecesidades.Necesidad nec = null;
            if (nombreNecesidad == "Hambre") nec = elTimer.hambre;
            else if (nombreNecesidad == "Dormir") nec = elTimer.dormir;
            else if (nombreNecesidad == "Caricias") nec = elTimer.caricias;

            if (nec != null && nec.estaEsperando)
            {
                // Calculamos el porcentaje de tiempo transcurrido (de 0.0 a 1.0)
                float tiempoTranscurrido = nec.tiempoMaximoEspera - nec.timerEsperaActual;
                float porcentaje = Mathf.Clamp01(tiempoTranscurrido / nec.tiempoMaximoEspera);

                ActualizarSpritePorcentaje(porcentaje);
            }
        }
        else
        {
            // Si la necesidad no está activa, mostramos por defecto el primer sprite (o puedes apagar el objeto si prefieres)
            if (spritesProgresivos != null && spritesProgresivos.Length > 0)
            {
                imagenUI.sprite = spritesProgresivos[0];
            }
        }
    }

    void ActualizarSpritePorcentaje(float porcentaje)
    {
        if (spritesProgresivos == null || spritesProgresivos.Length == 0) return;

        // Dividimos el porcentaje actual entre la cantidad de sprites para determinar qué índice mostrar
        int indice = Mathf.FloorToInt(porcentaje * spritesProgresivos.Length);
        indice = Mathf.Clamp(indice, 0, spritesProgresivos.Length - 1);

        imagenUI.sprite = spritesProgresivos[indice];
    }
}