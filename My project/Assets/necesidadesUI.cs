using UnityEngine;

public class SceneNeedsUI : MonoBehaviour
{
    [Header("Referencias Visuales de esta Escena")]
    public SpriteRenderer rufusSpriteRenderer;
    public Sprite spriteNormal;
    public Sprite spriteHambre;
    public Sprite spriteCaricias;
    public Sprite spriteDormir;

    [Header("GameObjects de Advertencia (Paneles / Iconos)")]
    public GameObject advertenciaHambre;
    public GameObject advertenciaCaricias;
    public GameObject advertenciaDormir;

    void Start()
    {
        // Apagamos las advertencias al iniciar la escena por seguridad
        ActualizarVisuales();
    }

    public void ActualizarVisuales()
    {
        if (timerNecesidades.Instance == null) return;

        bool hambre = timerNecesidades.Instance.tieneHambre;
        bool caricias = timerNecesidades.Instance.quiereCaricias;
        bool dormir = timerNecesidades.Instance.quiereDormir;

        // 1. Activar o desactivar los gameobjects de advertencia
        if (advertenciaHambre != null) advertenciaHambre.SetActive(hambre);
        if (advertenciaCaricias != null) advertenciaCaricias.SetActive(caricias);
        if (advertenciaDormir != null) advertenciaDormir.SetActive(dormir);

        // 2. Cambiar el sprite de Rufus según la prioridad de necesidades
        if (rufusSpriteRenderer != null && spriteNormal != null)
        {
            if (hambre && spriteHambre != null)
            {
                rufusSpriteRenderer.sprite = spriteHambre;
            }
            else if (caricias && spriteCaricias != null)
            {
                rufusSpriteRenderer.sprite = spriteCaricias;
            }
            else if (dormir && spriteDormir != null)
            {
                rufusSpriteRenderer.sprite = spriteDormir;
            }
            else
            {
                rufusSpriteRenderer.sprite = spriteNormal; // Estado normal
            }
        }
    }
}