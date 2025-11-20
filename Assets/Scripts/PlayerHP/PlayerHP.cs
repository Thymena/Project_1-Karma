using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHP : MonoBehaviour
{
    // Start is called before the first frame update
    public float health;
    [SerializeField] private float maxHealth;
    void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
        if (health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void TakeDamage(float damage)
    {
        health = Mathf.Clamp(health - damage, 0, maxHealth);
    
        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage(50);
        }
    }
}
