using UnityEngine;
using System.Collections;

public class mateInteraccion : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject mate;

    [Header("Configuración de Tiempo")]
    public float segundosVisible = 3f;

    // Este método ahora puede ser llamado de forma segura cuando se interactúa con el mate
    public void ActivarMateLocal()
    {
        StartCoroutine(RutinaMateLocal());
    }

    IEnumerator RutinaMateLocal()
    {
        if (mate != null) mate.SetActive(false);

        yield return new WaitForSeconds(segundosVisible);

        if (mate != null) mate.SetActive(true);
    }
}