using UnityEngine;
using System.Collections.Generic;

public class SelectorHitboxes : MonoBehaviour
{
    [Header("Indicador Visual de Selección (Opcional)")]
    public GameObject indicadorVisual;

    private List<Transform> hitboxesOrdenadas = new List<Transform>();
    private int indiceSeleccionado = 0;
    private Camera camara;

    void Start()
    {
        camara = Camera.main;
        ActualizarListaHitboxes();
    }

    public void ActualizarListaHitboxes()
    {
        hitboxesOrdenadas.Clear();

        popUpHitbox2[] scriptPop2 = FindObjectsOfType<popUpHitbox2>();
        popUpHitbox3[] scriptPop3 = FindObjectsOfType<popUpHitbox3>();
        HitboxOtraEscena[] scriptOtraEscena = FindObjectsOfType<HitboxOtraEscena>();

        List<Transform> todasLasHitboxes = new List<Transform>();

        foreach (var h in scriptPop2) todasLasHitboxes.Add(h.transform);
        foreach (var h in scriptPop3) todasLasHitboxes.Add(h.transform);
        foreach (var h in scriptOtraEscena) todasLasHitboxes.Add(h.transform);

        // Ordenamos de izquierda a derecha por posición X
        todasLasHitboxes.Sort((a, b) => a.position.x.CompareTo(b.position.x));

        hitboxesOrdenadas = todasLasHitboxes;
        indiceSeleccionado = 0;
        ActualizarSeleccionVisual();
    }

    void Update()
    {
        if (hitboxesOrdenadas == null || hitboxesOrdenadas.Count == 0) return;

        hitboxesOrdenadas.RemoveAll(h => h == null || !h.gameObject.activeInHierarchy);
        if (hitboxesOrdenadas.Count == 0) return;

        indiceSeleccionado = Mathf.Clamp(indiceSeleccionado, 0, hitboxesOrdenadas.Count - 1);

        int indiceAnterior = indiceSeleccionado;

        // --- 1. NAVEGACIÓN (Teclado O Arduino) ---
        bool moverDerecha = Input.GetKeyDown(KeyCode.RightArrow) || (ArduinoManager.Instance != null && ArduinoManager.Instance.botonDerechaPresionado);
        bool moverIzquierda = Input.GetKeyDown(KeyCode.LeftArrow) || (ArduinoManager.Instance != null && ArduinoManager.Instance.botonIzquierdaPresionado);

        if (moverDerecha)
        {
            indiceSeleccionado = (indiceSeleccionado + 1) % hitboxesOrdenadas.Count;
        }
        else if (moverIzquierda)
        {
            indiceSeleccionado = (indiceSeleccionado - 1 + hitboxesOrdenadas.Count) % hitboxesOrdenadas.Count;
        }

        // Si cambió la selección con las flechas, actualizamos los fondos y la posición visual
        if (indiceAnterior != indiceSeleccionado)
        {
            ActualizarSeleccionVisual();
        }

        // --- 2. ACCIÓN / SELECCIÓN (Enter O Botón Central del Arduino) ---
        bool accionar = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || (ArduinoManager.Instance != null && ArduinoManager.Instance.botonSeleccionarPresionado);

        if (accionar)
        {
            SimularClickEnHitboxSeleccionada();
        }
    }

    void ActualizarSeleccionVisual()
    {
        // 1. Primero desactivamos el hover en TODAS las hitboxes para evitar conflictos de sobreescritura
        foreach (Transform t in hitboxesOrdenadas)
        {
            if (t == null) continue;

            var p2 = t.GetComponent<popUpHitbox2>();
            if (p2 != null) p2.DesactivarFondoHover();

            var p3 = t.GetComponent<popUpHitbox3>();
            if (p3 != null) p3.DesactivarFondoHover();

            var otraEscena = t.GetComponent<HitboxOtraEscena>();
            if (otraEscena != null) otraEscena.DesactivarFondoHover();
        }

        // 2. Luego activamos el hover ÚNICAMENTE en la hitbox que está seleccionada actualmente
        if (hitboxesOrdenadas.Count > 0)
        {
            Transform tActual = hitboxesOrdenadas[indiceSeleccionado];
            if (tActual != null)
            {
                var p2 = tActual.GetComponent<popUpHitbox2>();
                if (p2 != null) p2.ActivarFondoHover();

                var p3 = tActual.GetComponent<popUpHitbox3>();
                if (p3 != null) p3.ActivarFondoHover();

                var otraEscena = tActual.GetComponent<HitboxOtraEscena>();
                if (otraEscena != null) otraEscena.ActivarFondoHover();
            }

            // Posicionar indicador visual si lo tenés asignado
            if (indicadorVisual != null)
            {
                indicadorVisual.SetActive(true);
                indicadorVisual.transform.position = tActual.position;
            }
        }
    }

    void SimularClickEnHitboxSeleccionada()
    {
        Transform hitboxActual = hitboxesOrdenadas[indiceSeleccionado];

        popUpHitbox2 pop2 = hitboxActual.GetComponent<popUpHitbox2>();
        if (pop2 != null) { pop2.AbrirPorTeclado(); return; }

        popUpHitbox3 pop3 = hitboxActual.GetComponent<popUpHitbox3>();
        if (pop3 != null) { pop3.AbrirPorTeclado(); return; }

        HitboxOtraEscena otraEscena = hitboxActual.GetComponent<HitboxOtraEscena>();
        if (otraEscena != null) { otraEscena.CambiarEscena(); return; }
    }
}