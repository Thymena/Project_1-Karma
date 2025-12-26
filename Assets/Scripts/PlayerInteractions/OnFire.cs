using UnityEngine;

public class OnFire : MonoBehaviour
{
    public const float KILL_TIME = 3.5f;

    [Header("Damage")]
    public PlayerHP playerHP;
    public float damage = 100f;

    [Header("Flash Settings")]
    public float maxFlashInterval = 0.35f; // slow at start
    public float minFlashInterval = 0.05f; // fast near death
    public Color flashColor = Color.red;

    private float timer;
    private float flashTimer;
    private bool onFire;
    private bool flashOn;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        playerHP = GetComponent<PlayerHP>();
    }

    void Update()
    {
        if (playerHP.health <= 0)
        {
            StopFlashing();
            return;
        }

        if (onFire)
        {
            HandleFlashing();
        }
    }

    void HandleFlashing()
    {
        float t = Mathf.Clamp01(timer / KILL_TIME);
        float flashInterval = Mathf.Lerp(maxFlashInterval, minFlashInterval, t);

        flashTimer += Time.deltaTime;

        if (flashTimer >= flashInterval)
        {
            flashOn = !flashOn;
            spriteRenderer.color = flashOn ? flashColor : originalColor;
            flashTimer = 0f;
        }
    }

    void StopFlashing()
    {
        onFire = false;
        timer = 0f;
        flashTimer = 0f;
        flashOn = false;

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Fire")) return;

        onFire = true;
        timer += Time.deltaTime;

        if (timer >= KILL_TIME)
        {
            playerHP.health -= damage;
            timer = 0f;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Fire"))
        {
            StopFlashing();
        }
    }
}