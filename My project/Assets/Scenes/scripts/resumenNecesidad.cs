using UnityEngine;
using UnityEngine.UI;

public class resumenNecesidad : MonoBehaviour
{
    private Image imagenUI;

    [Header("Sprites de Derrota (Iconos Vacíos o Específicos)")]
    public Sprite spriteHambreVacio;
    public Sprite spriteDormirVacio;
    public Sprite spriteCariciasVacio;

    void Start()
    {
        imagenUI = GetComponent<Image>();
        if (imagenUI == null) return;

        // Consultamos cuál fue la necesidad exacta que provocó el escape
        string causa = timerNecesidades.necesidadQueProvocoEscape;

        if (causa == "Hambre" && spriteHambreVacio != null)
        {
            imagenUI.sprite = spriteHambreVacio;
            imagenUI.color = Color.white;
        }
        else if (causa == "Dormir" && spriteDormirVacio != null)
        {
            imagenUI.sprite = spriteDormirVacio;
            imagenUI.color = Color.white;
        }
        else if (causa == "Caricias" && spriteCariciasVacio != null)
        {
            imagenUI.sprite = spriteCariciasVacio;
            imagenUI.color = Color.white;
        }
    }
}