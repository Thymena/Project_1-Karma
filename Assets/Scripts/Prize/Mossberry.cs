using System.Collections;
using UnityEngine;

public class Mossberry : MonoBehaviour
{
    public CollectibleManager manager;
    private AudioSource audioSource;
    private bool playerInside = false;

    public GameObject berry;
    public AudioClip[] audioClips;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.F))
        {
            audioSource.clip = audioClips[0];
            audioSource.Play();

            manager.OnCollectiblePicked();

            // Start coroutine to delay disabling the object
            StartCoroutine(DisableBerryWithDelay(0.3f));
        }
    }

    private IEnumerator DisableBerryWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        berry.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}