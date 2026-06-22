using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHP : MonoBehaviour
{
    private static readonly int DieHash = Animator.StringToHash("Die");

    public float health;
    [SerializeField] private float maxHealth;

    private Animator animator;
    private Rigidbody2D rb;
    private PlayerMovementRe movement;
    private bool isDead;

    public bool IsDead => isDead;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovementRe>();
    }

    void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
        if (health <= 0 && !isDead)
            StartCoroutine(DieSequence());
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        health = Mathf.Clamp(health - damage, 0, maxHealth);
    }

    IEnumerator DieSequence()
    {
        isDead = true;

        if (movement != null)
            movement.enabled = false;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;
        }

        if (animator != null)
            animator.SetTrigger(DieHash);

        yield return null;

        if (animator != null)
        {
            while (true)
            {
                var state = animator.GetCurrentAnimatorStateInfo(0);
                if (state.IsName("Player_Die") && state.normalizedTime >= 1f)
                    break;
                yield return null;
            }
        }

        DeathTimesCounter.Instance.AddDeath();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
