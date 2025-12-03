using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    
    public GameObject player;
    public PlayerHP playerHP;

    public CollectibleManager collectibleManager;
    public float damage = 100f;

    private void Start()
    {
        // Subscribe to manager timer updates
        collectibleManager.OnTimerChanged += UpdateTimerUI;

        // Initialize UI immediately
        UpdateTimerUI(collectibleManager.CurrentTimer);
    }

    private void UpdateTimerUI(float time)
    {
        // Apply damage when timer hits zero or below
        if (time <= 0f)
        {
            if (playerHP == null)
                playerHP = player.GetComponent<PlayerHP>();

            playerHP.health -= damage;
        }

        // Display timer (MM:SS)
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        text.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void OnDestroy()
    {
        // Prevent errors when exiting scene
        if (collectibleManager != null)
            collectibleManager.OnTimerChanged -= UpdateTimerUI;
    }
}

