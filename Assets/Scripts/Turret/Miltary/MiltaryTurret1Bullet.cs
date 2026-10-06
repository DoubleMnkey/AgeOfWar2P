using UnityEngine;

public class MiltaryTurret1Bullet : MonoBehaviour
{
    public float speed = 20f;
    private Transform target;
    private float damage;

    public void SetTarget(Transform _target, float _damage)
    {
        target = _target;
        damage = _damage;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 destination = GetTargetCenter(target);
        Vector3 dir = (destination - transform.position).normalized;
        transform.Translate(dir * speed * Time.deltaTime, Space.World);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (Vector3.Distance(transform.position, destination) < 0.3f)
        {
            HitTarget();
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

    void HitTarget()
    {
        UnitControl unit = target.GetComponent<UnitControl>();
        if (unit != null && !unit.IsDead)
        {
            unit.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}