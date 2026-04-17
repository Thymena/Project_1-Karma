using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
public class IceAndSpike : MonoBehaviour
{
    [Header("Filter")]
    [Tooltip("Only react when touching a collider with this tag (usually the Spikes tilemap object).")]
    [SerializeField] private string requiredOtherTag = "Spikes";

    [Header("Tilemap")]
    [Tooltip("Optional. If empty, the Tilemap will be read from the touched collider.")]
    [SerializeField] private Tilemap targetTilemap;
    [SerializeField] private bool reactToTriggers = true;

    [Header("Affected Cell Result")]
    [Tooltip("Optional prefab to spawn on the touched cell.")]
    [SerializeField] private GameObject reactedCellPrefab;
    [SerializeField] private Sprite reactedSprite;
    [SerializeField] private bool addCollider = true;

    [Header("Self")]
    [SerializeField] private bool destroySelfAfterReaction = true;

    private bool consumed;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (consumed || collision.contactCount <= 0) return;
        TryReact(collision.collider, collision.GetContact(0).point);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!reactToTriggers || consumed) return;
        TryReact(other, other.ClosestPoint(transform.position));
    }

    private void TryReact(Collider2D otherCollider, Vector2 contactPoint)
    {
        if (!string.IsNullOrWhiteSpace(requiredOtherTag) && !otherCollider.CompareTag(requiredOtherTag))
            return;

        Tilemap tilemap = targetTilemap != null ? targetTilemap : otherCollider.GetComponent<Tilemap>();
        if (tilemap == null) return;

        Vector3Int cell = tilemap.WorldToCell(contactPoint);
        if (!tilemap.HasTile(cell)) return;

        consumed = true;

        Vector3 cellCenter = tilemap.GetCellCenterWorld(cell);
        tilemap.SetTile(cell, null);
        
        GameObject reactedCell = reactedCellPrefab != null
            ? Instantiate(reactedCellPrefab, cellCenter, Quaternion.identity)
            : new GameObject("IceAndSpike_Cell");

        reactedCell.transform.position = cellCenter;

        SpriteRenderer sr = reactedCell.GetComponent<SpriteRenderer>();
        if (sr == null) sr = reactedCell.AddComponent<SpriteRenderer>();
        if (reactedSprite != null) sr.sprite = reactedSprite;

        // Required behavior: use this moving object's tag/layer.
        reactedCell.tag = gameObject.tag;
        reactedCell.layer = gameObject.layer;

        if (addCollider && reactedCell.GetComponent<Collider2D>() == null)
            reactedCell.AddComponent<BoxCollider2D>();

        if (destroySelfAfterReaction)
            Destroy(gameObject);
    }
}
