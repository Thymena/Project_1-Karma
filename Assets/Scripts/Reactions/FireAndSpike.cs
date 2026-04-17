using UnityEngine;

[DisallowMultipleComponent]
public class FireAndSpike : MonoBehaviour
{
    [Header("Filter")]
    [Tooltip("Only react when touching a collider with this tag.")]
    [SerializeField] private string requiredOtherTag = "Spikes";
    [SerializeField] private bool reactToTriggers = true;

    [Header("Repel")]
    [SerializeField] private float repelSpeed = 8f;
    [SerializeField] private float minSecondsBetweenRepels = 0.08f;
    [SerializeField] private float fallbackPushDistance = 0.35f;

    private Rigidbody2D rb;
    private float lastRepelTime = -10f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contactCount <= 0) return;
        if (!CanReact(collision.collider)) return;

        Vector2 contactPoint = collision.GetContact(0).point;
        RepelFromPoint(contactPoint);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!reactToTriggers) return;
        if (!CanReact(other)) return;

        Vector2 contactPoint = other.ClosestPoint(transform.position);
        RepelFromPoint(contactPoint);
    }

    private bool CanReact(Collider2D otherCollider)
    {
        if (Time.time - lastRepelTime < minSecondsBetweenRepels) return false;
        if (!string.IsNullOrWhiteSpace(requiredOtherTag) && !otherCollider.CompareTag(requiredOtherTag)) return false;
        return true;
    }

    private void RepelFromPoint(Vector2 contactPoint)
    {
        lastRepelTime = Time.time;

        Vector2 direction = ((Vector2)transform.position - contactPoint).normalized;
        if (direction == Vector2.zero) direction = Vector2.up;

        // Keep this reaction simple and independent from tilemap edits.
        if (rb != null)
        {
            rb.velocity = direction * repelSpeed;
        }
        else
        {
            transform.position += (Vector3)(direction * fallbackPushDistance);
        }
    }
}
