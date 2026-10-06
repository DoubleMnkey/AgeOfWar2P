using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public interface IDamageable
{
    void TakeDamage(float damage);
    bool IsDead { get; }
}

public class BaseHealth : MonoBehaviour, IDamageable
{
    [Header("기지 설정")]
    public float maxHealth = 500f; 
    public float currentHealth;
    public bool isLeftBase = true; 

    [Header("UI 연동 (체력바 & 텍스트)")]
    [SerializeField] private Image redGaugeImage;
    [SerializeField] private TextMeshProUGUI hpTextTMP;

    [Header("게임 오버 씬 설정")]
    [SerializeField] private string nextSceneName = "GameOverScene";

    public bool IsDead => currentHealth <= 0;

    private void Awake()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth > maxHealth) currentHealth = maxHealth;
        if (currentHealth < 0) currentHealth = 0;

        UpdateUI();

        if (IsDead)
        {
            Die();
        }
    }

    public void SetEraMaxHealth(float newMaxHealth)
    {
        float damageTaken = maxHealth - currentHealth;

        maxHealth = newMaxHealth;

        currentHealth = Mathf.Max(0f, maxHealth - damageTaken);

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (redGaugeImage != null && maxHealth > 0)
        {
            redGaugeImage.fillAmount = currentHealth / maxHealth;
        }

        if (hpTextTMP != null)
        {
            hpTextTMP.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
        }
    }

    private void Die()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}