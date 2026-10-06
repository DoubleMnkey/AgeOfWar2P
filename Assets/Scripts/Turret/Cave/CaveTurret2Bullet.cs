using UnityEngine;

public class CaveTurret2Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float splashRadius = 2f;

    private Transform target;
    private Vector3 targetPosition;
    private float damage;
    private LayerMask enemyLayer;
    private bool isInitialized = false;

    public void Init(Transform _target, float _damage, LayerMask _enemyLayer)
    {
        target = _target;
        damage = _damage;
        enemyLayer = _enemyLayer;

        if (target != null)
        {
            targetPosition = GetTargetCenter(target);
        }
        else
        {
            targetPosition = transform.position;
        }

        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized) return;

        Vector3 destination = target != null ? GetTargetCenter(target) : targetPosition;
        Vector3 dir = (destination - transform.position).normalized;

        transform.Translate(dir * speed * Time.deltaTime, Space.World);

        if (dir != Vector3.zero)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        if (Vector3.Distance(transform.position, destination) < 0.3f)
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
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, splashRadius, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;

            UnitControl unit = hit.GetComponent<UnitControl>();
            if (unit != null && !unit.IsDead)
            {
                unit.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(transform.position, splashRadius);
    }
}