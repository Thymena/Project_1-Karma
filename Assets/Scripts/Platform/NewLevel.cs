
using UnityEngine;


public class NewLevel: MonoBehaviour
{
    
    public Transform stopPoint;
    public float speed = 2f;
    public float stopThreshold = 0.01f;
    

    // internal state
    private Vector3 _targetPosition;
    private Rigidbody2D _rigidbody2D;
    private bool _isMoving = true;

    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();

        if (_rigidbody2D == null)
        {
            Debug.LogError($"{nameof(NewLevel)} requires a Rigidbody2D component.", this);
            enabled = false;
        }
    }

    void Start()
    {
        _targetPosition = stopPoint.position;
    }

    void FixedUpdate()
    {
        if (!_isMoving || _rigidbody2D == null) return;

        Vector2 currentPosition = _rigidbody2D.position;
        Vector2 targetPosition = _targetPosition;
        Vector2 newPos = Vector2.MoveTowards(currentPosition, targetPosition, speed * Time.fixedDeltaTime);

        _rigidbody2D.MovePosition(newPos);

        // Check if reached within threshold
        if (Vector2.Distance(newPos, targetPosition) <= stopThreshold)
        {
            _rigidbody2D.MovePosition(targetPosition);
            StopMovement();
        }
    }

    public void StopMovement()
    {
        _isMoving = false;
        if (_rigidbody2D != null)
        {
            _rigidbody2D.velocity = Vector2.zero;
            _rigidbody2D.angularVelocity = 0f;
            _rigidbody2D.MovePosition(_targetPosition);
        }
        enabled = false;
    }

    public bool IsMoving() => _isMoving;
}
