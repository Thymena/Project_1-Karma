using UnityEngine;

public class PointerUI : MonoBehaviour
{
    public string targetTag = "Berry";
    private RectTransform pointerRect;
    private RectTransform canvasRect;

    void Awake()
    {
        pointerRect = GetComponent<RectTransform>();
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    void Update()
    {
        Transform target = GetActiveBerry();
        if (target == null) return;

        // Convert target world position → screen position
        Vector2 screenPos = Camera.main.WorldToScreenPoint(target.position);

        // Convert screen → local canvas coordinates
        Vector2 canvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            null,              // IMPORTANT: null camera for ScreenSpace-Overlay
            out canvasPos
        );

        // Direction from pointer to target
        Vector2 pointerPos = pointerRect.anchoredPosition;
        Vector2 dir = canvasPos - pointerPos;

        if (dir.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            angle += 135f;

            pointerRect.localRotation = Quaternion.Euler(0, 0, angle);
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