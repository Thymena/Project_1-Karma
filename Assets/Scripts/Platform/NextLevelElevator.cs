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
    private Vector2 startPos;
    private Rigidbody2D movedRigidbody;
    
    // Start is called before the first frame update
    void Start()
    {
        if (movedObject == null)
        {
            movedObject = transform;
        }
        
        movedRigidbody = movedObject.GetComponent<Rigidbody2D>();
        
        if (movedRigidbody == null)
        {
            Debug.LogError("NextLevelElevator requires a Rigidbody2D on the moved object.", this);
            enabled = false;
            return;
        }
        
        movedRigidbody.isKinematic = true;
        startPos = movedRigidbody.position;
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
            
            if (timerDelay > 0.5f)
            {
                timer += Time.deltaTime; 
                //collision.transform.SetParent(transform);
                Vector2 nextPosition = movedRigidbody.position + Vector2.up * riseSpeed * Time.deltaTime;
                movedRigidbody.MovePosition(nextPosition);
            
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
            timerDelay = 0;
            //collision.transform.SetParent(null);
            Vector2 newPos = Vector2.MoveTowards(movedRigidbody.position, startPos, returnSpeed * Time.deltaTime);
            movedRigidbody.MovePosition(newPos);
        }
    }
}
