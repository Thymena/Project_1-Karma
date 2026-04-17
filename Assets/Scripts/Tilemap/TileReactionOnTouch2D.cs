using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// When this object touches a Tilemap (via Collision2D or Trigger2D),
/// removes ONLY the contacted tile, destroys this object, and spawns a replacement prefab.
/// </summary>
[DisallowMultipleComponent]
public class TileReactionOnTouch2D : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("If left empty, the script will try to find a Tilemap on the other collider.")]
    [SerializeField] private Tilemap targetTilemap;

    [Header("Reaction")]
    [Tooltip("Prefab to spawn at the tile center after the tile/object disappear.")]
    [SerializeField] private GameObject spawnPrefab;

    [Tooltip("Optional offset for the spawned prefab (world space).")]
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;

    [Tooltip("If true, the script will only react if the contacted cell currently has a tile.")]
    [SerializeField] private bool requireTilePresent = true;

    [Tooltip("If true, will also react with Trigger collisions (set TilemapCollider2D to Is Trigger).")]
    [SerializeField] private bool reactToTriggers = true;

    [Header("Self (what happens to THIS object)")]
    [Tooltip("If true, destroys this GameObject. If false, it will be 'transformed' by changing sprite/tag/layer.")]
    [SerializeField] private bool destroySelf = true;

    [Tooltip("SpriteRenderer to change when destroySelf is false. If empty, tries GetComponent<SpriteRenderer>().")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Tooltip("New sprite to set when destroySelf is false. Leave empty to keep current sprite.")]
    [SerializeField] private Sprite newSprite;

    [Tooltip("New Tag to set when destroySelf is false. Leave empty to keep current tag. Tag must exist in Project Settings > Tags and Layers.")]
    [SerializeField] private string newTag;

    [Tooltip("New Layer to set when destroySelf is false. -1 means keep current layer.")]
    [SerializeField] private int newLayer = -1;

    private bool _consumed;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_consumed) return;

        // Contact point is the simplest reliable way to pick the exact cell hit.
        if (collision.contactCount <= 0) return;
        Vector2 contactPoint = collision.GetContact(0).point;

        TryReact(collision.collider, contactPoint);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!reactToTriggers) return;
        if (_consumed) return;

        // For triggers there are no contacts, so use ClosestPoint.
        Vector2 contactPoint = other.ClosestPoint(transform.position);
        TryReact(other, contactPoint);
    }

    private void TryReact(Collider2D otherCollider, Vector2 contactPoint)
    {
        Tilemap tilemap = targetTilemap != null ? targetTilemap : otherCollider.GetComponent<Tilemap>();
        if (tilemap == null) return;

        // Convert the world contact point to the tile cell.
        Vector3Int cellPos = tilemap.WorldToCell(contactPoint);

        if (requireTilePresent && !tilemap.HasTile(cellPos)) return;

        _consumed = true;

        // Remove ONLY this one tile.
        tilemap.SetTile(cellPos, null);

        // Spawn replacement at the center of that cell.
        if (spawnPrefab != null)
        {
            Vector3 spawnPos = tilemap.GetCellCenterWorld(cellPos) + spawnOffset;
            Instantiate(spawnPrefab, spawnPos, Quaternion.identity);
        }

        // Either destroy this object, or "transform" it to look/behave like a new one.
        if (destroySelf)
        {
            Destroy(gameObject);
            return;
        }

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && newSprite != null)
            spriteRenderer.sprite = newSprite;

        if (!string.IsNullOrWhiteSpace(newTag))
            gameObject.tag = newTag;

        if (newLayer >= 0 && newLayer <= 31)
            gameObject.layer = newLayer;
    }
}


