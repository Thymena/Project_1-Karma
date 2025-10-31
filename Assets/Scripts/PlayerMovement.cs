using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 7f;
    public float jumpForce = 15f;
    public Rigidbody2D rb;
    public bool isGrounded = true;
    public Animator animator;
    public bool isRunning;
    public bool isJumping;
    public bool isFalling;
    public bool isCrouching;

    private SpriteRenderer spriteRenderer;
    private bool facingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //jump and fall
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.y, jumpForce);
        }

        if (rb.velocity.y > 0 && !isGrounded)
        {
            isJumping = true;
            isFalling = false;
            isRunning = false;
            isCrouching = false;
            SetAnimatorState();
        }

        if (rb.velocity.y < 0 && !isGrounded)
        {
            isJumping = false;
            isFalling = true;
            isRunning = false;
            isCrouching = false;
            SetAnimatorState();
        }

        //移动
        Vector2 playerInput = Vector2.zero;

        playerInput.x = Input.GetAxis("Horizontal");
        //transform.Translate(Vector3.right * playerInput.x * speed * Time.deltaTime);
        rb.velocity = new Vector2(playerInput.x * speed, rb.velocity.y);

        //当方向不同改变人物方向
        if (facingRight == false && playerInput.x > 0)
        {
            Flip();
        }
        else if (facingRight == true && playerInput.x < 0)
        {
            Flip();
        }

        //Run animation
        if (Mathf.Abs(rb.velocity.x) > 0 && isGrounded && !isJumping)
        {
            isJumping = false;
            isFalling = false;
            isRunning = true;
            isCrouching = false;
            SetAnimatorState();
        }
        
        //idle
        if (Mathf.Abs(rb.velocity.magnitude) == 0 && isGrounded)
        {
            isJumping = false;
            isFalling = false;
            isRunning = false;
            isCrouching = false;
            SetAnimatorState();
        }

        //crouching
        if (Input.GetKey(KeyCode.R) && isGrounded)
        {
            isJumping = false;
            isFalling = false;
            isRunning = false;
            isCrouching = true;
            SetAnimatorState();
        }

        
    }
    
 
    //跳跃限制：测试是否在地面
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = false;
        }
    }





    //set the animator
    void SetAnimatorState()
    {
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsFalling", isFalling);
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsCrouching", isCrouching);
    }

    //flip character
    void Flip()
    {
        facingRight = !facingRight;
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }
}

