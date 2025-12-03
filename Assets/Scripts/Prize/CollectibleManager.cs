using System;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    public GameObject[] collectibles;
    public float baseTimer = 90f;

    public float CurrentTimer { get; private set; }
    public int CollectedCount { get; private set; }
    
    public event Action<int> OnCountChanged;
    public GameController gameController;
    
    private bool hasStartedSpawning = false;


    // UI
    public event Action<float> OnTimerChanged;
    private GameObject currentActiveCollectible;

    void Start()
    {
        // Disable all collectibles at the start
        foreach (GameObject obj in collectibles)
            obj.SetActive(false);

        CurrentTimer = baseTimer;

        // Do NOT spawn here — wait until external script tells you to start
        hasStartedSpawning = false;  
    }


    void Update()
    {
        if (currentActiveCollectible == null)
            return;

        // Countdown
        CurrentTimer -= Time.deltaTime;

        // Notify UI
        OnTimerChanged?.Invoke(CurrentTimer);

        // Timer expired
        if (CurrentTimer <= 0f)
        {
            currentActiveCollectible.SetActive(false);
            SpawnRandomCollectible();
        }
        
        //Enough amount of count collected
        if (CollectedCount == 5)
        {
            gameController.pastTest = true;
            foreach (GameObject obj in collectibles)
                obj.SetActive(false);
        }
    }

    public void OnCollectiblePicked()
    {
        if (gameController.startTest == true)
        {
            CollectedCount++;
        
            OnCountChanged?.Invoke(CollectedCount);

            // Disable current collectible
            if (currentActiveCollectible != null)
            {
                currentActiveCollectible.SetActive(false);
            }
        
            SpawnRandomCollectible();
        }
        
    }

    private void SpawnRandomCollectible()
    {
        CurrentTimer = baseTimer;
        OnTimerChanged?.Invoke(CurrentTimer);

        if (collectibles.Length == 0)
            return;

        int newIndex;

        if (currentActiveCollectible == null)
        {
            newIndex = UnityEngine.Random.Range(0, collectibles.Length);
        }
        else
        {
            int lastIndex = Array.IndexOf(collectibles, currentActiveCollectible);
            do
            {
                newIndex = UnityEngine.Random.Range(0, collectibles.Length);
            }
            while (newIndex == lastIndex);
        }

        currentActiveCollectible = collectibles[newIndex];
        currentActiveCollectible.SetActive(true);
    }

    
    public void BeginSpawning()
    {
        if (!hasStartedSpawning)
        {
            hasStartedSpawning = true;
            SpawnRandomCollectible();
        }
    }


}
