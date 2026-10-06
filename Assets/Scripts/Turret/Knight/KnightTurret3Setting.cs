using UnityEngine;
using System.Collections;

public class KnightTurret3Setting : TurretControl
{
    [Header("기름 쏟기 연출 설정")]
    public float pourDuration = 3.0f;  
    public float spawnInterval = 0.1f; 

    protected override void Attack(Transform target)
    {
        if (bulletPrefab == null || firePoint == null) return;

        StartCoroutine(PourOilRoutine());
    }

    private IEnumerator PourOilRoutine()
    {
        float elapsed = 0f;

        while (elapsed < pourDuration)
        {
            if (firePoint != null && bulletPrefab != null)
            {
                Vector3 spawnPos = firePoint.position;
                spawnPos.x += Random.Range(-0.25f, 0.25f);

                GameObject oilObj = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

                KnightTurret3Oil oil = oilObj.GetComponent<KnightTurret3Oil>();
                if (oil != null)
                {
                    oil.Init(damage, enemyLayer);
                }
            }

            elapsed += spawnInterval;
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}