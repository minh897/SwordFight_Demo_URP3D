using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour, IDamageable
{
    public bool IsDead { get; private set; }

    [SerializeField] private GameObject uiHealthBar;
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    private Slider healthSlider;

    void Awake()
    {
        if (gameObject.CompareTag("Enemy"))
            healthSlider = uiHealthBar.GetComponentInChildren<Slider>();
    }

    void Start()
    {
        currentHealth = maxHealth;

        // set up enmy health bar UI
        if (gameObject.CompareTag("Enemy"))
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        HandleHealth(damage);
        HandleDeath();
    }

    private void HandleHealth(int damage)
    {
        currentHealth -= damage;

        // update enemy health bar ui
        if (gameObject.CompareTag("Enemy"))
            healthSlider.value = currentHealth;
    }

    private void HandleDeath()
    {
        // check if current health is depleted
        if (currentHealth > 0)
            return;
        
        IsDead = true;
        healthSlider.gameObject.SetActive(false);
    }
}
