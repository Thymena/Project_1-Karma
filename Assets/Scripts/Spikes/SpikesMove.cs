using System.Collections;
using UnityEngine;

public class SpikesMove : MonoBehaviour
{
    [Header("Target")] public Transform movedObject;

    [Header("Motion")] public float moveSpeed = 10f;

    private bool playerInside = false;
    private Transform playerTransform;

    public bool isMoving = false;
    public bool hit = false;                                                                                                                                                                                                                                                                                     
    public GameObject changedObject1;
	public GameObject changedObject2;
	public bool isHitFire = false;
	public bool isHitIce = false;

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

        // Move infinitely until hitting
        while (!hit)
        {
            Vector2 nextPos = (Vector2)movedObject.position + dir * moveSpeed * Time.deltaTime;
            movedObject.position = nextPos;

            // Check collision with floor
            Collider2D[] hits = Physics2D.OverlapCircleAll(nextPos, 0.1f);
            foreach (var h in hits)
            {
                if (h.CompareTag("Floor")) 
                {
                    hit = true;
                    break;
                }
                
				// Check collision with Ice
                if (h.CompareTag("Ice"))
                {
                    hit = true;
                    isHitIce = true;

                    Destroy(changedObject1);
					Destroy(changedObject2);
                    Destroy(gameObject);
                    
                    break;
                }
				// Check collision with fire
                if (h.CompareTag("Fire"))
                {
                    hit = true;
					isHitFire = true;
                   
                    Destroy(gameObject);
                    Destroy(changedObject1);
					Destroy(changedObject2);

                    break;
                }
			
            }

            yield return null;
        }

        isMoving = false;
    }
}