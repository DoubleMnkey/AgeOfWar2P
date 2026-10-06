using System.Collections;
using UnityEngine;

public class SpecialSkillManager : MonoBehaviour
{
    [Header("Special Skill Prefabs")]
    public GameObject meteorPrefab; 
    public GameObject arrowPrefab; 
    public GameObject healEffectPrefab; 
    public GameObject jetPrefab;
    public GameObject futureLaserPrefab;

    [Header("Cooldown Settings")]
    public float[] ageMaxCooldowns = new float[5] { 60f, 60f, 60f, 60f, 60f };

    private float leftMaxCooldown = 60f;
    private float rightMaxCooldown = 60f;

    private float leftCooldownTimer = 0f;
    private float rightCooldownTimer = 0f;

    public PlayerUIManager uiManager; 

    void Start()
    {
        leftCooldownTimer = 0f;
        rightCooldownTimer = 0f;

        leftMaxCooldown = GetCooldownForAge(1);
        rightMaxCooldown = GetCooldownForAge(1);

        if (uiManager != null)
        {
            uiManager.UpdateSpecialCooldownUI(true, 0f, leftMaxCooldown);
            uiManager.UpdateSpecialCooldownUI(false, 0f, rightMaxCooldown);
        }
    }

    void Update()
    {
        if (leftCooldownTimer > 0f)
        {
            leftCooldownTimer -= Time.deltaTime;
            if (leftCooldownTimer < 0f) leftCooldownTimer = 0f;
            if (uiManager != null) uiManager.UpdateSpecialCooldownUI(true, leftCooldownTimer, leftMaxCooldown);
        }

        if (rightCooldownTimer > 0f)
        {
            rightCooldownTimer -= Time.deltaTime;
            if (rightCooldownTimer < 0f) rightCooldownTimer = 0f;
            if (uiManager != null) uiManager.UpdateSpecialCooldownUI(false, rightCooldownTimer, rightMaxCooldown);
        }
    }

    private float GetCooldownForAge(int age)
    {
        if (ageMaxCooldowns == null || ageMaxCooldowns.Length == 0) return 60f;

        int index = Mathf.Clamp(age - 1, 0, ageMaxCooldowns.Length - 1);

        return ageMaxCooldowns[index];
    }

    public void UseSpecial(bool isLeft, int playerAge)
    {
        if (!CanUseSpecial(isLeft))
        {
            return;
        }

        float selectedCooldown = GetCooldownForAge(playerAge);

        if (isLeft)
        {
            leftMaxCooldown = selectedCooldown;
            leftCooldownTimer = leftMaxCooldown;
        }
        else
        {
            rightMaxCooldown = selectedCooldown;
            rightCooldownTimer = rightMaxCooldown;
        }

        LayerMask enemyLayer = isLeft ? (LayerMask)(1 << LayerMask.NameToLayer("RightUnit") | 1 << LayerMask.NameToLayer("RightBase"))
                                      : (LayerMask)(1 << LayerMask.NameToLayer("LeftUnit") | 1 << LayerMask.NameToLayer("LeftBase"));

        switch (playerAge)
        {
            case 1:
                if (meteorPrefab != null)
                {
                    StartCoroutine(MeteorRain(enemyLayer));
                }
                break;

            case 2:
                if (arrowPrefab != null)
                {
                    StartCoroutine(ArrowRain(enemyLayer));
                }
                break;

            case 3:
                if (healEffectPrefab != null)
                {
                    StartCoroutine(ApplyHealAndInvincible(isLeft));
                }
                break;

            case 4:
                if (jetPrefab != null)
                {
                    SpawnAirstrike(isLeft, enemyLayer);
                }
                break;

            case 5:
                if (futureLaserPrefab != null)
                {
                    StartCoroutine(FutureLaserSweep(isLeft, enemyLayer));
                }
                break;
        }
    }

    IEnumerator MeteorRain(LayerMask targetLayer)
    {
        float duration = 5f;
        float spawnInterval = 0.3f;
        float timer = 0f;

        while (timer < duration)
        {
            float randomX = Random.Range(-11f, 11f);
            Vector3 spawnPosition = new Vector3(randomX, 14f, 0f);

            GameObject meteorObj = Instantiate(meteorPrefab, spawnPosition, Quaternion.identity);

            Meteor meteorScript = meteorObj.GetComponent<Meteor>();
            if (meteorScript != null)
            {
                meteorScript.targetLayer = targetLayer;
            }

            yield return new WaitForSeconds(spawnInterval);
            timer += spawnInterval;
        }
    }

