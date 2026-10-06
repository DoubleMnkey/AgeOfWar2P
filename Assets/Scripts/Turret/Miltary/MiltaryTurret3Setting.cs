using UnityEngine;

public class MiltaryTurret3Setting : TurretControl
{
    [Header("½Ö¿­ ÃÑ¿­ À§Ä¡ (2°³ ÇÒ´ç)")]
    public Transform[] dualFirePoints = new Transform[2];

    private int currentFireIndex = 0; 

    protected override void Attack(Transform target)
    {
        if (bulletPrefab == null) return;

        Transform spawnPoint = firePoint; 

        if (dualFirePoints != null && dualFirePoints.Length >= 2 && dualFirePoints[0] != null && dualFirePoints[1] != null)
        {
            spawnPoint = dualFirePoints[currentFireIndex];
            currentFireIndex = (currentFireIndex + 1) % 2; 
        }

        if (spawnPoint == null) return;

        GameObject bulletObj = Instantiate(bulletPrefab, spawnPoint.position, Quaternion.identity);

        MiltaryTurret3Bullet bullet = bulletObj.GetComponent<MiltaryTurret3Bullet>();
        if (bullet != null)
        {
            bullet.SetTarget(target, damage);
        }
    }
}