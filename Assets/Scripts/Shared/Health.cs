using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour, IDamageable
{
    public bool IsDead { get; private set; }

    [SerializeField] private Slider healthSliderUI;
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;

        // set up value for health bar UI
        healthSliderUI.maxValue = maxHealth;
        healthSliderUI.value = currentHealth;
    }

    public void TakeDamage(float damage)
    {
        HandleHealth(damage);
        HandleHealthUI();
        HandleDeath();
    }

    private void HandleHealth(float damage)
    {
        currentHealth -= damage;
    }

    private void HandleDeath()
    {
        // check if current health is depleted
        if (currentHealth > 0)
            return;
        
        IsDead = true;
    }

    private void HandleHealthUI()
    {
        // disable health bar UI when died
        if (currentHealth <= 0)
        {
            healthSliderUI.gameObject.SetActive(false);
            return;
        }

        // update slider value with current health
        healthSliderUI.value = currentHealth;
    }
}
