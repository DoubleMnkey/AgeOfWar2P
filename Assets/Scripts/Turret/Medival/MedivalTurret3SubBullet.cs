using UnityEngine;

public class MedivalTurret3SubBullet : MonoBehaviour
{
    public float baseSpeed = 3f;
    public float baseArcHeight = 1.8f;
    public float hitRadius = 0.6f; 
    public GameObject fireAreaPrefab; 

    [Header("사운드 설정")]
    [SerializeField] private AudioClip landSFX;

    private Vector3 startPos;
    private Vector3 targetPos;
    private float progress = 0f;
    private float damage;
    private LayerMask enemyLayer;

    private float speed;
    private float arcHeight;

    public void Init(Vector3 _targetPos, float _damage, LayerMask _enemyLayer)
    {
        startPos = transform.position;
        targetPos = _targetPos;
        damage = _damage;
        enemyLayer = _enemyLayer;

        speed = baseSpeed * Random.Range(0.85f, 1.15f);
        arcHeight = baseArcHeight * Random.Range(0.8f, 1.25f);
        progress = 0f;
    }

    void Update()
    {
        progress += Time.deltaTime * speed;
        float currentProgress = Mathf.Min(progress, 1.0f);

        Vector3 currentPos = Vector3.Lerp(startPos, targetPos, currentProgress);
        currentPos.y += Mathf.Sin(currentProgress * Mathf.PI) * arcHeight;
        transform.position = currentPos;

        if (currentProgress >= 1.0f)
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
                unit.TakeDamage(damage); 
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
            GameObject fireObj = Instantiate(fireAreaPrefab, transform.position, Quaternion.identity);
            MedivalTurret3FireArea fireScript = fireObj.GetComponent<MedivalTurret3FireArea>();
            if (fireScript != null)
            {
                fireScript.Init();
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
    }
}