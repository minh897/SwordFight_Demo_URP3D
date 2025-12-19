using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private GameObject uiHealthBar;
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    private GameManager gameManager;
    private Slider healthSlider;

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        if (gameObject.CompareTag("Enemy"))
            healthSlider = uiHealthBar.GetComponentInChildren<Slider>();
    }

    void Start()
    {
        currentHealth = maxHealth;

        if (gameObject.CompareTag("Enemy"))
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
            // check for win or lose condition whenever player or an enemy die
            gameManager.CheckWinLoseCondition(gameObject);
        }

        Debug.Log(transform.gameObject + " health reduced by " + currentHealth);
    }

    private void Die()
    {
        Debug.Log(transform.gameObject + " has died");
        transform.gameObject.SetActive(false);
    }
}
