using UnityEngine;
using UnityEngine.UI;

public class resumenVictoria : MonoBehaviour
{
    [Header("Tipo de Necesidad a Mostrar")]
    public string tipoNecesidad; // Escribe en el Inspector: "Hambre", "Dormir" o "Caricias"

    private Image imagenUI;

    [Header("Sprites de Éxito / Completado")]
    public Sprite spriteCompletadoHambre;
    public Sprite spriteCompletadoDormir;
    public Sprite spriteCompletadoCaricias;

    void Start()
    {
        imagenUI = GetComponent<Image>();
        if (imagenUI == null) return;

        timerNecesidades elTimer = timerNecesidades.Instance;
        if (elTimer == null) return;

        // Dependiendo de qué necesidad sea este objeto, mostramos su sprite correspondiente
        if (tipoNecesidad == "Hambre" && spriteCompletadoHambre != null)
        {
            imagenUI.sprite = spriteCompletadoHambre;
            imagenUI.color = Color.white;
        }
        else if (tipoNecesidad == "Dormir" && spriteCompletadoDormir != null)
        {
            imagenUI.sprite = spriteCompletadoDormir;
            imagenUI.color = Color.white;
        }
        else if (tipoNecesidad == "Caricias" && spriteCompletadoCaricias != null)
        {
            imagenUI.sprite = spriteCompletadoCaricias;
            imagenUI.color = Color.white;
        }
    }
}