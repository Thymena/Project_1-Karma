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
    private int wallDirection; 

    [Header("Animation States")]
    public Animator animator;
    public bool isRunning;
    public bool isJumping;
    public bool isFalling;
    public bool isCrouching;

    private SpriteRenderer spriteRenderer;
    
    [HideInInspector] public bool IsRecovering = false;
    private float currentSpeedMultiplier = 1f;

    public IEnumerator RecoverTraction(float duration)
    {
        IsRecovering = true;
        float timer = 0f;
        currentSpeedMultiplier = 0.3f; // temporarily lose traction and slow down acceleration

        while (timer < duration)
        {
            timer += Time.deltaTime;
            currentSpeedMultiplier = Mathf.Lerp(0.3f, 1f, timer / duration);
            yield return null;
        }

        currentSpeedMultiplier = 1f;
        IsRecovering = false;
    }

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

        //jump & fall
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

        // --- WALL CLING DETECTION ---
        if (isTouchingWall && !isGrounded && Mathf.Abs(horizontalInput) > 0.1f)
        {
            // Determine which wall we are touching
            wallDirection = facingRight ? 1 : -1;
            isWallClinging = true;
        }
        else if (isGrounded || Mathf.Abs(horizontalInput) < 0.1f)
        {
            isWallClinging = false;
        }

        // --- DETACH FROM WALL WHEN PRESSING OPPOSITE DIRECTION ---
        if (isWallClinging)
        {
            if ((wallDirection == 1 && horizontalInput < 0) || (wallDirection == -1 && horizontalInput > 0))
            {
                // pressed opposite direction — detach
                isWallClinging = false;
                rb.gravityScale = normalGravity;
            }
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
        

        //flip
        if (facingRight == false && horizontalInput > 0) Flip();
        else if (facingRight == true && horizontalInput < 0) Flip();
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
        rb.velocity = new Vector2(horizontalInput * speed * currentSpeedMultiplier, rb.velocity.y);
    }

    void HandleWallCling()
    {
        rb.gravityScale = 0f;

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
            Vector2 checkDir = facingRight ? Vector2.right : Vector2.left;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3)checkDir * wallCheckDistance);
        }
    }
}
