using UnityEngine;
using TMPro;

public class EndSceneUI : MonoBehaviour
{
    public TextMeshProUGUI collectedText;

    void Start()
    {
        // Display the last round's count
        collectedText.text = "" + GameManager.Instance.lastRoundCollected;
    }
}