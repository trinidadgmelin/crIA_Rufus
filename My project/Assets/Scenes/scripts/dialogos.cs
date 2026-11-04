using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class dialogos : MonoBehaviour
{
    [Header("UI General")]
    public TextMeshProUGUI textoDialogo;
    public GameObject fondoDialogo;
    public GameObject objetoIndicaciones;
    public GameObject personajeDialogo;

    [Header("Imagen por Diálogo")]
    public Image imagenDialogoUI;
    public Sprite[] dialogSprites;

    [Header("Textos")]
    [TextArea(3, 10)]
    public string[] sentences;

    [Header("Cambio de Escena al Final")]
    public string escenaDestino = "escena5"; // Nombre de la escena a la que saltará al terminar

    private int index = 0;

    void Start()
    {
        index = 0;

        if (fondoDialogo != null)
        {
            fondoDialogo.SetActive(true);
        }

        if (personajeDialogo != null)
        {
            personajeDialogo.SetActive(true);
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
        // Si todavía quedan oraciones por delante, avanzamos
        if (index < sentences.Length - 1)
        {
            index++;
            ShowSentence();
        }
        else
        {
            // CAMBIO: Cuando se terminan los diálogos, cambia automáticamente de escena
            SceneManager.LoadScene(escenaDestino);
        }
    }

    void ShowSentence()
    {
        if (sentences.Length > 0 && index < sentences.Length)
        {
            // 1. Manejo del TEXTO, FONDO y PERSONAJE (Se ocultan en el índice 3 y vuelven en el 4)
            if (index == 3)
            {
                if (textoDialogo != null) textoDialogo.text = "";
                if (fondoDialogo != null) fondoDialogo.SetActive(false);
                if (personajeDialogo != null) personajeDialogo.SetActive(false);
            }
            else
            {
                if (fondoDialogo != null) fondoDialogo.SetActive(true);
                if (personajeDialogo != null) personajeDialogo.SetActive(true);

                if (string.IsNullOrEmpty(sentences[index]))
                {
                    textoDialogo.text = "";
                }
                else
                {
                    textoDialogo.text = sentences[index];
                }
            }

            // 2. Control de las INDICACIONES (Desaparece desde el segundo diálogo en adelante)
            if (objetoIndicaciones != null)
            {
                if (index >= 1)
                {
                    objetoIndicaciones.SetActive(false);
                }
                else
                {
                    objetoIndicaciones.SetActive(true);
                }
            }

            // 3. Manejo de la IMAGEN específica del diálogo
            if (imagenDialogoUI != null && dialogSprites != null && index < dialogSprites.Length)
            {
                if (dialogSprites[index] != null)
                {
                    imagenDialogoUI.gameObject.SetActive(true);
                    imagenDialogoUI.sprite = dialogSprites[index];

                    // Ajuste de tamaño dinámico según el índice
                    RectTransform rectTransform = imagenDialogoUI.GetComponent<RectTransform>();

                    if (index == 2)
                    {
                        rectTransform.sizeDelta = new Vector2(600f, 400f);
                    }
                    else if (index == 3)
                    {
                        rectTransform.sizeDelta = new Vector2(800f, 600f);
                    }
                    else
                    {
                        rectTransform.sizeDelta = new Vector2(400f, 400f);
                    }
                }
                else
                {
                    imagenDialogoUI.gameObject.SetActive(false);
                }
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
        if (personajeDialogo != null)
        {
            personajeDialogo.SetActive(true);
        }
        ShowSentence();
    }
}