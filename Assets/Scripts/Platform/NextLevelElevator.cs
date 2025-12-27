using UnityEngine;

public class NextLevelElevator : MonoBehaviour
{
    public float riseSpeed = 3f;
    public float returnSpeed = 3f;
    public float changeTime = 1f;
    public float fadeLeadTime = 0.5f;

    public Transform movedObject;

    private Rigidbody2D movedRigidbody;
    private Vector2 startPos;

    private float timer = 0f;
    private float timerDelay = 0f;
    private bool fadeStarted = false;
    private bool triggered = false; // ensures scene loads only once

    void Start()
    {
        if (movedObject == null)
            movedObject = transform;

        movedRigidbody = movedObject.GetComponent<Rigidbody2D>();
        if (movedRigidbody == null)
        {
            enabled = false;
            return;
        }

        movedRigidbody.isKinematic = true;
        startPos = movedRigidbody.position;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || triggered) return;

        timerDelay += Time.deltaTime;

        if (timerDelay > 0.5f) // player must stay on elevator briefly
        {
            timer += Time.deltaTime;

            // Move elevator up
            movedRigidbody.MovePosition(
                movedRigidbody.position + Vector2.up * riseSpeed * Time.deltaTime
            );

            // Start fade early while elevator is moving
            if (!fadeStarted && timer >= changeTime - fadeLeadTime)
            {
                fadeStarted = true;
                SceneControllerScript.instance.BeginTransition(fadeDuration: 1f, sceneLoadDelay: 0f);
            }

            // Mark elevator as triggered when done
            if (timer >= changeTime)
                triggered = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || triggered) return;

        // Reset timers
        timer = 0f;
        timerDelay = 0f;
        
        Vector2 newPos = Vector2.MoveTowards(
            movedRigidbody.position,
            startPos,
            returnSpeed * Time.deltaTime
        );
        movedRigidbody.MovePosition(newPos);
    }
}
