using UnityEngine;

public class FutureTurret2Bullet : MonoBehaviour
{
    [Header("가속 연출 설정")]
    public float initialSpeed = 1f;
    public float acceleration = 45f;
    public float maxSpeed = 50f;

    [Header("타격 설정")]
    public float splashRadius = 2.5f;

    private float currentSpeed;
    private Transform target;
    private Vector3 lastTargetPos;
    private float damage;
    private LayerMask enemyLayer;

    public void SetTarget(Transform _target, float _damage, LayerMask _enemyLayer)
    {
        target = _target;
        damage = _damage;
        enemyLayer = _enemyLayer;

        currentSpeed = initialSpeed;

        if (target != null)
        {
            lastTargetPos = GetTargetCenter(target);
        }
    }

    void Update()
    {
        Vector3 destination = target != null ? GetTargetCenter(target) : lastTargetPos;
        if (target != null) lastTargetPos = GetTargetCenter(target);

        currentSpeed += acceleration * Time.deltaTime;
        currentSpeed = Mathf.Min(currentSpeed, maxSpeed);

        Vector3 dir = (destination - transform.position).normalized;
        transform.Translate(dir * currentSpeed * Time.deltaTime, Space.World);

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
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, splashRadius, enemyLayer);

        foreach (Collider2D hit in hits)
        {
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
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, splashRadius);
    }
}