    IEnumerator ArrowRain(LayerMask targetLayer)
    {
        float duration = 5.0f;
        float spawnInterval = 0.1f;
        float timer = 0f;

        while (timer < duration)
        {
            float randomX = Random.Range(-11f, 11f);
            Vector3 spawnPosition = new Vector3(randomX, 12f, 0f);

            GameObject arrowObj = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity);

            Arrow arrowScript = arrowObj.GetComponent<Arrow>();
            if (arrowScript != null)
            {
                arrowScript.targetLayer = targetLayer;
            }

            yield return new WaitForSeconds(spawnInterval);
            timer += spawnInterval;
        }
    }

    IEnumerator ApplyHealAndInvincible(bool isLeft)
    {
        string myLayerName = isLeft ? "LeftUnit" : "RightUnit";
        int myLayer = LayerMask.NameToLayer(myLayerName);

        UnitControl[] allUnits = FindObjectsByType<UnitControl>(FindObjectsSortMode.None);

        foreach (UnitControl unit in allUnits)
        {
            if (unit.gameObject.layer == myLayer && !unit.IsDead)
            {
                StartCoroutine(UnitBuffRoutine(unit));
            }
        }

        yield return null;
    }

    IEnumerator UnitBuffRoutine(UnitControl unit)
    {
        float duration = 8f;
        float totalHealAmount = 80f;
        float healPerSecond = totalHealAmount / duration;

        unit.IsInvincible = true;

        GameObject effectInstance = Instantiate(healEffectPrefab, unit.transform.position, Quaternion.identity);

        HealEffectFollower follower = effectInstance.GetComponent<HealEffectFollower>();
        if (follower != null)
        {
            follower.Setup(unit.transform);
        }

        float timer = 0f;
        while (timer < duration)
        {
            if (unit == null) break;

            unit.Heal(healPerSecond * Time.deltaTime);

            timer += Time.deltaTime;
            yield return null;
        }

        if (unit != null)
        {
            unit.IsInvincible = false;
        }

        if (effectInstance != null)
        {
            Destroy(effectInstance);
        }
    }

    private void SpawnAirstrike(bool isLeft, LayerMask enemyLayer)
    {
        float startX = isLeft ? -15f : 15f;
        float endX = isLeft ? 15f : -15f;
        float flyHeight = 6f;

        GameObject jetObj = Instantiate(jetPrefab);
        AirstrikeJet jet = jetObj.GetComponent<AirstrikeJet>();
        if (jet != null)
        {
            jet.Initialize(isLeft, enemyLayer, startX, endX, flyHeight);
        }
    }

    IEnumerator FutureLaserSweep(bool isLeft, LayerMask enemyLayer)
    {
        float startX = isLeft ? -13f : 13f;
        float step = isLeft ? 3f : -3f;

        for (int i = 0; i < 10; i++)
        {
            float currentX = startX + (step * i);

            GameObject laser = Instantiate(
                futureLaserPrefab,
                new Vector3(currentX, 2f, 0f),
                Quaternion.identity
            );

            FutureLaserBeam beam = laser.GetComponent<FutureLaserBeam>();
            if (beam != null)
            {
                beam.targetLayer = enemyLayer;
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    public bool CanUseSpecial(bool isLeft)
    {
        return isLeft ? (leftCooldownTimer <= 0f) : (rightCooldownTimer <= 0f);
    }

    public float GetRemainingCooldown(bool isLeft)
    {
        return isLeft ? leftCooldownTimer : rightCooldownTimer;
    }

    public void ResetCooldown(bool isLeft)
    {
        if (isLeft)
        {
            leftCooldownTimer = 0f;
            if (uiManager != null) uiManager.UpdateSpecialCooldownUI(true, 0f, leftMaxCooldown);
        }
        else
        {
            rightCooldownTimer = 0f;
            if (uiManager != null) uiManager.UpdateSpecialCooldownUI(false, 0f, rightMaxCooldown);
        }
    }
}