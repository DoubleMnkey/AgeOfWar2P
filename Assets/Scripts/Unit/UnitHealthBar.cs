using UnityEngine;
using UnityEngine.UI;

public class UnitHealthBar : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private Image redHealthFillImage;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("위치 고정 설정")]
    [Tooltip("콜라이더가 없을 때 사용할 기본 Y축 높이")]
    [SerializeField] private float yOffset = 0.5f;
    [Tooltip("유닛 머리(콜라이더 최상단)와 체력바 사이의 여백")]
    [SerializeField] private float headMargin = 0.1f;

    private Canvas myCanvas;
    private SpriteRenderer unitSpriteRenderer;
    private Collider2D unitCollider;
    private UnitControl ownerUnit;
    private MenuManager menuManager;

    public void Init(UnitControl unit)
    {
        ownerUnit = unit;
        menuManager = FindObjectOfType<MenuManager>();

        myCanvas = GetComponent<Canvas>();
        if (myCanvas != null)
        {
            myCanvas.renderMode = RenderMode.WorldSpace;
        }

        if (ownerUnit != null)
        {
            unitSpriteRenderer = ownerUnit.GetComponent<SpriteRenderer>();
            unitCollider = ownerUnit.GetComponent<Collider2D>();
        }

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        if (redHealthFillImage != null)
        {
            redHealthFillImage.color = Color.red;
        }

        UpdateVisibility();
        UpdateHealthBar();
    }

    private void LateUpdate()
    {
        if (ownerUnit == null) return;

        UpdateVisibility();

        UpdateSortingOrder();

        FixTransformAndPosition();

        UpdateHealthBar();
    }

    private void UpdateSortingOrder()
    {
        if (myCanvas != null && unitSpriteRenderer != null)
        {
            myCanvas.sortingLayerID = unitSpriteRenderer.sortingLayerID;
            myCanvas.sortingOrder = unitSpriteRenderer.sortingOrder + 1;
        }
    }

    private void UpdateVisibility()
    {
        if (canvasGroup == null) return;

        if (menuManager != null && ownerUnit != null)
        {
            int unitLayer = ownerUnit.gameObject.layer;
            int leftLayer = LayerMask.NameToLayer("LeftUnit");  
            int rightLayer = LayerMask.NameToLayer("RightUnit"); 

            if (unitLayer == leftLayer)
            {
                canvasGroup.alpha = menuManager.showLeftHealthBars ? 1f : 0f;
            }
            else if (unitLayer == rightLayer)
            {
                canvasGroup.alpha = menuManager.showRightHealthBars ? 1f : 0f;
            }
            else
            {
                canvasGroup.alpha = 1f;
            }
        }
        else
        {
            canvasGroup.alpha = 1f;
        }
    }

    private void FixTransformAndPosition()
    {
        Vector3 targetPos = ownerUnit.transform.position;

        if (unitCollider != null)
        {
            targetPos.y = unitCollider.bounds.max.y + headMargin;
        }
        else
        {
            targetPos.y += yOffset;
        }

        transform.position = targetPos;
        transform.rotation = Quaternion.identity;

        Vector3 localScale = transform.localScale;
        if (transform.parent != null && transform.parent.lossyScale.x < 0)
        {
            localScale.x = -Mathf.Abs(localScale.x);
        }
        else
        {
            localScale.x = Mathf.Abs(localScale.x);
        }
        transform.localScale = localScale;
    }

    public void UpdateHealthBar()
    {
        if (ownerUnit == null || redHealthFillImage == null) return;

        float ratio = Mathf.Clamp01(ownerUnit.currentHealth / ownerUnit.maxHealth);
        redHealthFillImage.fillAmount = ratio;
    }
}