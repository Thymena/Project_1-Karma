using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mossberry : MonoBehaviour
{
    public Rigidbody2D rb; 
 	public AudioClip[] audioClips;
    private AudioSource audioSource;
	private bool playerInside = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.F))
        {
			audioSource = GetComponent<AudioSource>(); 
			audioSource.clip = audioClips[0];
        	audioSource.Play();
            Destroy(gameObject,0.3F);
        }
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
