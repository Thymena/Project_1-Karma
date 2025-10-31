using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    
    public Transform pointA; 
    public Transform pointB; 
    public float moveSpeed = 2f; 

    private Vector3 startPos;

    private Vector3 endPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = pointA.position;
        endPos = pointB.position;
    }

    void Update()
    {
        float t = Mathf.PingPong(Time.time * moveSpeed, 1f);
        transform.position = Vector3.Lerp(startPos, endPos, t);
    }

    // 玩家不掉下去
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
