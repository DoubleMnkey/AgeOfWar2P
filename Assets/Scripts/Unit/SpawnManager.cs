using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [System.Serializable]
    public struct UnitBuildInfo
    {
        public GameObject unitPrefab;   
        public float buildTime;  
    }

    [System.Serializable]
    public struct AgeUnitData
    {
        public string ageName;   
        public UnitBuildInfo[] unitBuildInfos; 
    }

    public struct ProductionRequest
    {
        public int ageIndex;
        public bool isLeft;
        public int unitID;
    }

    [Header("시대별 유닛 데이터")]
    [SerializeField] private AgeUnitData[] ageUnitDatabase;

    [Header("진영별 게이지바 연결")]
    [SerializeField] private GaugeBar leftGaugeBar;
    [SerializeField] private GaugeBar rightGaugeBar;

    [Header("진영별 생산 대기열 UI (회색 Square 5개씩)")]
    [SerializeField] private GameObject[] leftQueueIcons; 
    [SerializeField] private GameObject[] rightQueueIcons;

    [Header("스폰 위치")]
    [SerializeField] private Transform leftSpawnPoint;
    [SerializeField] private Transform rightSpawnPoint;

    [Header("스폰 간격 설정")]
    [SerializeField] private float extraPadding = 0.3f;

    [Header("레이어 이름 설정")]
    [SerializeField] private string leftUnitLayerName = "LeftUnit";
    [SerializeField] private string rightUnitLayerName = "RightUnit";

    private int globalSpawnOrder = 0;

    private Queue<ProductionRequest> leftProductionQueue = new Queue<ProductionRequest>();
    private Queue<ProductionRequest> rightProductionQueue = new Queue<ProductionRequest>();

    private bool isLeftProducing = false;
    private bool isRightProducing = false;

    private Queue<UnitControl> leftSpawnQueue = new Queue<UnitControl>();
    private UnitControl lastActivatedLeftUnit = null;

    private Queue<UnitControl> rightSpawnQueue = new Queue<UnitControl>();
    private UnitControl lastActivatedRightUnit = null;

    private void Update()
    {
        ProcessProductionQueue(true); 
        ProcessProductionQueue(false); 

        ProcessQueue(leftSpawnQueue, leftSpawnPoint, ref lastActivatedLeftUnit);
        ProcessQueue(rightSpawnQueue, rightSpawnPoint, ref lastActivatedRightUnit);
    }

    public bool ProduceUnit(int ageIndex, bool isLeft, int unitID) 
    {
        Queue<ProductionRequest> queue = isLeft ? leftProductionQueue : rightProductionQueue;

        if (queue.Count >= 5)
        {
            return false; 
        }

        if (ageIndex < 0 || ageIndex >= ageUnitDatabase.Length) return false;
        if (unitID < 0 || unitID >= ageUnitDatabase[ageIndex].unitBuildInfos.Length) return false;

        queue.Enqueue(new ProductionRequest
        {
            ageIndex = ageIndex,
            isLeft = isLeft,
            unitID = unitID
        });

        UpdateQueueUI(isLeft);

        return true; 
    }

    private void ProcessProductionQueue(bool isLeft)
    {
        bool isProducing = isLeft ? isLeftProducing : isRightProducing;
        Queue<ProductionRequest> queue = isLeft ? leftProductionQueue : rightProductionQueue;
        GaugeBar gaugeBar = isLeft ? leftGaugeBar : rightGaugeBar;

        if (!isProducing && queue.Count > 0)
        {
            ProductionRequest currentRequest = queue.Dequeue();
            UpdateQueueUI(isLeft); 

            float buildTime = ageUnitDatabase[currentRequest.ageIndex].unitBuildInfos[currentRequest.unitID].buildTime;

            if (isLeft) isLeftProducing = true;
            else isRightProducing = true;

            gaugeBar.StartProduction(buildTime, () =>
            {
                RealSpawnUnit(currentRequest.ageIndex, currentRequest.isLeft, currentRequest.unitID);
                if (isLeft) isLeftProducing = false;
                else isRightProducing = false;
            });
        }
    }

    private void UpdateQueueUI(bool isLeft)
    {
        GameObject[] icons = isLeft ? leftQueueIcons : rightQueueIcons;
        Queue<ProductionRequest> queue = isLeft ? leftProductionQueue : rightProductionQueue;

        if (icons == null) return;

        int currentQueueCount = queue.Count;
        for (int i = 0; i < icons.Length; i++)
        {
            if (icons[i] != null)
            {
                icons[i].SetActive(i < currentQueueCount);
            }
        }
    }

    private void RealSpawnUnit(int ageIndex, bool isLeft, int unitID)
    {
        GameObject prefabToSpawn = ageUnitDatabase[ageIndex].unitBuildInfos[unitID].unitPrefab;
        Transform spawnPoint = isLeft ? leftSpawnPoint : rightSpawnPoint;
        Quaternion rotation = isLeft ? Quaternion.identity : Quaternion.Euler(0, 180f, 0);

        GameObject newUnitObj = Instantiate(prefabToSpawn, spawnPoint.position, rotation);
        UnitControl newUnit = newUnitObj.GetComponent<UnitControl>();

        if (newUnit == null) return;

        newUnit.eraIndex = ageIndex; 
        newUnit.unitID = unitID; 

        SetupUnitLayers(newUnitObj, newUnit, isLeft);

        globalSpawnOrder = (globalSpawnOrder + 1) % 30000;
        newUnit.SetSortingOrder(globalSpawnOrder);

        if (isLeft) leftSpawnQueue.Enqueue(newUnit);
        else rightSpawnQueue.Enqueue(newUnit);
    }

    private void SetupUnitLayers(GameObject unitObj, UnitControl unitControl, bool isLeft)
    {
        int leftLayer = LayerMask.NameToLayer(leftUnitLayerName);
        int rightLayer = LayerMask.NameToLayer(rightUnitLayerName);

        if (isLeft)
        {
            unitObj.layer = leftLayer;
            unitControl.isLeft = false;
            unitControl.allyLayer = 1 << leftLayer;
            unitControl.enemyLayer = 1 << rightLayer;
        }
        else
        {
            unitObj.layer = rightLayer;
            unitControl.isLeft = true;
            unitControl.allyLayer = 1 << rightLayer;
            unitControl.enemyLayer = 1 << leftLayer;
        }
    }
    private void ProcessQueue(Queue<UnitControl> queue, Transform spawnPoint, ref UnitControl lastActivatedUnit)
    {
        if (queue.Count == 0) return;

        UnitControl nextUnit = queue.Peek();

        if (lastActivatedUnit == null || lastActivatedUnit.IsDead)
        {
            ActivateNext(queue, ref lastActivatedUnit);
            return;
        }

        float requiredDistance = lastActivatedUnit.GetHalfWidth() + nextUnit.GetHalfWidth() + extraPadding;
        float currentDistance = Vector3.Distance(spawnPoint.position, lastActivatedUnit.transform.position);

        if (currentDistance >= requiredDistance)
        {
            ActivateNext(queue, ref lastActivatedUnit);
        }
    }

    private void ActivateNext(Queue<UnitControl> queue, ref UnitControl lastActivatedUnit)
    {
        UnitControl unitToActivate = queue.Dequeue();
        unitToActivate.ActivateMovement();
        lastActivatedUnit = unitToActivate;
    }
}