using UnityEngine;

public class FireSubBullet : MonoBehaviour
{
    public float baseSpeed = 3f; 
    public float baseArcHeight = 5f; 
    public float hitRadius = 0.6f; 
    public GameObject fireAreaPrefab; 

    [Header("사운드 설정")]
    [SerializeField] private AudioClip landSFX;

    private Vector3 startPos;
    private Vector3 targetPos;
    private float progress = 0f;
    private float subDamage;
    private LayerMask enemyLayer;
    private bool isLaunched = false;

    private float speed;
    private float arcHeight;

    public void Launch(Vector3 target, float damage, LayerMask layer)
    {
        startPos = transform.position; 
        targetPos = target;          
        subDamage = damage;
        enemyLayer = layer;

        speed = baseSpeed * Random.Range(0.85f, 1.15f);
        arcHeight = baseArcHeight * Random.Range(0.8f, 1.25f);

        progress = 0f;
        isLaunched = true;
    }

    void Update()
    {
        if (!isLaunched) return;

        progress += Time.deltaTime * speed;
        float p = Mathf.Min(progress, 1.0f);

        Vector3 currentPos = Vector3.Lerp(startPos, targetPos, p);

        currentPos.y += Mathf.Sin(p * Mathf.PI) * arcHeight;
        transform.position = currentPos;

        if (p >= 1.0f)
        {
            OnLanding();
        }
    }

    void OnLanding()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, hitRadius, enemyLayer);
        bool hitEnemy = false;

        foreach (Collider2D hit in hits)
        {
            UnitControl unit = hit.GetComponent<UnitControl>();
            if (unit != null && !unit.IsDead)
            {
                unit.TakeDamage(subDamage); 
                hitEnemy = true;
                break;
            }
        }

        if (landSFX != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(landSFX);
        }

        if (!hitEnemy && fireAreaPrefab != null)
        {
            Instantiate(fireAreaPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
    }
}