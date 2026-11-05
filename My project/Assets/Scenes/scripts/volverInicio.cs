using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorVolverInicio : MonoBehaviour
{
    void Update()
    {
        // 1. Verificamos si el botón central del Arduino fue presionado en este frame
        bool botonArduino = ArduinoManager.Instance != null && ArduinoManager.Instance.botonSeleccionarPresionado;

        // 2. Verificamos si se presionó Enter en el teclado (con KeyCode en mayúscula)
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