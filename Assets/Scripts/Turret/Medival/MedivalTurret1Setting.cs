using UnityEngine;

public class MedivalTurret1Setting : TurretControl
{
    protected override void Attack(Transform target)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        MedivalTurret1Bullet bullet = bulletObj.GetComponent<MedivalTurret1Bullet>();
        if (bullet != null)
        {
            bullet.SetTarget(target, damage);
        }
    }
}