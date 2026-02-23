using TMPro;
using UnityEngine;

public class ClassificationItemUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text scoreText;

    public void Setup(string playerName, float score)
    {
        nameText.text = playerName;
        scoreText.text = score.ToString();
    }
}
