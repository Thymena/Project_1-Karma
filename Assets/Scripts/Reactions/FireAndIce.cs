using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
public class FireAndIce : MonoBehaviour
{
    [Header("Filter")]
    [Tooltip("Only react when touching a collider with this tag (usually the Ice tilemap object).")]
    [SerializeField] private string requiredOtherTag = "Ice";

    [Header("Tilemap")]
    [Tooltip("Optional. If empty, the Tilemap will be read from the touched collider.")]
    [SerializeField] private Tilemap targetTilemap;
    [SerializeField] private bool reactToTriggers = true;
    [Tooltip("If true, the script can still react when tag is found on parent/root instead of the exact collider.")]
    [SerializeField] private bool allowParentOrRootTagMatch = true;
    [Tooltip("If true, also try this object's position when contact-point cell has no tile.")]
    [SerializeField] private bool fallbackToSelfPositionCell = true;

    [Header("Self Transform After Reaction")]
    [SerializeField] private Sprite reactedSprite;
    [SerializeField] private string reactedTag = "Floor";
    [SerializeField] private int reactedLayer = -1;
    [SerializeField] private bool ensureCollider = true;

    [Header("Self")]
    [SerializeField] private bool destroySelfAfterReaction = false;

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

    // Called by an optional child trigger relay (for setups where Fire trigger is on a child).
    public void TryReactFromChildTrigger(Collider2D other, Vector2 contactPoint)
    {
        if (!reactToTriggers || consumed) return;
        TryReact(other, contactPoint);
    }

    private void TryReact(Collider2D otherCollider, Vector2 contactPoint)
    {
        if (!IsOtherTagMatch(otherCollider))
            return;

        Tilemap tilemap = ResolveTilemap(otherCollider, contactPoint);
        if (tilemap == null) return;

        if (!TryFindReactCell(tilemap, contactPoint, out Vector3Int cell))
            return;

        consumed = true;

        tilemap.SetTile(cell, null);
        
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && reactedSprite != null) sr.sprite = reactedSprite;

        if (!string.IsNullOrWhiteSpace(reactedTag))
            gameObject.tag = reactedTag;

        if (reactedLayer >= 0 && reactedLayer <= 31)
            gameObject.layer = reactedLayer;

        if (ensureCollider && GetComponent<Collider2D>() == null)
            gameObject.AddComponent<BoxCollider2D>();

        if (destroySelfAfterReaction)
            Destroy(gameObject);
    }

    private bool IsOtherTagMatch(Collider2D otherCollider)
    {
        if (string.IsNullOrWhiteSpace(requiredOtherTag)) return true;
        if (otherCollider.CompareTag(requiredOtherTag)) return true;
        if (!allowParentOrRootTagMatch) return false;

        Transform t = otherCollider.transform;
        if (t.parent != null && t.parent.CompareTag(requiredOtherTag)) return true;
        if (t.root != null && t.root.CompareTag(requiredOtherTag)) return true;

        return false;
    }

    private Tilemap ResolveTilemap(Collider2D otherCollider, Vector2 contactPoint)
    {
        if (targetTilemap != null) return targetTilemap;

        Tilemap tilemap = otherCollider.GetComponent<Tilemap>();
        if (tilemap != null) return tilemap;

        tilemap = otherCollider.GetComponentInParent<Tilemap>();
        if (tilemap != null) return tilemap;

        // Fallback: find any tilemap that actually has a tile at this contact point.
        Tilemap[] allTilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
        for (int i = 0; i < allTilemaps.Length; i++)
        {
            Tilemap candidate = allTilemaps[i];
            Vector3Int cell = candidate.WorldToCell(contactPoint);
            if (candidate.HasTile(cell))
                return candidate;
        }

        return null;
    }

    private bool TryFindReactCell(Tilemap tilemap, Vector2 contactPoint, out Vector3Int cell)
    {
        // Primary: exact contact point.
        Vector3Int c0 = tilemap.WorldToCell(contactPoint);
        if (tilemap.HasTile(c0))
        {
            cell = c0;
            return true;
        }

        // Secondary: search immediate neighbors around contact.
        if (TryFindNeighborTile(tilemap, c0, out cell))
            return true;

        // Tertiary: use this object's current position (useful when touching non-tilemap Ice objects).
        if (fallbackToSelfPositionCell)
        {
            Vector3Int c1 = tilemap.WorldToCell(transform.position);
            if (tilemap.HasTile(c1))
            {
                cell = c1;
                return true;
            }

            if (TryFindNeighborTile(tilemap, c1, out cell))
                return true;
        }

        cell = default;
        return false;
    }

    private bool TryFindNeighborTile(Tilemap tilemap, Vector3Int center, out Vector3Int found)
    {
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                Vector3Int c = new Vector3Int(center.x + x, center.y + y, center.z);
                if (!tilemap.HasTile(c)) continue;
                found = c;
                return true;
            }
        }

        found = default;
        return false;
    }
}
