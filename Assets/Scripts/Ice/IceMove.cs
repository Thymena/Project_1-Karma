using System.Collections;
using UnityEngine;

public class IceMove : MonoBehaviour
{
    [Header("Target")] public Transform movedObject;

    [Header("Motion")] public float moveSpeed = 10f;

    private bool playerInside = false;
    private Transform playerTransform;

    public bool isMoving = false;
    public  bool hit = false;
    public GameObject changedObject;
    public bool isHitSpikes = false;

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
            StartCoroutine(MoveAwayCoroutine());
        }
    }
    
    public void StopMovement()
    {
        hit = true;
        isMoving = false;
    }

    IEnumerator MoveAwayCoroutine()
    {
        isMoving = true;

        // Determine direction
        Vector2 dir = (Vector2)(movedObject.position - playerTransform.position);
        if (dir == Vector2.zero) dir = Vector2.up;
        dir = dir.normalized;

        // Restrict movement to pure X or pure Y
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            dir = new Vector2(Mathf.Sign(dir.x), 0f);
        else
            dir = new Vector2(0f, Mathf.Sign(dir.y));

        // Move infinitely until hitting a tagged object
        while (!hit)
        {
            Vector2 nextPos = (Vector2)movedObject.position + dir * moveSpeed * Time.deltaTime;

            // Move directly (no rigidbody)
            movedObject.position = nextPos;

            // Check collision with floor objects
            Collider2D[] hits = Physics2D.OverlapCircleAll(nextPos, 0.1f);
            foreach (var h in hits)
            {
                if (h.CompareTag("Floor")) 
                {
                    hit = true;
                    break;
                }
                
				//check collision with fire
                if (h.CompareTag("Fire"))
                {
                    hit = true;
                    changedObject.tag = "Floor";
                    changedObject.layer = LayerMask.NameToLayer("Wall");
                    Destroy(gameObject);
                    
                    break;
                }

				//check collision with spikes
                if (h.CompareTag("Spikes"))
                {
                    hit = true;
                    isHitSpikes = true;
                    Destroy(gameObject);
                    Destroy(changedObject);

                    break;
                }
			
            }

            yield return null;
        }

        isMoving = false;
    }
}