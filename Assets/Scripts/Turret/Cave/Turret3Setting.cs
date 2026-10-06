using UnityEngine;

public class Turret3Setting : TurretControl
{
    public new void OnFireFrame()
    {
        base.OnFireFrame();
    }

    protected override void Attack(Transform target)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        CaveTurret3Bullet bullet = bulletObj.GetComponent<CaveTurret3Bullet>();
        if (bullet != null)
        {
            bullet.SetTarget(target, damage, enemyLayer);
        }
    }
}