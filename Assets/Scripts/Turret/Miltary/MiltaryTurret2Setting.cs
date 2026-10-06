using UnityEngine;

public class MiltaryTurret2Setting : TurretControl
{
    protected override void Attack(Transform target)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        MiltaryTurret2Bullet bullet = bulletObj.GetComponent<MiltaryTurret2Bullet>();
        if (bullet != null)
        {
            bullet.SetTarget(target, damage, enemyLayer);
        }
    }
}