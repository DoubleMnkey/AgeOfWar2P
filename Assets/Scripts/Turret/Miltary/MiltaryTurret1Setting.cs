using UnityEngine;

public class MiltaryTurret1Setting : TurretControl
{
    protected override void Attack(Transform target)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        MiltaryTurret1Bullet bullet = bulletObj.GetComponent<MiltaryTurret1Bullet>();
        if (bullet != null)
        {
            bullet.SetTarget(target, damage);
        }
    }
}