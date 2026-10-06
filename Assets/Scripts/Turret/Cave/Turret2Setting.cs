using UnityEngine;

public class Turret2Setting : TurretControl
{
    protected override void Attack(Transform target)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        CaveTurret2Bullet bullet = bulletObj.GetComponent<CaveTurret2Bullet>();
        if (bullet != null)
        {
            bullet.Init(target, damage, enemyLayer);
        }
    }
}