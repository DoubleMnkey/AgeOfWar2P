using UnityEngine;

public class KnightTurret2Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float hitRadius = 0.8f;
    public GameObject subBulletPrefab;

    [Header("사운드 설정")]
    [SerializeField] private AudioClip shatterSFX;

    private Vector3 targetPosition;
    private float damage;
    private LayerMask enemyLayer;

    public void Init(Transform target, float _damage, LayerMask _enemyLayer)
    {
        targetPosition = target != null ? GetTargetCenter(target) : transform.position;
        damage = _damage;
        enemyLayer = _enemyLayer;
    }

    void Update()
    {
        if (targetPosition == Vector3.zero)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (targetPosition - transform.position).normalized;
        transform.Translate(dir * speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, targetPosition) < 0.2f)
        {
            Shatter();
        }
    }

    private Vector3 GetTargetCenter(Transform targetTransform)
    {
        if (targetTransform == null) return Vector3.zero;

        Collider2D col = targetTransform.GetComponent<Collider2D>();
        if (col != null)
        {
            return col.bounds.center;
        }

        return targetTransform.position + Vector3.up * 1.0f;
    }

    void Shatter()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, hitRadius, enemyLayer);
        foreach (Collider2D hit in hits)
        {
            UnitControl unit = hit.GetComponent<UnitControl>();
            if (unit != null && !unit.IsDead)
            {
                unit.TakeDamage(damage);
                break;
            }
        }

        if (shatterSFX != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(shatterSFX);
        }

        if (subBulletPrefab != null)
        {
            float[] baseDirX = new float[] { -1f, 0f, 1f };

            for (int i = 0; i < 3; i++)
            {
                GameObject sub = Instantiate(subBulletPrefab, transform.position, Quaternion.identity);
                FireSubBullet subScript = sub.GetComponent<FireSubBullet>();
                if (subScript != null)
                {
                    float randomX = (baseDirX[i] * Random.Range(2.0f, 4.0f)) + Random.Range(-0.8f, 0.8f);
                    float randomY = Random.Range(-0.5f, 0.2f);

                    Vector3 landingTarget = transform.position + new Vector3(randomX, randomY, 0f);
                    subScript.Launch(landingTarget, damage / 2.0f, enemyLayer);
                }
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
    }
}