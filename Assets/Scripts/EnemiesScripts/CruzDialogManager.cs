using UnityEngine;

public class CruzDialogManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Transform dialoguePanel;
    void Start()
    {
        dialoguePanel = GameObject.Find("Dialogue UI").transform;
        GameObject cruzDialogue1 = dialoguePanel.Find("CruzDialogue1").gameObject;
        cruzDialogue1.SetActive(true);
    }

    public void ShowDeathDialog()
    {
        GameObject cruzDeathDialogue = dialoguePanel.Find("CruzDeathDialogue").gameObject;
        cruzDeathDialogue.SetActive(true);
    }
}
