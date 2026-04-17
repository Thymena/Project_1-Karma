using UnityEngine;

[DisallowMultipleComponent]
public class FireAndIceChildTriggerRelay : MonoBehaviour
{
    [Tooltip("Auto-found from parent if left empty.")]
    [SerializeField] private FireAndIce fireAndIce;
    [SerializeField] private string requiredSelfTag = "Fire";

    private void Awake()
    {
        if (fireAndIce == null)
            fireAndIce = GetComponentInParent<FireAndIce>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (fireAndIce == null) return;
        if (!string.IsNullOrWhiteSpace(requiredSelfTag) && !CompareTag(requiredSelfTag)) return;

        Vector2 contactPoint = other.ClosestPoint(transform.position);
        fireAndIce.TryReactFromChildTrigger(other, contactPoint);
    }
}
