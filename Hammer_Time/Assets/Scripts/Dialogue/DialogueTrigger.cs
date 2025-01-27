using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue[] dialogue;

    public Dialogue[] qualDialogue;
    public Dialogue[] reviewDialogue;
    public Dialogue[] introDialogue;
    public Dialogue[] storyDialogue;
    public Dialogue[] helpDialogue;
    public Dialogue[] strategyDialogue;

    public GameObject talkingHead;

    void Awake()
    {

    }
    public void TriggerDialogue(string dialogueType, int index)
    {
        switch (dialogueType)
        {
            case "Qualifiers":
                FindObjectOfType<DialogueManager>().StartDialogue(qualDialogue[index]);
                break;
            case "Review":
                FindObjectOfType<DialogueManager>().StartDialogue(reviewDialogue[index]);
                break;
            case "Intro":
                FindObjectOfType<DialogueManager>().StartDialogue(introDialogue[index]);
                break;
            case "Story":
                FindObjectOfType<DialogueManager>().StartDialogue(storyDialogue[index]);
                break;
            case "Help":
                FindObjectOfType<DialogueManager>().StartDialogue(helpDialogue[index]);
                break;
            case "Strategy":
                FindObjectOfType<DialogueManager>().StartDialogue(strategyDialogue[index]);
                break;
        }
        //FindObjectOfType<DialogueManager>().StartDialogue(dialogue[index]);
        talkingHead.SetActive(true);
    }
}
