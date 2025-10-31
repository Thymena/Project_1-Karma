using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestClimbs : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 7f;
    public float jumpForce = 15f;
    public Rigidbody2D rb;
    public bool isGrounded = true;

    [Header("Wall Settings")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.6f;
    public LayerMask wallLayer;
    public float wallSlideSpeed = 2f;
    public float wallClimbSpeed = 3f;
    public float normalGravity = 3f;

    private bool isTouchingWall;
    private bool isWallClinging;
    private bool facingRight = true;
    private float horizontalInput;
    private float verticalInput;

    [Header("Animation States")]
    public Animator animator;
    public bool isRunning;
    public bool isJumping;
    public bool isFalling;
    public bool isCrouching;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        CheckWall();

        // jump & fall
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

        // --- RUNNING ---
        if (Mathf.Abs(rb.velocity.x) > 0 && isGrounded)
        {
            isJumping = false; 
            isFalling = false; 
            isRunning = true; 
            isCrouching = false;
            SetAnimatorState();
        }

        // --- IDLE ---
        if (Mathf.Abs(rb.velocity.x) == 0 && isGrounded)
        {
            isJumping = false; 
            isFalling = false; 
            isRunning = false; 
            isCrouching = false;
            SetAnimatorState();
        }

        // --- CROUCH ---
        if (Input.GetKey(KeyCode.R) && isGrounded)
        {
            isJumping = false; 
            isFalling = false; 
            isRunning = false; 
            isCrouching = true;
            SetAnimatorState();
        }

        

        //flip
        if (facingRight == false && horizontalInput > 0)
        {
            Flip();
        }
        else if (facingRight == true && horizontalInput < 0)
        {
            Flip();
        }
        
        // 攀墙
        if (isTouchingWall && !isGrounded && Mathf.Abs(horizontalInput) > 0.1f)
        {
            isWallClinging = true;
        }
        else if (isGrounded || Mathf.Abs(horizontalInput) < 0.1f)
        {
            isWallClinging = false;
        }
    }

    void FixedUpdate()
    {
        if (isWallClinging)
        {
            HandleWallCling();
        }
        else
        {
            HandleNormalMovement();
        }
    }

    void HandleNormalMovement()
    {
        rb.gravityScale = normalGravity;
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
    }

    void HandleWallCling()
    {
        rb.gravityScale = 0f;

        // Move up or down the wall
        if (verticalInput > 0.1f)
        {
            rb.velocity = new Vector2(0, wallClimbSpeed);
        }
        else if (verticalInput < -0.1f)
        {
            rb.velocity = new Vector2(0, -wallClimbSpeed);
        }
        else
        {
            // Slide slowly down if no input
            rb.velocity = new Vector2(0, -wallSlideSpeed);
        }
    }

    void CheckWall()
    {
        Vector2 checkDir = facingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, checkDir, wallCheckDistance, wallLayer);
        isTouchingWall = hit.collider != null;
    }

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

    void Flip()
    {
        facingRight = !facingRight;
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    void SetAnimatorState()
    {
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsFalling", isFalling);
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsCrouching", isCrouching);
    }

    void OnDrawGizmosSelected()
    {
        if (wallCheck != null)
        {
            Gizmos.color = Color.cyan;
            Vector2 checkDir = facingRight ? Vector2.right : Vector2.left;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3)checkDir * wallCheckDistance);
        }
    }
}