using UnityEngine;

public class GoldPopupManager : MonoBehaviour
{
    public static GoldPopupManager Instance { get; private set; }

    [Header("ÇÁ¸®ÆÕ ¿¬°á")]
    [SerializeField] private GameObject goldPopupPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowGoldPopup(Vector3 spawnPosition, int amount)
    {
        if (goldPopupPrefab == null || amount <= 0) return;

        Vector3 spawnPos = spawnPosition + Vector3.up * 1.2f;

        GameObject popupObj = Instantiate(goldPopupPrefab, spawnPos, Quaternion.identity);
        GoldPopup popup = popupObj.GetComponent<GoldPopup>();

        if (popup != null)
        {
            popup.Setup(amount);
        }
    }
}