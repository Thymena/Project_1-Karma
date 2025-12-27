using UnityEngine;

public class PlayerMovementRe : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 7f;
    public float jumpForce = 15f;
    public float normalGravity = 3f;

    [Header("Ice")]
    public bool isIced;
    public float iceAcceleration = 30f;
    public float iceCoastingFriction = 0.6f;
    public float iceMaxSpeed = 11f;
    public float iceStopThreshold = 0.02f;

    [Header("Wall")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.6f;
    public LayerMask wallLayer;
    public float wallSlideSpeed = 2f;
    public float wallJumpHorizontal = 8f;

    [Header("Ledge")]
    public float edgeHopVelocity = 3f;
    public float edgePushHorizontal = 0.8f;
    public float edgeDetachCooldown = 1f;
    public float wallStickTime = 0.3f;

    [Header("Coyote Time")]
    public float groundCoyoteTime = 0.12f;
    public float wallCoyoteTime = 0.12f;

    [Header("Animation")]
    public Animator animator;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    private float horizontal;
    private bool facingRight = true;

    private bool isGrounded;
    private bool isWallClinging;
    private bool isTouchingWall;
    private int wallDirection;

    private float wallStickTimer;
    private float lastEdgeDetachTime = -10f;
    private float lastGroundedTime = -10f;
    private float lastWallContactTime = -10f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // move input
        horizontal = Input.GetAxisRaw("Horizontal");

        CheckWall();
        HandleJumpInput();
        HandleWallClingInput();
        HandleFlip();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        rb.gravityScale = normalGravity;

        // ledge detach even if cling already dropped
        if (isTouchingWall && IsAtWallEdge())
        {
            EdgeDetach();
            return;
        }

        if (isWallClinging)
        {
            HandleWallCling();
        }
        else if (isIced)
        {
            HandleIceMovement();
        }
        else
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
    }


    void HandleJumpInput()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;

        if (isGrounded || Time.time - lastGroundedTime <= groundCoyoteTime)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            lastGroundedTime = -10f;
        }
        else if (isWallClinging || Time.time - lastWallContactTime <= wallCoyoteTime)
        {
            WallJump();
        }
    }

    void HandleWallClingInput()
    {
        bool pressingTowardWall = (wallDirection == 1 && horizontal > 0.1f) || (wallDirection == -1 && horizontal < -0.1f);

        if (isTouchingWall && !isGrounded && pressingTowardWall && Time.time - lastEdgeDetachTime > edgeDetachCooldown)
        {
            if (!isWallClinging)
            {
                isWallClinging = true;
                wallStickTimer = wallStickTime;
            }
        }
        else
        {
            isWallClinging = false;
        }
    }

    // checks wall direction and wall presence
    void CheckWall()
    {
        if (Time.time - lastEdgeDetachTime < edgeDetachCooldown)
        {
            isTouchingWall = false;
            return;
        }

        Vector2 origin = wallCheck ? wallCheck.position : transform.position;

        RaycastHit2D right = Physics2D.Raycast(origin, Vector2.right, wallCheckDistance, wallLayer);
        RaycastHit2D left  = Physics2D.Raycast(origin, Vector2.left,  wallCheckDistance, wallLayer);

        if (right.collider)
        {
            isTouchingWall = true;
            wallDirection = 1;
            lastWallContactTime = Time.time;
        }
        else if (left.collider)
        {
            isTouchingWall = true;
            wallDirection = -1;
            lastWallContactTime = Time.time;
        }
        else
        {
            isTouchingWall = false;
        }
    }

    void HandleWallCling()
    {
        // Ledge hop 
        if (IsAtUpperLedge())
        {
            EdgeDetach();
            return;
        }

        // Lost wall contact for fall
        if (!isTouchingWall)
        {
            isWallClinging = false;
            return;
        }

        // Stick delay
        if (wallStickTimer > 0f)
        {
            wallStickTimer -= Time.fixedDeltaTime;
            rb.velocity = Vector2.zero;
            return;
        }

        // Slide
        rb.velocity = new Vector2(0f, -wallSlideSpeed);
    }

    // Checking if is at wall edge for fall
    bool IsAtWallEdge()
    {
        Vector2 dir = wallDirection == 1 ? Vector2.right : Vector2.left;

        Vector2 low = wallCheck.position;
        Vector2 high = low + Vector2.up * 0.7f;

        bool lowHit = Physics2D.Raycast(low, dir, wallCheckDistance, wallLayer);
        bool highHit = Physics2D.Raycast(high, dir, wallCheckDistance, wallLayer);

        return lowHit && !highHit && rb.velocity.y >= -0.1f;
    }
    
    // Checking if is at ledge for ledge jump
    bool IsAtUpperLedge()
    {
        Vector2 dir = wallDirection == 1 ? Vector2.right : Vector2.left;

        Vector2 low  = wallCheck.position;
        Vector2 high = low + Vector2.up * 0.7f;

        bool lowHit = Physics2D.Raycast(low,  dir, wallCheckDistance, wallLayer);
        bool highHit = Physics2D.Raycast(high, dir, wallCheckDistance, wallLayer);

        return lowHit && !highHit && rb.velocity.y >= -0.05f;
    }
    
    void WallJump()
    {
        isWallClinging = false;
        rb.velocity = new Vector2(-wallDirection * wallJumpHorizontal, jumpForce);
        lastEdgeDetachTime = Time.time;
    }

    void EdgeDetach()
    {
        isWallClinging = false;
        wallStickTimer = 0f;

        rb.velocity = new Vector2(-wallDirection * edgePushHorizontal, Mathf.Max(rb.velocity.y, edgeHopVelocity));

        lastEdgeDetachTime = Time.time;
    }

    void HandleIceMovement()
    {
        rb.gravityScale = normalGravity;

        float vx = rb.velocity.x;

        // slow acceleration
        if (Mathf.Abs(horizontal) > 0.01f)
        {
            float target = horizontal * iceMaxSpeed;
            vx = Mathf.MoveTowards(vx, target, iceAcceleration * 0.6f * Time.fixedDeltaTime);
        }
        // slow decay when no input
        else
        {
            float friction = Mathf.Exp(-iceCoastingFriction * 0.35f * Time.fixedDeltaTime);
            vx *= friction;

            // Only stop when almost completely still
            if (Mathf.Abs(vx) < iceStopThreshold)
                vx = 0f;
        }

        rb.velocity = new Vector2(
            Mathf.Clamp(vx, -iceMaxSpeed, iceMaxSpeed),
            rb.velocity.y
        );
    }

    // tests for floors or ice
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Floor") || other.CompareTag("SpikeBack"))
        {
            isGrounded = true;
            lastGroundedTime = Time.time;
        }
        else if (other.CompareTag("Ice"))
        {
            isIced = true;
            lastGroundedTime = Time.time;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Floor") || other.CompareTag("SpikeBack"))
            isGrounded = false;
        else if (other.CompareTag("Ice"))
            isIced = false;
    }
    
    void HandleFlip()
    {
        if ((facingRight && horizontal < -0.1f) || (!facingRight && horizontal > 0.1f))
        {
            facingRight = !facingRight;
            sprite.flipX = !sprite.flipX;
        }
    }

    //updates animator
    void UpdateAnimator()
    {
        animator.SetBool("IsJumping", rb.velocity.y > 0 && !isGrounded && !isIced);
        animator.SetBool("IsFalling", rb.velocity.y < 0 && !isGrounded && !isIced);
        animator.SetBool("IsRunning", Mathf.Abs(rb.velocity.x) > 3.5f && (isGrounded || isIced));
        animator.SetBool("IsCrouching", Input.GetKey(KeyCode.R) && isGrounded);
    }
}