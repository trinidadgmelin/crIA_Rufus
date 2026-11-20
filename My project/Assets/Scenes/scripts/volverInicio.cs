using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorVolverInicio : MonoBehaviour
{
    private static ControladorVolverInicio instance;

    void Awake()
    {
        // Patrón Singleton para que persista entre escenas
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Evita que se destruya al cambiar de escena
            Debug.Log("[ControladorVolverInicio] Activado y persistente.");
        }
        else
        {
            // Si ya existe uno previo en otra escena, destruimos este duplicado
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        // 1. Verificamos si el botón central del Arduino fue presionado en este frame
        bool botonArduino = ArduinoManager.Instance != null && ArduinoManager.Instance.botonSeleccionarPresionado;

        // 2. Verificamos si se presionó Enter en el teclado
        bool teclaEnter = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);

        // Si ocurre cualquiera de las dos acciones, volvemos al inicio
        if (botonArduino || teclaEnter)
        {
            VolverAInicio();
        }
    }

    public void VolverAInicio()
    {
        SceneManager.LoadScene("inicio");
    }
}