using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] audioClips;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
		audioSource.clip = audioClips[0];
        audioSource.Play();
        //audioSource.clip = audioClips[0];
        //audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
       
	}
}
