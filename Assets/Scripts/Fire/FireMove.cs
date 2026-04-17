using UnityEngine;

public class FireMove : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;

    [Header("Special Objects")]
    public GameObject changedObject1;
    public GameObject changedObject2;

    private Collider2D col;
    private Rigidbody2D rb;

    private bool playerInside;
    private Transform playerTransform;

    private bool isMoving;
    private bool hasStopped;
    private Vector2 moveDir;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (playerInside && !isMoving && !hasStopped)
        {
            CalculateMoveDirection();
            isMoving = true;
        }
    }

    void FixedUpdate()
    {
        if (isMoving && !hasStopped)
        {
            rb.velocity = moveDir * moveSpeed;
        }
    }

    void CalculateMoveDirection()
    {
        Vector2 dir = (Vector2)(transform.position - playerTransform.position);
        if (dir == Vector2.zero) dir = Vector2.up;

        //lock to X or Y
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            moveDir = new Vector2(Mathf.Sign(dir.x), 0f);
        else
            moveDir = new Vector2(0f, Mathf.Sign(dir.y));
    }

    void StopMove()
    {
        isMoving = false;
        hasStopped = true;
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static; //permanently stop
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        //ignore player completely
        if (collision.collider.CompareTag("Player"))
            return;

        Debug.Log("hit " + collision.collider.name);

        if (hasStopped) return;

        StopMove();

        if (!isMoving && collision.collider.CompareTag("Ice"))
        {
            changedObject1.tag = "Floor";
            changedObject2.tag = "Untagged";
        }
        else if (!isMoving && collision.collider.CompareTag("Spikes"))
        {
            Destroy(changedObject1);
            Destroy(changedObject2);
            Destroy(gameObject);
        }
        else if (!isMoving && collision.collider.CompareTag("SpikeBack"))
        {
            Destroy(changedObject1);
            Destroy(changedObject2);
            Destroy(gameObject);
        }
        
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
}