using UnityEngine;

public class CantoDialogueManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Transform dialoguePanel;
    void Start()
    {
        dialoguePanel = GameObject.Find("Dialogue UI").transform;
        GameObject cantoDialogue1 = dialoguePanel.Find("CantoDialogue1").gameObject;
        cantoDialogue1.SetActive(true);
    }


    public void ShowDeathDialog()
    {
        GameObject cantoDeathDialogue = dialoguePanel.Find("CantoDeathDialogue1").gameObject;
        cantoDeathDialogue.SetActive(true);
    }
}
