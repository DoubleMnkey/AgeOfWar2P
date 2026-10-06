using UnityEngine;

public class Turret1Setting : TurretControl
{
    protected override void Attack(Transform target)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        CaveTurret1Bullet bullet = bulletObj.GetComponent<CaveTurret1Bullet>();
        if (bullet != null)
        {
            bullet.SetTarget(target, damage);
        }
    }
}