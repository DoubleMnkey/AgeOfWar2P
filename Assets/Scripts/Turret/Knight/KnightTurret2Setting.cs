using UnityEngine;

public class KnightTurret2Setting : TurretControl
{
    protected override void Attack(Transform target)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        KnightTurret2Bullet bullet = bulletObj.GetComponent<KnightTurret2Bullet>();
        if (bullet != null)
        {
            bullet.Init(target, damage, enemyLayer);
        }
    }
}