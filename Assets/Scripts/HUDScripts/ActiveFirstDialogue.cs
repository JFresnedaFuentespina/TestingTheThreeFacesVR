using System.Collections;
using UnityEngine;

public class ActiveFirstDialogue : MonoBehaviour
{
    public bool waitOneFrame = false;
    public bool hasBeenActivated = false;
    public GameObject dialoguePanel;

    void Start()
    {
        if (hasBeenActivated) return;

        hasBeenActivated = true;

        if (waitOneFrame)
        {
            StartCoroutine(ActivateAfterDelay());
        }
        else
        {
            dialoguePanel.SetActive(true);
        }
    }

    IEnumerator ActivateAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        dialoguePanel.SetActive(true);
    }
}