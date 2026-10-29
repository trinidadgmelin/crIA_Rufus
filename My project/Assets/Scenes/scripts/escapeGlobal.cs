using UnityEngine;

public class GlobalQuitHandler : MonoBehaviour
{
    private static GlobalQuitHandler instance;

    void Awake()
    {
        // Patrón Singleton para asegurar que solo exista un objeto de estos y persista
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Evita que se destruya al cambiar de escena
        }
        else
        {
            Destroy(gameObject); // Si ya hay uno en otra escena, destruye el duplicado
            return;
        }
    }

    void Update()
    {
        // Detecta si se presiona la tecla Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Nota: Application.Quit() no cierra el editor de Unity, solo funciona en builds (ejecutables)
            Application.Quit();

#if UNITY_EDITOR
            // Esto es solo para que también se detenga el juego mientras estás probando en el Editor
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}