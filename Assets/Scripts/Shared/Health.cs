using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour, IDamageable
{
    public bool IsDead { get; private set; }

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
        if (IsDead) 
            return;
            
        currentHealth -= damage;
        healthSlider.value = currentHealth;
        HandleDeath();
    }

    public void HandleDeath()
    {
        if (currentHealth > 0)
            return;
        
        IsDead = true;
    }
}
