using UnityEngine;

public class PointerScriptArrow : MonoBehaviour
{
    public string targetTag = "Berry";
    public float angleOffset = 135f; // keep same visual alignment as UI pointer

    void Update()
    {
        Transform target = GetActiveBerry();
        if (target == null) return;

        // Direction from pointer to target (world space)
        Vector2 dir = (Vector2)target.position - (Vector2)transform.position;

        if (dir.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            angle += angleOffset;

            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    Transform GetActiveBerry()
    {
        GameObject[] berries = GameObject.FindGameObjectsWithTag(targetTag);

        foreach (GameObject b in berries)
        {
            if (b.activeInHierarchy)
                return b.transform;
        }
        return null;
    }
}