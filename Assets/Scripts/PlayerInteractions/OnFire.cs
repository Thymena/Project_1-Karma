using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnFire : MonoBehaviour
{
    public const float KILL_TIME = 3f;
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
        if (other.CompareTag("Fire"))
        {
           timer += Time.deltaTime; 
            
            if (timer >= KILL_TIME)
            {
                playerHP = GetComponent<PlayerHP>();
                playerHP.health -= damage;
              
            }
        }
        
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Fire"))
        {
            timer = 0f;
        }
    }
}
