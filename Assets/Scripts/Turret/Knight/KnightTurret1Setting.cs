using UnityEngine;

public class KnightTurret1Setting : TurretControl
{
    protected override void Attack(Transform target)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        KnightTurret1Bullet bullet = bulletObj.GetComponent<KnightTurret1Bullet>();
        if (bullet != null)
        {
            bullet.Init(target, damage, enemyLayer);
        }
    }
}