using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
public class PushAwayOnPlayerTrigger2D : MonoBehaviour
{
    [Header("Start")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool lockToCardinalAxis = true;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Stop")]
    [SerializeField] private string stopOnTag = "Floor";
    [SerializeField] private bool allowParentOrRootStopTagMatch = true;
    [Tooltip("If true, checks overlaps every physics step to avoid missed stop events.")]
    [SerializeField] private bool useContinuousOverlapStopCheck = true;
    [Tooltip("If true, contacts with TilemapCollider2D will never stop this object.")]
    [SerializeField] private bool ignoreTilemapCollidersForStop = true;

    private Rigidbody2D rb;
    private Collider2D[] selfColliders;
    private readonly Collider2D[] overlapResults = new Collider2D[24];
    private Vector2 moveDirection;
    private bool hasStarted;
    private bool stoppedForever;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = GetComponentInParent<Rigidbody2D>();
        selfColliders = GetComponentsInChildren<Collider2D>(includeInactive: false);
    }

    private void Update()
    {
        if (rb != null) return;
        if (!hasStarted || stoppedForever) return;

        transform.position += (Vector3)(moveDirection * moveSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (stoppedForever)
        {
            // Keep lock deterministic even if another script changes body settings.
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.bodyType = RigidbodyType2D.Static;
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
            }
            return;
        }

        if (!hasStarted) return;

        if (useContinuousOverlapStopCheck && CheckOverlapForStopTag())
        {
            StopAndLock();
            return;
        }

        if (rb == null) return;

        rb.velocity = moveDirection * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (stoppedForever) return;

        if (!hasStarted)
        {
            if (!other.CompareTag(playerTag)) return;
            StartMoveFromPlayer(other.transform);
            return;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasStarted || stoppedForever) return;
        if (ShouldStopBy(collision.collider))
            StopAndLock();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!hasStarted || stoppedForever) return;
        if (ShouldStopBy(collision.collider))
            StopAndLock();
    }

    private void StartMoveFromPlayer(Transform playerTransform)
    {
        Vector2 dir = (Vector2)(transform.position - playerTransform.position);
        if (dir == Vector2.zero) dir = Vector2.up;

        if (lockToCardinalAxis)
        {
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                dir = new Vector2(Mathf.Sign(dir.x), 0f);
            else
                dir = new Vector2(0f, Mathf.Sign(dir.y));
        }

        moveDirection = dir.normalized;
        hasStarted = true;
    }

    public void StopAndLock()
    {
        if (stoppedForever) return;

        stoppedForever = true;
        hasStarted = false;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Static;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        // Prevent any restart from trigger callbacks in this component.
        enabled = false;
    }

    private bool IsStopTag(Collider2D other)
    {
        if (string.IsNullOrWhiteSpace(stopOnTag)) return true;
        if (other.CompareTag(stopOnTag)) return true;
        if (!allowParentOrRootStopTagMatch) return false;

        Transform t = other.transform;
        if (t.parent != null && t.parent.CompareTag(stopOnTag)) return true;
        if (t.root != null && t.root.CompareTag(stopOnTag)) return true;
        return false;
    }

    private bool ShouldStopBy(Collider2D other)
    {
        if (other == null) return false;
        if (other.isTrigger) return false;
        if (other.CompareTag(playerTag)) return false;
        if (other.transform.root == transform.root) return false;
        if (ignoreTilemapCollidersForStop && other.GetComponent<TilemapCollider2D>() != null) return false;
        return IsStopTag(other);
    }

    private bool CheckOverlapForStopTag()
    {
        if (selfColliders == null || selfColliders.Length == 0) return false;

        ContactFilter2D filter = new ContactFilter2D
        {
            useTriggers = false
        };
        filter.NoFilter();

        for (int i = 0; i < selfColliders.Length; i++)
        {
            Collider2D self = selfColliders[i];
            if (self == null || !self.enabled) continue;

            int count = self.OverlapCollider(filter, overlapResults);
            for (int j = 0; j < count; j++)
            {
                Collider2D other = overlapResults[j];
                if (ShouldStopBy(other))
                    return true;
            }
        }

        return false;
    }
}
