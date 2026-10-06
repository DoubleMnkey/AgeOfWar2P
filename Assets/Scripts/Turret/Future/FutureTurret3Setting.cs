using UnityEngine;

public class FutureTurret3Setting : TurretControl
{
    protected override void Attack(Transform target)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        FutureTurret3Bullet bullet = bulletObj.GetComponent<FutureTurret3Bullet>();
        if (bullet != null)
        {
            bullet.SetTarget(target, damage);
        }
    }
}