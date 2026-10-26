using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class dialogos : MonoBehaviour
{
    [Header("UI General")]
    public TextMeshProUGUI textoDialogo;
    public GameObject fondoDialogo;
    public Image fondoNidoImage;

    [Header("Textos y Fondos")]
    [TextArea(3, 10)]
    public string[] sentences;
    public Sprite[] backgroundSprites;

    [Header("Botones e Indicaciones Condicionales")]
    public GameObject panelTresBotones;
    public GameObject objetoIndicaciones;
    public int indexParaMostrarBotones = 2;

    private int index = 0;

    // ESTA VARIABLE ES LA CLAVE PARA COMUNICAR ESCENAS
    public static bool saltarABotones = false;

    void Start()
    {
        // Si venimos desde la otra hitbox, saltamos directo al índice de los botones
        if (saltarABotones)
        {
            index = indexParaMostrarBotones;
            saltarABotones = false; // La apagamos para que la próxima vez inicie normal
        }
        else
        {
            index = 0; // Inicio normal
        }

        // Estado inicial de los paneles
        if (panelTresBotones != null)
        {
            panelTresBotones.SetActive(false);
        }
        if (objetoIndicaciones != null)
        {
            objetoIndicaciones.SetActive(true);
        }

        ShowSentence();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            NextSentence();
        }
    }

    void NextSentence()
    {
        if (index < sentences.Length - 1)
        {
            index++;
            ShowSentence();
        }
        else
        {
            Debug.Log("Diálogo finalizado. Cambiando de escena...");
            SceneManager.LoadScene("escena3");
        }
    }

    void ShowSentence()
    {
        if (sentences.Length > 0 && index < sentences.Length)
        {
            // 1. Muestra el texto actual
            textoDialogo.text = sentences[index];

            // 2. Cambia el fondo si hay sprites asignados
            if (fondoNidoImage != null && backgroundSprites != null && index < backgroundSprites.Length)
            {
                if (backgroundSprites[index] != null)
                {
                    fondoNidoImage.sprite = backgroundSprites[index];
                }
            }

            // 3. Controla los botones y las indicaciones según la oración actual
            if (index == indexParaMostrarBotones)
            {
                if (panelTresBotones != null) panelTresBotones.SetActive(true);     // Muestra los botones
                if (objetoIndicaciones != null) objetoIndicaciones.SetActive(false); // Oculta "indicaciones"
            }
            else
            {
                if (panelTresBotones != null) panelTresBotones.SetActive(false);    // Oculta los botones
                if (objetoIndicaciones != null) objetoIndicaciones.SetActive(true);  // Muestra "indicaciones" en el resto
            }
        }
    }

    public void StartDialogue()
    {
        index = 0;
        if (fondoDialogo != null)
        {
            fondoDialogo.SetActive(true);
        }
        ShowSentence();
    }
}