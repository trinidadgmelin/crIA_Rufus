using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalQuitHandler : MonoBehaviour
{
    private static GlobalQuitHandler instance;

    void Awake()
    {
        // Patrón Singleton persistente
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Debe estar en la raíz de la jerarquía
            Debug.Log("[GlobalQuitHandler] Instancia creada y marcada como DontDestroyOnLoad.");
        }
        else
        {
            Debug.LogWarning("[GlobalQuitHandler] Duplicado detectado en la escena " + SceneManager.GetActiveScene().name + ". Destruyendo.");
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        // 1. Volver al inicio presionando la letra B
        if (Input.GetKeyDown(KeyCode.B))
        {
            VolverAInicio();
        }

        // 2. Detecta si se presiona la tecla Escape para salir del juego
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

    public void VolverAInicio()
    {
        SceneManager.LoadScene("inicio");
    }
}