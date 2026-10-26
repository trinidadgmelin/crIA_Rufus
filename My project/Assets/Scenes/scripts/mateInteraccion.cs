using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class mateInteraccion : MonoBehaviour, IPointerClickHandler
{
    [Header("Referencias")]
    public GameObject mate;
    public Image rufusFeliz;
    public Sprite rufusMate;

    public void OnPointerClick(PointerEventData eventData)
    {
        EjecutarAccion();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            EjecutarAccion();
        }
    }

    void EjecutarAccion()
    {
        if (mate != null)
        {
            mate.SetActive(false);
        }

        if (rufusFeliz != null && rufusMate != null)
        {
            rufusFeliz.sprite = rufusMate;
        }

        Debug.Log("¡Acción realizada: el mate desapareció y el pájaro ahora toma mate!");
    }
}