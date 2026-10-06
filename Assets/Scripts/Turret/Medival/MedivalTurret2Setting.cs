using UnityEngine;

public class MedivalTurret2Setting : TurretControl
{
    protected override void Attack(Transform target)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        MedivalTurret2Bullet bullet = bulletObj.GetComponent<MedivalTurret2Bullet>();
        if (bullet != null)
        {
            bullet.Init(target, damage, enemyLayer);
        }
    }
}