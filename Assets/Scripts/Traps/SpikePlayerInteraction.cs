using System.Collections;
using UnityEngine;

public class MoveObjectOppositePlayer : MonoBehaviour
{
    [Header("Target")] public Transform movedObject;
    public string movedObjectName = "Movable"; // Auto-find if not assigned

    [Header("Motion")] public float moveDistance = 2f;
    public float moveSpeed = 3f;
    public bool useRigidbody2D = false;

    [Header("Return")] public bool returnAfter = false;
    public float returnDelay = 1f;

    private bool playerInside = false;
    private Transform playerTransform;
    private bool isMoving = false;
    private Vector3 originalPosition;

    void Awake()
    {
        if (movedObject == null) TryAutoFindSibling();
        if (movedObject != null) originalPosition = movedObject.position;
    }

    void TryAutoFindSibling()
    {
        if (transform.parent == null) return;
        Transform found = transform.parent.Find(movedObjectName);
        if (found != null) movedObject = found;
        else Debug.LogWarning($"MoveObjectOppositePlayer2D: No sibling named '{movedObjectName}' found.");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            playerTransform = other.transform;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            playerTransform = null;
        }
    }

    void Update()
    {
        if (playerInside && !isMoving && Input.GetKeyDown(KeyCode.F))
        {
            if (movedObject == null)
            {
                Debug.LogWarning("No movedObject assigned.");
                return;
            }
            StartCoroutine(MoveAwayCoroutine());
        }
    }

    IEnumerator MoveAwayCoroutine()
    {
        isMoving = true;

        Vector2 dir = (Vector2)(movedObject.position - playerTransform.position);
        if (dir == Vector2.zero) dir = Vector2.up;
        dir = dir.normalized;

        // Restrict movement to pure X or pure Y
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            dir = new Vector2(Mathf.Sign(dir.x), 0f);
        else
            dir = new Vector2(0f, Mathf.Sign(dir.y));

        Rigidbody2D rb = movedObject.GetComponent<Rigidbody2D>();

        // move until hitting a tagged object
        bool hit = false;
        while (!hit)
        {
            Vector2 nextPos = (Vector2)movedObject.position + dir * moveSpeed * Time.deltaTime;

            // Advance movement
            if (useRigidbody2D && rb != null)
                rb.MovePosition(nextPos);
            else
                movedObject.position = nextPos;

            // Check collision with stop-tag objects
            Collider2D[] hits = Physics2D.OverlapCircleAll(nextPos, 0.1f);
            foreach (var h in hits)
            {
                if (h.CompareTag("Floor")) // CHANGE THIS TAG NAME
                {
                    hit = true;
                    break;
                }
            }

            yield return null;
        }

        isMoving = false;
    }
}

