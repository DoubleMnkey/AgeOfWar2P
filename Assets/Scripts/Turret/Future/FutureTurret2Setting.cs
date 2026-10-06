using UnityEngine;

public class FutureTurret2Setting : TurretControl
{
    protected override void Attack(Transform target)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        FutureTurret2Bullet bullet = bulletObj.GetComponent<FutureTurret2Bullet>();
        if (bullet != null)
        {
            bullet.SetTarget(target, damage, enemyLayer);
        }
    }
}