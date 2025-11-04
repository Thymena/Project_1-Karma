using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementRe : MonoBehaviour
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
    public float wallClimbSpeed = 3f;
    public float normalGravity = 3f;

    [Header("Wall Jump")]
    public float wallJumpHorizontal = 8f;  // horizontal push when wall-jumping

    private bool isTouchingWall;
    private bool isWallClinging;
    private bool facingRight = true;
    private float horizontalInput;
    private float verticalInput;
    private int wallDirection; // 1 = wall on right, -1 = wall on left

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
    public float edgeDetachCooldown = 1.0f; // 1 second cooldown after wall-jump
    private float lastEdgeDetachTime = -10f;

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

        // Update wall presence & direction (respects cooldown)
        CheckWall();

        // Ground jump as before
        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || isIced))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // ------------ Wall cling / detach / wall-jump logic ------------
        // Determine whether the player is intentionally pressing INTO the wall (to cling)
        bool pressingTowardWall = (wallDirection == 1 && horizontalInput > 0.1f)
                               || (wallDirection == -1 && horizontalInput < -0.1f);

        // Determine whether the player is pressing AWAY from the wall (either hold or key-down)
        bool pressingAwayHold = (wallDirection == 1 && horizontalInput < -0.1f)
                             || (wallDirection == -1 && horizontalInput > 0.1f);

        bool pressingAwayDown =
            (wallDirection == 1 && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)))
         || (wallDirection == -1 && (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)));

        // Only allow entering cling when touching a wall, not grounded, and player is pressing toward the wall
        if (isTouchingWall && !isGrounded && pressingTowardWall && (Time.time - lastEdgeDetachTime > edgeDetachCooldown))
        {
            isWallClinging = true;
        }
        else if (isGrounded || !pressingTowardWall)
        {
            // stop clinging if grounded or not pressing toward the wall
            isWallClinging = false;
        }

        // If currently clinging: check for detach or wall-jump
        if (isWallClinging)
        {
            // NEW: Wall-jump triggers on Space regardless of horizontal input.
            // This makes the player launch away diagonally even if still holding into-wall.
            if (Input.GetKeyDown(KeyCode.Space))
            {
                WallJump();
            }
            // If player presses away (hold or tap) without Space => detach naturally and fall
            else if (pressingAwayHold || pressingAwayDown)
            {
                DetachAndFall();
            }
        }

        // ------------ Animation flags (unchanged logic) ------------
        if (rb.velocity.y > 0 && (!isGrounded || !isIced))
        {
            isJumping = true;
            isFalling = false;
            isRunning = false;
            isCrouching = false;
            SetAnimatorState();
        }

        if (rb.velocity.y < 0 && (!isGrounded || !isIced))
        {
            isJumping = false;
            isFalling = true;
            isRunning = false;
            isCrouching = false;
            SetAnimatorState();
        }

        // Run animation
        if (!isJumping && (isGrounded || isIced) && Mathf.Abs(rb.velocity.x) > 3.5f)
        {
            isJumping = false;
            isFalling = false;
            isRunning = true;
            isCrouching = false;
            SetAnimatorState();
        }

        // Idle
        if (Mathf.Abs(rb.velocity.magnitude) <= 3.5f && (isGrounded || isIced))
        {
            isJumping = false;
            isFalling = false;
            isRunning = false;
            isCrouching = false;
            SetAnimatorState();
        }

        // Crouch
        if (Input.GetKey(KeyCode.R) && isGrounded)
        {
            isJumping = false;
            isFalling = false;
            isRunning = false;
            isCrouching = true;
            SetAnimatorState();
        }

        // Flip sprite based on movement
        if (!facingRight && horizontalInput > 0.1f)
            Flip();
        else if (facingRight && horizontalInput < -0.1f)
            Flip();

        SetAnimatorState();
    }

    void FixedUpdate()
    {
        if (isWallClinging)
        {
            HandleWallCling();
        }
        else
        {
            if (isIced && (isGrounded || !isGrounded))
                HandleIceMovement();
            else
                HandleNormalMovement();
        }
    }

    public void CheckWall()
    {
        // Respect cooldown after detaching/jumping
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

        // If wall disappears while clinging, treat like an edge detach (hop + cooldown)
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
        float targetX = horizontalInput * speed * currentSpeedMultiplier;
        rb.velocity = new Vector2(targetX, rb.velocity.y);
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
        if (!isTouchingWall)
        {
            DetachFromWall();
            return;
        }

        if (IsAtWallEdge())
        {
            DetachFromWall();
            return;
        }

        // Freeze gravity and allow controlled climb/slide while clinging
        rb.gravityScale = 0f;

        if (verticalInput > 0.1f)
            rb.velocity = new Vector2(0, wallClimbSpeed);
        else if (verticalInput < -0.1f)
            rb.velocity = new Vector2(0, -wallClimbSpeed);
        else
            rb.velocity = new Vector2(0, -wallSlideSpeed);
    }

    // Use collision contacts to detect ground/ice from below
    void OnCollisionStay2D(Collision2D collision)
    {
        bool contactBelow = false;
        foreach (ContactPoint2D cp in collision.contacts)
        {
            if (cp.normal.y > 0.5f)
            {
                contactBelow = true;
                break;
            }
        }

        if (collision.gameObject.CompareTag("Floor") && contactBelow)
            isGrounded = true;

        if (collision.gameObject.CompareTag("Ice"))
        {
            isIced = true;
            if (contactBelow) isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
            isGrounded = false;
        if (collision.gameObject.CompareTag("Ice"))
        {
            isIced = false;
            isGrounded = false;
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        if (spriteRenderer) spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    // Detach with hop/push (used when wall disappears or at edge)
    void DetachFromWall()
    {
        isWallClinging = false;
        rb.gravityScale = normalGravity;
        float pushX = facingRight ? -edgePushHorizontal : edgePushHorizontal; // push away from wall
        rb.velocity = new Vector2(pushX, edgeHopVelocity);
        lastEdgeDetachTime = Time.time;
    }

    // Detach naturally and fall (no hop/push), used when pressing away
    void DetachAndFall()
    {
        if (!isWallClinging) return;
        isWallClinging = false;
        rb.gravityScale = normalGravity;
        // keep velocity unchanged so physics handles natural fall
        lastEdgeDetachTime = Time.time; // still apply cooldown to prevent instant re-grab
    }

    // Diagonal wall-jump away from the wall.
    // IMPORTANT: This ignores current horizontal input so it will always launch opposite the wall
    // even if the player is still holding the key into the wall.
    void WallJump()
    {
        if (!isWallClinging) return;
        isWallClinging = false;
        rb.gravityScale = normalGravity;

        // wallDirection == 1 (wall on right) -> jump left (-1)
        float horizontalPush = -wallDirection * Mathf.Abs(wallJumpHorizontal);

        // apply diagonal velocity (away + upward)
        rb.velocity = new Vector2(horizontalPush, jumpForce);

        // prevent immediate re-cling
        lastEdgeDetachTime = Time.time;

        // animation
        isJumping = true;
        isFalling = false;
        SetAnimatorState();
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