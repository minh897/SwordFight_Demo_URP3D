using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    private GameManager gameManager;

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(transform.gameObject + " health reduced by " + currentHealth);
        if (currentHealth <= 0)
        {
            Die();
            // check for win or lose condition whenever player or an enemy die
            gameManager.CheckWinLoseCondition(gameObject);
        }
    }

    private void Die()
    {
        Debug.Log(transform.gameObject + " has died");
        transform.gameObject.SetActive(false);
    }
}
