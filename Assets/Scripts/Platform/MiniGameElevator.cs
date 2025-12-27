using UnityEngine;

public class MiniGameElevator : MonoBehaviour
{
    public float riseSpeed = 3f;
    public float returnSpeed = 3f;
    public float changeTime = 1f;
    public float fadeLeadTime = 0.5f; // optional fade start before scene load
    public Transform movedObject;
    public GameController start;

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

        // start game 
        if (timerDelay > 1.5f && !start.startTest)
        {
            start.startTest = true;
        }

        // Elevator movement and scene transition
        if (timerDelay > 0.5f && start.pastTest)
        {
            timer += Time.deltaTime;

            // Move elevator up
            movedRigidbody.MovePosition(movedRigidbody.position + Vector2.up * riseSpeed * Time.deltaTime);

            // start animation early for better effects
            if (!fadeStarted && timer >= changeTime - fadeLeadTime)
            {
                fadeStarted = true;
                SceneControllerScript.instance.BeginTransition(fadeDuration: 1f, sceneLoadDelay: 0f);
            }

            // Mark elevator as triggered when done
            if (timer >= changeTime)
            {
                triggered = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || triggered) return;

        // reset timers
        timer = 0f;
        timerDelay = 0f;
        
        Vector2 newPos = Vector2.MoveTowards(movedRigidbody.position, startPos, returnSpeed * Time.deltaTime);
        movedRigidbody.MovePosition(newPos);
    }
}
