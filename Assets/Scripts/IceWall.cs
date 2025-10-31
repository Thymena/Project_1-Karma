using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class IceWall : MonoBehaviour
{
    [Header("Ice Physical Properties")]
    public float iceFriction = 0.02f;
    public float iceBounciness = 0f;

    [Header("Wall Behavior")]
    public float iceWallSlideSpeed = 6f;
    public float normalGravity = 3f;

    [Header("Floor Behavior")]
    public float iceFloorSlideMultiplier = 0.98f;
    public float minHorizontalSpeed = 0.3f;
    public float recoveryTime = 1.5f; // time to regain traction after leaving ice

    private PhysicsMaterial2D iceMaterial;
    private Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        iceMaterial = new PhysicsMaterial2D("IceMaterial");
        iceMaterial.friction = iceFriction;
        iceMaterial.bounciness = iceBounciness;
        col.sharedMaterial = iceMaterial;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        PlayerMovementRe pm = collision.gameObject.GetComponent<PlayerMovementRe>();
        if (rb == null || pm == null) return;

        // Compute average contact normal to determine orientation
        Vector2 avgNormal = Vector2.zero;
        foreach (ContactPoint2D contact in collision.contacts)
            avgNormal += contact.normal;
        avgNormal.Normalize();

        bool isFloor = Vector2.Angle(avgNormal, Vector2.up) < 45f;
        bool isWall = !isFloor && (Vector2.Angle(avgNormal, Vector2.right) < 60f || Vector2.Angle(avgNormal, Vector2.left) < 60f);

        // --- FLOOR ICE BEHAVIOR (act like Floor-tagged collider for animations) ---
        if (isFloor)
        {
            // Ensure player is considered grounded by your PlayerMovementRe logic
            pm.isGrounded = true;

            // Ensure gravity is normal (so jumping/landing behave as normal)
            rb.gravityScale = pm.normalGravity;

            // Maintain running/falling/jumping states based on velocity so animations play normally
            // Running when significant horizontal speed while grounded:
            if (Mathf.Abs(rb.velocity.x) > 0.1f)
            {
                pm.isRunning = true;
            }
            else
            {
                pm.isRunning = false;
            }

            // Not jumping/falling when standing on floor (even if sliding)
            pm.isJumping = false;
            pm.isFalling = false;
            pm.isCrouching = false;

            // Apply sliding due to low traction (preserve horizontal velocity sign)
            if (Mathf.Abs(rb.velocity.x) > minHorizontalSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x * iceFloorSlideMultiplier, rb.velocity.y);
            }

            // Trigger animator update
            pm.SetAnimatorState();

            // Start recovery coroutine if not already running (so when leaving ice, traction is regained smoothly)
            if (!pm.IsRecovering)
            {
                pm.StartCoroutine(pm.RecoverTraction(recoveryTime));
            }

            return;
        }

        // --- WALL ICE BEHAVIOR ---
        if (isWall && !pm.isGrounded)
        {
            float side = Mathf.Sign(collision.transform.position.x - transform.position.x);

            bool pressingToward =
                (side < 0f && Input.GetKey(KeyCode.D)) ||
                (side > 0f && Input.GetKey(KeyCode.A));

            bool pressingAway =
                (side < 0f && Input.GetKey(KeyCode.A)) ||
                (side > 0f && Input.GetKey(KeyCode.D));

            if (pressingToward)
            {
                // Cling & slide down — block upward movement
                rb.gravityScale = 0f;
                rb.velocity = new Vector2(0f, -iceWallSlideSpeed);

                // Let PlayerMovementRe handle animations but set falling state so animator reflects sliding
                pm.isFalling = true;
                pm.isJumping = false;
                pm.isRunning = false;
                pm.isCrouching = false;
                pm.SetAnimatorState();
            }
            else if (pressingAway)
            {
                // Detach: restore normal gravity; animations handled by player's script
                rb.gravityScale = pm.normalGravity;
            }
            else
            {
                // Neither toward nor away: normal gravity so player can fall
                rb.gravityScale = pm.normalGravity;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        PlayerMovementRe pm = collision.gameObject.GetComponent<PlayerMovementRe>();
        if (rb == null || pm == null) return;

        // When leaving ice, restore normal gravity and mark as airborne so player's jump/fall logic runs
        rb.gravityScale = pm.normalGravity;
        pm.isGrounded = false;
        pm.isFalling = true;
        pm.isRunning = false;
        pm.SetAnimatorState();

        // Optionally start traction recovery on player (if not already running)
        if (!pm.IsRecovering)
            pm.StartCoroutine(pm.RecoverTraction(recoveryTime));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Collider2D c = GetComponent<Collider2D>();
        if (c != null)
            Gizmos.DrawWireCube(c.bounds.center, c.bounds.size);
    }
}