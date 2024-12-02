using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    private Queue<string> sentences;
    public Animator animator;
    AudioManager audioManager;
    public GameObject nextSceneButton;
    private CanvasGroup buttonCanvasGroup;  

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        buttonCanvasGroup = nextSceneButton.GetComponent<CanvasGroup>();
        buttonCanvasGroup.alpha = 0;
        buttonCanvasGroup.interactable = false;
        buttonCanvasGroup.blocksRaycasts = false;
    }
    void Start()
    {
        sentences = new Queue<string>();
    }
    public void StartDialogue ( Dialogue dialogue)
    {

        animator.SetBool("IsOpen", true);

        sentences.Clear();
        
        foreach(string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        audioManager.PlaySFX(audioManager.talking);
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }
    IEnumerator TypeSentence(string sentence)
    {
        int dialogueSpeed = 5;
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            for (int i = 0; i < dialogueSpeed; i++)
            {
                yield return null;
            }
        }
        audioManager.StopSFX();

    }
    public void EndDialogue()
    {
        //animator.SetBool("IsOpen", false);
        buttonCanvasGroup.alpha = 1;
        buttonCanvasGroup.interactable = true;
        buttonCanvasGroup.blocksRaycasts = true;
    }
    
}
