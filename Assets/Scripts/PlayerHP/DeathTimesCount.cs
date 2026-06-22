using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DeathTimesCounter : MonoBehaviour
{
    public static DeathTimesCounter Instance;

    [SerializeField] private TextMeshProUGUI deathText;
    private static int deathCount = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        UpdateUI(); // Force first update
    }

    public void AddDeath()
    {
        deathCount++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (deathText == null)
        {
            Debug.LogError("DeathText is NOT assigned in Inspector!");
            return;
        }

        deathText.text = $"Deaths: {deathCount}";
    }
}

