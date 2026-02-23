using UnityEngine;

public class CaraDialogueManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Transform dialoguePanel;
    void Start()
    {
        dialoguePanel = GameObject.Find("Dialogue UI").transform;
        GameObject caraDialogue1 = dialoguePanel.Find("CaraDialogue1").gameObject;
        caraDialogue1.SetActive(true);
    }


    public void ShowDeathDialog()
    {
        GameObject caraDeathDialogue = dialoguePanel.Find("CaraDeathDialogue").gameObject;
        caraDeathDialogue.SetActive(true);
    }
}
