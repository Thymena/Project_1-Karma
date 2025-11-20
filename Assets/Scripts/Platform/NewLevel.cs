
using UnityEngine;


public class NewLevel: MonoBehaviour
{
    
    public Transform stopPoint;
    public float speed = 2f;
    public float stopThreshold = 0.01f;
    

    // internal state
    private Vector3 _targetPosition;
    private bool _isMoving = true;

    void Start()
    {
        _targetPosition = stopPoint.position;
    }

    void Update()
    {
        if (!_isMoving) return;

        // Move using transform (no physics)
        Vector3 newPos = Vector3.MoveTowards(transform.position, _targetPosition, speed * Time.deltaTime);
        transform.position = newPos;

        // Check if reached within threshold
        if (Vector3.Distance(newPos, _targetPosition) <= stopThreshold)
        {
            transform.position = _targetPosition; 
            _isMoving = false;

            // permanently stop the script to avoid further updates
            enabled = false;
        }
    }

   
    public void StopMovement()
    {
        _isMoving = false;
        enabled = false;
    }
    
    public bool IsMoving() => _isMoving;
}
