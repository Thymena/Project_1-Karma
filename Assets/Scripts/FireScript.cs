using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FireScript : MonoBehaviour
{
    public const float KILL_TIME = 2f;
    public GameObject player;
    public Transform respawnPoint;

    private float timer = 0f;
    private int healthFire = 100; 
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log(other.name);
        if (other.CompareTag("Player"))
        {
            timer += Time.deltaTime;
            
            
            if (timer >= KILL_TIME)
            {
                healthFire = (int)(100 - timer * 50);
                if ((healthFire = 0) != 0)
                {
                    Debug.Log("Player Died");
                    SceneManager.LoadScene("Layer0");
                }
            }
        }
        
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            timer = 0f; 
            healthFire = 100;
        }
    }
}
