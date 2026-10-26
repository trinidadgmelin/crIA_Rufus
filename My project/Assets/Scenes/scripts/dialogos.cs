using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI textoDialogo;
    public GameObject fondoDialogo;

    [Header("Textos del Diálogo")]
    [TextArea(3, 10)]
    public string[] sentences;

    private int index = 0;

    void Start()
    {
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
            textoDialogo.text = sentences[index];
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