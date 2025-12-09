using System.Collections;
using UnityEngine;

public class Mossberry : MonoBehaviour
{
    public CollectibleManager manager;
    private AudioSource audioSource;
    private bool playerInside = false;
    private bool isCollecting = false; // prevents double triggers

    public GameObject berry;
    public AudioClip[] audioClips;

    // Delay before registering the collect (seconds)
    public float collectDelay = 0.3f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (playerInside && !isCollecting)
        {
            // Start the audio + collect coroutine BEFORE telling manager
            StartCoroutine(PlayAndCollect());
        }
    }

    private IEnumerator PlayAndCollect()
    {
        isCollecting = true;

        if (audioClips != null && audioClips.Length > 0)
        {
            audioSource.PlayOneShot(audioClips[0]);
        }

        // Wait for audio to play
        yield return new WaitForSeconds(collectDelay);

        // Tell the manager AFTER audio/delay — manager can now safely disable this berry
        manager.OnCollectiblePicked();
        
        isCollecting = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }
}