using UnityEngine;

public class MedivalTurret3Setting : TurretControl
{
    protected override void Attack(Transform target)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        MedivalTurret3Bullet bullet = bulletObj.GetComponent<MedivalTurret3Bullet>();
        if (bullet != null)
        {
            bullet.SetTarget(target, damage, enemyLayer);
        }
    }
}