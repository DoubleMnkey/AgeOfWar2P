using UnityEngine;

public class FutureTurret1Setting : TurretControl
{
    protected override void Attack(Transform target)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        FutureTurret1Bullet bullet = bulletObj.GetComponent<FutureTurret1Bullet>();
        if (bullet != null)
        {
            bullet.SetTarget(target, damage);
        }
    }
}