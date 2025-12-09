using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement: MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 7f;
    public float jumpForce = 15f;
    public Rigidbody2D rb;
    public bool isGrounded = true;
    public bool isIced = false;

    [Header("Ice Settings (slippery)")]
    public float iceAcceleration = 30f;
    public float iceCoastingFriction = 1.5f;
    public float iceMaxSpeed = 10f;
    public float iceStopThreshold = 0.05f;

    [Header("Wall Settings")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.6f;
    public LayerMask wallLayer;
    public float wallSlideSpeed = 2f;
    public float wallClimbSpeed = 3f; // (unused now but safe to keep)
    public float normalGravity = 3f;

    [Header("Wall Jump")]
    public float wallJumpHorizontal = 8f;  

    private bool isTouchingWall;
    private bool isWallClinging;
    private bool facingRight = true;
    private float horizontalInput;
    private float verticalInput;
    private int wallDirection; 

    [Header("Animation States")]
    public Animator animator;
    public bool isRunning;
    public bool isJumping;
    public bool isFalling;
    public bool isCrouching;

    [HideInInspector]
    public bool IsRecovering = false;
    private float currentSpeedMultiplier = 1f;

    private SpriteRenderer spriteRenderer;

    public float edgeHopVelocity = 3f;
    public float edgePushHorizontal = 0.8f;
    public float edgeDetachCooldown = 1.0f; 
    private float lastEdgeDetachTime = -10f;

    // Wall stick delay
    private float wallStickTime = 0.3f; 
    private float wallStickTimer = 0f;  
    private bool isWallSliding = false;
    private bool justTouchedWall = false; 

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

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || isIced))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        bool pressingTowardWall = (wallDirection == 1 && horizontalInput > 0.1f)
                               || (wallDirection == -1 && horizontalInput < -0.1f);

        bool pressingAwayHold = (wallDirection == 1 && horizontalInput < -0.1f)
                             || (wallDirection == -1 && horizontalInput > 0.1f);

        bool pressingAwayDown = (wallDirection == 1 && Input.GetKeyDown(KeyCode.A))
                             || (wallDirection == -1 && Input.GetKeyDown(KeyCode.D));

        // Enter cling
        if (isTouchingWall && !isGrounded && pressingTowardWall && (Time.time - lastEdgeDetachTime > edgeDetachCooldown))
        {
            if (!isWallClinging)
            {
                isWallClinging = true;
                justTouchedWall = true;
            }

            if (justTouchedWall)
            {
                wallStickTimer = wallStickTime;
                justTouchedWall = false;
            }
        }
        else
        {
            isWallClinging = false;
            isWallSliding = false;
        }

        // Handle wall jump & detach
        if (isWallClinging)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                WallJump();
            }
            else if (pressingAwayHold || pressingAwayDown)
            {
                DetachAndFall();
            }
        }

        UpdateAnimatorStates();

        // Flip
        if (!facingRight && horizontalInput > 0.1f)
            Flip();
        else if (facingRight && horizontalInput < -0.1f)
            Flip();
    }

    void FixedUpdate()
    {
        if (isWallClinging)
        {
            HandleWallCling();
        }
        else
        {
            if (isIced)
                HandleIceMovement();
            else
                HandleNormalMovement();
        }
    }

    void CheckWall()
    {
        if (Time.time - lastEdgeDetachTime < edgeDetachCooldown)
        {
            isTouchingWall = false;
            return;
        }

        Vector2 origin = wallCheck != null ? (Vector2)wallCheck.position : (Vector2)transform.position;
        RaycastHit2D hitRight = Physics2D.Raycast(origin, Vector2.right, wallCheckDistance, wallLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(origin, Vector2.left, wallCheckDistance, wallLayer);

        bool wasTouching = isTouchingWall;

        if (hitRight.collider != null)
        {
            isTouchingWall = true;
            wallDirection = 1;
        }
        else if (hitLeft.collider != null)
        {
            isTouchingWall = true;
            wallDirection = -1;
        }
        else
        {
            isTouchingWall = false;
        }

        if (!isTouchingWall && wasTouching && isWallClinging)
        {
            DetachFromWall();
        }
    }

    bool IsAtWallEdge()
    {
        Vector2 dir = (wallDirection == 1) ? Vector2.right : Vector2.left;
        Vector2 lowOrigin = wallCheck != null ? (Vector2)wallCheck.position : (Vector2)transform.position;
        Vector2 highOrigin = lowOrigin + Vector2.up * 0.6f;

        RaycastHit2D wallLow = Physics2D.Raycast(lowOrigin, dir, wallCheckDistance, wallLayer);
        RaycastHit2D wallHigh = Physics2D.Raycast(highOrigin, dir, wallCheckDistance, wallLayer);

        return (wallLow.collider != null) && (wallHigh.collider == null);
    }

    void HandleNormalMovement()
    {
        rb.gravityScale = normalGravity;
        rb.velocity = new Vector2(horizontalInput * speed * currentSpeedMultiplier, rb.velocity.y);
    }

    void HandleIceMovement()
    {
        rb.gravityScale = normalGravity;

        float currentX = rb.velocity.x;
        float absInput = Mathf.Abs(horizontalInput);

        if (absInput > 0.01f)
        {
            float targetSpeed = horizontalInput * iceMaxSpeed * currentSpeedMultiplier;
            float newX = Mathf.MoveTowards(currentX, targetSpeed, iceAcceleration * Time.fixedDeltaTime);
            rb.velocity = new Vector2(newX, rb.velocity.y);
        }
        else
        {
            float damping = Mathf.Exp(-iceCoastingFriction * Time.fixedDeltaTime);
            float newX = currentX * damping;

            if (Mathf.Abs(newX) < iceStopThreshold)
                newX = 0f;

            rb.velocity = new Vector2(newX, rb.velocity.y);
        }

        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -iceMaxSpeed, iceMaxSpeed), rb.velocity.y);
    }

    void HandleWallCling()
    {
        if (!isTouchingWall || IsAtWallEdge())
        {
            DetachFromWall();
            return;
        }

        rb.gravityScale = 0f;

        // ==== WALL CLIMB REMOVED ====

        // Stick timer
        if (wallStickTimer > 0f)
        {
            wallStickTimer -= Time.fixedDeltaTime;
            rb.velocity = new Vector2(0, 0);
            isWallSliding = false;
            return;
        }

        // Auto wall slide
        rb.velocity = new Vector2(0, -wallSlideSpeed);
        isWallSliding = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Floor"))
            isGrounded = true;
        if (other.gameObject.CompareTag("Ice"))
            isIced = true;
        if (other.gameObject.CompareTag("SpikeBack"))
            isGrounded = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Floor"))
            isGrounded = false;
        if (other.gameObject.CompareTag("Ice"))
            isIced = false;
        if (other.gameObject.CompareTag("SpikeBack"))
            isGrounded = false;
    }

    void Flip()
    {
        facingRight = !facingRight;
        if (spriteRenderer) spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    void DetachFromWall()
    {
        isWallClinging = false;
        isWallSliding = false;
        wallStickTimer = 0f;
        rb.gravityScale = normalGravity;
        float pushX = facingRight ? -edgePushHorizontal : edgePushHorizontal;
        rb.velocity = new Vector2(pushX, edgeHopVelocity);
        lastEdgeDetachTime = Time.time;
    }

    void DetachAndFall()
    {
        if (!isWallClinging) return;
        isWallClinging = false;
        isWallSliding = false;
        wallStickTimer = 0f;
        rb.gravityScale = normalGravity;
        lastEdgeDetachTime = Time.time;
    }

    void WallJump()
    {
        if (!isWallClinging) return;
        isWallClinging = false;
        isWallSliding = false;
        wallStickTimer = 0f;
        rb.gravityScale = normalGravity;

        float horizontalPush = -wallDirection * Mathf.Abs(wallJumpHorizontal);
        rb.velocity = new Vector2(horizontalPush, jumpForce);

        lastEdgeDetachTime = Time.time;
    }

    void UpdateAnimatorStates()
    {
        if (rb.velocity.y > 0 && !(isGrounded || isIced))
        {
            isJumping = true; isFalling = false; isRunning = false; isCrouching = false;
            SetAnimatorState();
        }
        else if (rb.velocity.y < 0 && (!isGrounded || !isIced))
        {
            isJumping = false; isFalling = true; isRunning = false; isCrouching = false;
            SetAnimatorState();
        }

        if ((isGrounded || isIced) && Mathf.Abs(rb.velocity.x) > 3.5f)
        {
            isJumping = false; isFalling = false; isRunning = true; isCrouching = false;
            SetAnimatorState();
        }

        if (Mathf.Abs(rb.velocity.x) <= 3.5f && (isGrounded || isIced))
        {
            isJumping = false; isFalling = false; isRunning = false; isCrouching = false;
            SetAnimatorState();
        }

        if (Input.GetKey(KeyCode.R) && isGrounded)
        {
            isJumping = false; isFalling = false; isRunning = false; isCrouching = true;
            SetAnimatorState();
        }
    }

    public void SetAnimatorState()
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
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * wallCheckDistance);
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.left * wallCheckDistance);
        }
    }
}
