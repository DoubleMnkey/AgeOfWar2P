using UnityEngine;

public class TurretManager : MonoBehaviour
{
    [Header("Turret Prefabs Per Age")]
    public MenuManager.AgeTurretPrefabData[] ageTurretPrefabs;

    [Header("Turret Positions")]
    public Transform[] leftTurretPositions = new Transform[4];
    public Transform[] rightTurretPositions = new Transform[4];

    private int[] leftSlotTurretType = new int[4] { -1, -1, -1, -1 };
    private int[] rightSlotTurretType = new int[4] { -1, -1, -1, -1 };

    private int[] leftTurretPrices = new int[4];
    private int[] rightTurretPrices = new int[4];

    public void ClearTurretPositionsOnStart()
    {
        ClearPositions(leftTurretPositions, leftSlotTurretType);
        ClearPositions(rightTurretPositions, rightSlotTurretType);
    }

    private void ClearPositions(Transform[] posArray, int[] slotTypes)
    {
        if (posArray == null) return;
        for (int i = 0; i < posArray.Length; i++)
        {
            slotTypes[i] = -1;
            if (posArray[i] != null)
            {
                SpriteRenderer sr = posArray[i].GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = null;
                    sr.enabled = false;
                }
            }
        }
    }
    public bool BuildTurretAtSlot(bool isLeft, PlayerData player, int slotIndex, int selectedType, MenuManager.GenerationImageData[] genData)
    {
        if (selectedType < 0 || selectedType >= 3 || slotIndex > player.unlockedSlots || slotIndex >= 4)
            return false;

        int[] slotTypes = isLeft ? leftSlotTurretType : rightSlotTurretType;
        if (slotTypes[slotIndex] != -1) return false;

        int ageIndex = Mathf.Clamp(player.age - 1, 0, 4);
        int cost = GameDatabase.TurretCost[ageIndex, selectedType];

        if (player.gold < cost) return false;

        Transform[] positions = isLeft ? leftTurretPositions : rightTurretPositions;
        Transform targetPos = positions[slotIndex];
        if (targetPos == null) return false;

        player.gold -= cost;

        int[] prices = isLeft ? leftTurretPrices : rightTurretPrices;
        prices[slotIndex] = cost;
        slotTypes[slotIndex] = selectedType;

        SpriteRenderer sr = targetPos.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = null;
            sr.enabled = false; 
        }

        if (ageTurretPrefabs != null && ageIndex < ageTurretPrefabs.Length)
        {
            GameObject prefab = ageTurretPrefabs[ageIndex].turretPrefabs[selectedType];
            if (prefab != null)
            {
                foreach (Transform child in targetPos) Destroy(child.gameObject);

                GameObject turretObj = Instantiate(prefab, targetPos.position, Quaternion.identity, targetPos);

                if (!isLeft)
                {
                    Vector3 scale = turretObj.transform.localScale;
                    scale.x = -Mathf.Abs(scale.x);
                    turretObj.transform.localScale = scale;
                }

                TurretControl turretScript = turretObj.GetComponent<TurretControl>();
                if (turretScript != null)
                {
                    turretScript.Init(isLeft);
                }
            }
        }

        return true;
    }

    public void SellTurretAtSlot(bool isLeft, PlayerData player, int slotIndex)
    {
        int[] slotTypes = isLeft ? leftSlotTurretType : rightSlotTurretType;
        if (slotIndex > player.unlockedSlots || slotIndex >= 4 || slotTypes[slotIndex] == -1) return;

        Transform[] positions = isLeft ? leftTurretPositions : rightTurretPositions;
        Transform targetPos = positions[slotIndex];

        if (targetPos != null)
        {
            SpriteRenderer sr = targetPos.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = null;
                sr.enabled = false;
            }
            foreach (Transform child in targetPos) Destroy(child.gameObject);
        }

        int[] turretPrices = isLeft ? leftTurretPrices : rightTurretPrices;
        int refund = turretPrices[slotIndex] / 2;

        player.gold += refund;
        slotTypes[slotIndex] = -1;
        turretPrices[slotIndex] = 0;
    }
}