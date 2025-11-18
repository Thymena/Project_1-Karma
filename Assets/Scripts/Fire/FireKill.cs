using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;

public class FireScript : MonoBehaviour
{
    public const float KILL_TIME = 2f;
    public Transform respawnPoint;
    public PlayerHP playerHP;
    public float damage = 100;

    private float timer;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            timer += Time.deltaTime;
            
            if (timer >= KILL_TIME)
            {
                playerHP = other.GetComponent<PlayerHP>();
                playerHP.health -= damage;
                Debug.Log("Player Died");
              
            }
        }
        
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            timer = 0f;
        }
    }
}
