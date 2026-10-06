using UnityEngine;

public class TurretControl : MonoBehaviour
{
    [Header("터렛 기본 설정")]
    public float attackRange = 5f;       
    public float attackRate = 1f;       
    public float damage = 10f;             
    public LayerMask enemyLayer;         

    [Header("사운드 설정")]
    [SerializeField] protected AudioClip attackSFX; 

    [Header("연결 요소")]
    public Transform firePoint;         
    public GameObject bulletPrefab;      
    public Animator animator;           

    [Header("회전 추적 설정")]
    [SerializeField] private bool canRotate = true;       
    [SerializeField] private float rotationSpeed = 5f;     
    [SerializeField] private float defaultAngle = 0f;      
    [SerializeField] private float minAngle = -60f;        
    [SerializeField] private float maxAngle = 30f;      

    protected float attackTimer = 0f;
    private bool isFacingLeft = false;  

    public virtual void Init(bool isLeft)
    {
        enemyLayer = LayerMask.GetMask(isLeft ? "RightUnit" : "LeftUnit");
        isFacingLeft = !isLeft;
    }

    protected virtual void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.speed = attackRate > 0 ? attackRate : 1f;
        }
    }

    protected virtual void Update()
    {
        Transform target = GetClosestEnemy();

        if (canRotate)
        {
            if (target != null)
            {
                RotateTowardsTarget(GetTargetCenter(target));
            }
            else
            {
                ResetToIdleAngle();
            }
        }

        if (target == null)
        {
            attackTimer = 0f;
            return;
        }

        attackTimer += Time.deltaTime;
        float fireInterval = 1f / attackRate;

        if (attackTimer >= fireInterval)
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            attackTimer -= fireInterval;
        }
    }
    protected Vector3 GetTargetCenter(Transform target)
    {
        if (target == null) return Vector3.zero;

        Collider2D col = target.GetComponent<Collider2D>();
        if (col != null)
        {
            return col.bounds.center;
        }

        return target.position + Vector3.up * 3.0f;
    }

    private void RotateTowardsTarget(Vector3 targetPos)
    {
        Vector2 direction = targetPos - transform.position;

        if (isFacingLeft)
        {
            direction.x = -direction.x;
            direction.y = -direction.y;
        }

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        targetAngle = Mathf.Clamp(targetAngle, minAngle, maxAngle);

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void ResetToIdleAngle()
    {
        Quaternion defaultRotation = Quaternion.Euler(0f, 0f, defaultAngle);
        transform.rotation = Quaternion.Slerp(transform.rotation, defaultRotation, Time.deltaTime * rotationSpeed);
    }

    public void OnFireFrame()
    {
        Transform target = GetClosestEnemy();
        if (target != null)
        {
            if (attackSFX != null && SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySFX(attackSFX);
            }
            Attack(target);
        }
    }
    protected virtual void Attack(Transform target)
    {
    }

    protected Transform GetClosestEnemy()
    {
        UnitControl[] allUnits = FindObjectsOfType<UnitControl>();

        Transform closestEnemy = null;
        float minXDistance = Mathf.Infinity;

        foreach (UnitControl unit in allUnits)
        {
            if (unit == null || unit.IsDead) continue;

            if (((1 << unit.gameObject.layer) & enemyLayer) == 0) continue;

            float xDistance = Mathf.Abs(transform.position.x - unit.transform.position.x);

            if (xDistance <= attackRange && xDistance < minXDistance)
            {
                minXDistance = xDistance;
                closestEnemy = unit.transform;
            }
        }

        return closestEnemy;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        float direction = isFacingLeft ? -1f : 1f;
        float boxHeight = 10f;

        Vector3 center = transform.position + new Vector3((attackRange / 2f) * direction, -boxHeight / 2f, 0f);
        Vector3 size = new Vector3(attackRange, boxHeight, 0f);

        Gizmos.DrawWireCube(center, size);
    }
}