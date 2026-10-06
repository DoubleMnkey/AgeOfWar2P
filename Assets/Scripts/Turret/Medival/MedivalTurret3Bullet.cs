using UnityEngine;

public class MedivalTurret3Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float hitRadius = 0.8f; 

    [Header("분출될 작은 불구슬 프리팹")]
    public GameObject subBulletPrefab;

    [Header("사운드 설정")]
    [SerializeField] private AudioClip explodeSFX;

    private Transform target;
    private Vector3 lastTargetPos;
    private float damage;
    private LayerMask enemyLayer;

    public void SetTarget(Transform _target, float _damage, LayerMask _enemyLayer)
    {
        target = _target;
        damage = _damage;
        enemyLayer = _enemyLayer;

        if (target != null)
        {
            lastTargetPos = GetTargetCenter(target);
        }
    }

    void Update()
    {
        Vector3 destination = target != null ? GetTargetCenter(target) : lastTargetPos;
        Vector3 dir = (destination - transform.position).normalized;
        transform.Translate(dir * speed * Time.deltaTime, Space.World);

        if (dir != Vector3.zero)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        if (Vector3.Distance(transform.position, destination) < 0.4f)
        {
            Explode();
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

    void Explode()
    {
        if (explodeSFX != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(explodeSFX);
        }

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

        if (subBulletPrefab != null)
        {
            float[] baseDirX = new float[] { -1f, 0f, 1f };

            for (int i = 0; i < 3; i++)
            {
                GameObject subObj = Instantiate(subBulletPrefab, transform.position, Quaternion.identity);
                MedivalTurret3SubBullet subScript = subObj.GetComponent<MedivalTurret3SubBullet>();

                if (subScript != null)
                {
                    float randomX = (baseDirX[i] * Random.Range(2.0f, 4.0f)) + Random.Range(-0.8f, 0.8f);
                    float randomY = Random.Range(-0.5f, 0.2f);

                    Vector3 targetLandPos = transform.position + new Vector3(randomX, randomY, 0f);
                    subScript.Init(targetLandPos, damage / 2, enemyLayer);
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