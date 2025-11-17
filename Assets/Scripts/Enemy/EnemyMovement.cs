using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform pointA;   
    public Transform pointB;  
    public float movespeed = 3f;  

    private Vector3 startPos;
    private Vector3 endPos;
    private bool facingRight = true;

    void Start()
    {
        startPos = pointA.position;
        endPos = pointB.position;
    }

    void Update()
    {
        float t = Mathf.PingPong(Time.time * movespeed, 1f);
        Vector3 newPos = Vector3.Lerp(startPos, endPos, t);

        
        if (newPos.x > transform.position.x && !facingRight)
        {
            Flip();
        }
        else if (newPos.x < transform.position.x && facingRight)
        {
            Flip();
        }

        transform.position = newPos;
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("你被张启睿拱了"); 
            UnityEditor.EditorApplication.isPlaying = false;    
        }
        
    }
}