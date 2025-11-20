using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelElevator: MonoBehaviour
{
    public float riseSpeed = 3f;
    public float returnSpeed = 3f;
    public float changeTime = 1f;
    public Transform movedObject;
    
    private float timer;
    private float timerDelay;
    private Vector3 startPos;
    
    // Start is called before the first frame update
    void Start()
    {
        startPos = movedObject.position;
    }

    void Update()
    {
        
    }
    
    

    // 玩家不掉下去
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            timerDelay += Time.deltaTime;
            
            if (timerDelay > 1f)
            {
                timer += Time.deltaTime; 
                //collision.transform.SetParent(transform);
                movedObject.position += Vector3.up * riseSpeed * Time.deltaTime;
            
                if (timer >= changeTime)
                {
                    SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
                }
            }
            
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            timer = 0;
            //collision.transform.SetParent(null);
            Vector3 newPos = Vector3.MoveTowards(movedObject.position, startPos, returnSpeed * Time.deltaTime);
            transform.position = newPos;
        }
    }
}
