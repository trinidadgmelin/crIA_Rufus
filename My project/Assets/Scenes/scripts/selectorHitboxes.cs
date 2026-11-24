using UnityEngine;
using System.Collections.Generic;

public class SelectorHitboxes : MonoBehaviour
{
    [Header("Indicador Visual de Selección (Opcional)")]
    public GameObject indicadorVisual;

    private List<Transform> hitboxesOrdenadas = new List<Transform>();
    private int indiceSeleccionado = 0;
    private bool haySeleccionActiva = false;
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

        haySeleccionActiva = false;
        DesactivarTodosLosHovers();
        if (indicadorVisual != null) indicadorVisual.SetActive(false);
    }

    void Update()
    {
        // 🎯 BLOQUEO TOTAL: Si hay popup abierto o Rufus está mostrando un sprite temporal, 
        // anulamos completamente las hitboxes, los hovers y la navegación por teclado/Arduino.
        if (timerNecesidades.HayPopupAbierto) return;

        if (necesidadesUI.Instance != null && necesidadesUI.Instance.EstaBloqueado)
        {
            if (indicadorVisual != null) indicadorVisual.SetActive(false);
            DesactivarTodosLosHovers();
            return;
        }

        if (hitboxesOrdenadas == null || hitboxesOrdenadas.Count == 0) return;

        hitboxesOrdenadas.RemoveAll(h => h == null || !h.gameObject.activeInHierarchy);
        if (hitboxesOrdenadas.Count == 0) return;

        indiceSeleccionado = Mathf.Clamp(indiceSeleccionado, 0, hitboxesOrdenadas.Count - 1);

        int indiceAnterior = indiceSeleccionado;

        // --- 1. NAVEGACIÓN (Teclado normal / Botones de Arduino invertidos) ---
        bool arduinoDer = ArduinoManager.Instance != null && ArduinoManager.Instance.botonDerechaPresionado;
        bool arduinoIzq = ArduinoManager.Instance != null && ArduinoManager.Instance.botonIzquierdaPresionado;

        bool moverDerecha = Input.GetKeyDown(KeyCode.RightArrow) || arduinoIzq;
        bool moverIzquierda = Input.GetKeyDown(KeyCode.LeftArrow) || arduinoDer;

        if (moverDerecha)
        {
            if (!haySeleccionActiva)
            {
                haySeleccionActiva = true;
                indiceSeleccionado = hitboxesOrdenadas.Count - 1;
            }
            else
            {
                indiceSeleccionado = (indiceSeleccionado + 1) % hitboxesOrdenadas.Count;
            }
            ActualizarSeleccionVisual();
        }
        else if (moverIzquierda)
        {
            if (!haySeleccionActiva)
            {
                haySeleccionActiva = true;
                indiceSeleccionado = 0;
            }
            else
            {
                indiceSeleccionado = (indiceSeleccionado - 1 + hitboxesOrdenadas.Count) % hitboxesOrdenadas.Count;
            }
            ActualizarSeleccionVisual();
        }

        // --- 2. ACCIÓN / SELECCIÓN (Enter O Botón Central del Arduino) ---
        bool accionar = haySeleccionActiva && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || (ArduinoManager.Instance != null && ArduinoManager.Instance.botonSeleccionarPresionado));

        if (accionar)
        {
            SimularClickEnHitboxSeleccionada();
        }
    }

    void ActualizarSeleccionVisual()
    {
        DesactivarTodosLosHovers();

        if (haySeleccionActiva && hitboxesOrdenadas.Count > 0)
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

            if (indicadorVisual != null)
            {
                indicadorVisual.SetActive(true);
                indicadorVisual.transform.position = hitboxesOrdenadas[indiceSeleccionado].position;
            }
        }
        else
        {
            if (indicadorVisual != null) indicadorVisual.SetActive(false);
        }
    }

    void DesactivarTodosLosHovers()
    {
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
    }

    void SimularClickEnHitboxSeleccionada()
    {
        if (!haySeleccionActiva) return;

        Transform hitboxActual = hitboxesOrdenadas[indiceSeleccionado];

        popUpHitbox2 pop2 = hitboxActual.GetComponent<popUpHitbox2>();
        if (pop2 != null) { pop2.AbrirPorTeclado(); return; }

        popUpHitbox3 pop3 = hitboxActual.GetComponent<popUpHitbox3>();
        if (pop3 != null) { pop3.AbrirPorTeclado(); return; }

        HitboxOtraEscena otraEscena = hitboxActual.GetComponent<HitboxOtraEscena>();
        if (otraEscena != null) { otraEscena.CambiarEscena(); return; }
    }
}