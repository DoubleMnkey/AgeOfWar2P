using UnityEngine;
using System.Collections.Generic;

public class KnightTurret3Oil : MonoBehaviour
{
    public float fallSpeed = 8f;
    public float maxDistance = 15f;  
    public float groundY = -8f;      
    public float damageRadius = 1.2f;
    public float damage = 20f;
    public float damageInterval = 0.2f; 

    private Vector3 startPos;
    private LayerMask enemyLayer;
    private Dictionary<UnitControl, float> hitCooldowns = new Dictionary<UnitControl, float>();

    public void Init(float _damage, LayerMask _enemyLayer)
    {
        damage = _damage;
        enemyLayer = _enemyLayer;
        startPos = transform.position;
    }

    void Update()
    {
        float spreadX = Random.Range(-0.5f, 0.5f) * Time.deltaTime;
        transform.Translate(new Vector3(spreadX, -fallSpeed * Time.deltaTime, 0), Space.World);

        List<UnitControl> keys = new List<UnitControl>(hitCooldowns.Keys);
        foreach (var unit in keys)
        {
            hitCooldowns[unit] -= Time.deltaTime;
            if (hitCooldowns[unit] <= 0f) hitCooldowns.Remove(unit);
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, damageRadius, enemyLayer);
        foreach (Collider2D hit in hits)
        {
            UnitControl unit = hit.GetComponent<UnitControl>();
            if (unit != null && !unit.IsDead)
            {
                if (!hitCooldowns.ContainsKey(unit))
                {
                    unit.TakeDamage(damage);
                    hitCooldowns[unit] = damageInterval;
                }
            }
        }

        if (Vector3.Distance(startPos, transform.position) >= Mathf.Abs(maxDistance) || transform.position.y <= groundY)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}