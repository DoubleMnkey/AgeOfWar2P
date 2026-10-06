using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static MenuManager;

public class PlayerUIManager : MonoBehaviour
{
    [Header("Left Main Panels")]
    public GameObject leftPanel;
    public GameObject leftUnitPanel;
    public GameObject leftTurretPanel;
    public GameObject leftSelectTurretPanel;
    public GameObject leftSellTurretPanel;

    [Header("Right Main Panels")]
    public GameObject rightPanel;
    public GameObject rightUnitPanel;
    public GameObject rightTurretPanel;
    public GameObject rightSelectTurretPanel;
    public GameObject rightSellTurretPanel;

    [Header("UI Text")]
    public TextMeshProUGUI leftGoldText;
    public TextMeshProUGUI leftExpText;
    public TextMeshProUGUI rightGoldText;
    public TextMeshProUGUI rightExpText;

    public TextMeshProUGUI leftDescriptionText;
    public TextMeshProUGUI rightDescriptionText;

    [Header("Special Images & Buttons")]
    public Image leftSpecialImage;
    public Image rightSpecialImage;
    public Image leftSpecialCooldownImage;  
    public Image rightSpecialCooldownImage; 
    public RectTransform leftSpecialButton;
    public RectTransform rightSpecialButton;

    [Header("Health Images")]
    public Image leftHealthImage;
    public Image rightHealthImage;

    [Header("Left Highlight & Buttons")]
    public RectTransform leftHighlight;
    public RectTransform[] leftMainButtons;
    public RectTransform[] leftUnitButtons;
    public RectTransform[] leftTurretButtons;
    public RectTransform leftSelectCancelButton;
    public RectTransform leftSellCancelButton;

    [Header("Right Highlight & Buttons")]
    public RectTransform rightHighlight;
    public RectTransform[] rightMainButtons;
    public RectTransform[] rightUnitButtons;
    public RectTransform[] rightTurretButtons;
    public RectTransform rightSelectCancelButton;
    public RectTransform rightSellCancelButton;

    [Header("Select/Sell Slot UI")]
    public RectTransform[] leftSelectTurretButtons;
    public RectTransform[] leftSellTurretButtons;
    public RectTransform[] rightSelectTurretButtons;
    public RectTransform[] rightSellTurretButtons;
    public void UpdateSubPanelUI(bool isLeft, PlayerData player)
    {
        if (isLeft)
        {
            if (leftGoldText != null) leftGoldText.text = player.gold.ToString();
            if (leftExpText != null) leftExpText.text = player.exp.ToString();
        }
        else
        {
            if (rightGoldText != null) rightGoldText.text = player.gold.ToString();
            if (rightExpText != null) rightExpText.text = player.exp.ToString();
        }
    }

    public void UpdatePanels(bool isLeft, MenuManager.MenuState menuState)
    {
        GameObject mainP = isLeft ? leftPanel : rightPanel;
        GameObject unitP = isLeft ? leftUnitPanel : rightUnitPanel;
        GameObject turretP = isLeft ? leftTurretPanel : rightTurretPanel;
        GameObject selectTurretP = isLeft ? leftSelectTurretPanel : rightSelectTurretPanel;
        GameObject sellTurretP = isLeft ? leftSellTurretPanel : rightSellTurretPanel;

        if (mainP != null) mainP.SetActive(menuState == MenuManager.MenuState.Main || menuState == MenuManager.MenuState.HealthToggle || menuState == MenuManager.MenuState.Special);
        if (unitP != null) unitP.SetActive(menuState == MenuManager.MenuState.UnitMenu);
        if (turretP != null) turretP.SetActive(menuState == MenuManager.MenuState.TurretMenu);
        if (selectTurretP != null) selectTurretP.SetActive(menuState == MenuManager.MenuState.SelectTurretSlot);
        if (sellTurretP != null) sellTurretP.SetActive(menuState == MenuManager.MenuState.SellTurret);
    }

    public void UpdateSlotUIState(RectTransform[] buttons, int unlockedSlots)
    {
        if (buttons == null) return;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
            {
                buttons[i].gameObject.SetActive(i <= unlockedSlots);
            }
        }
    }

    public void UpdateHighlight(bool isLeft, PlayerData player)
    {
        RectTransform highlight = isLeft ? leftHighlight : rightHighlight;
        RectTransform[] mainBtns = isLeft ? leftMainButtons : rightMainButtons;
        RectTransform[] unitBtns = isLeft ? leftUnitButtons : rightUnitButtons;
        RectTransform[] turretBtns = isLeft ? leftTurretButtons : rightTurretButtons;
        RectTransform[] selectSlotBtns = isLeft ? leftSelectTurretButtons : rightSelectTurretButtons;
        RectTransform[] sellSlotBtns = isLeft ? leftSellTurretButtons : rightSellTurretButtons;
        RectTransform selectCancelBtn = isLeft ? leftSelectCancelButton : rightSelectCancelButton;
        RectTransform sellCancelBtn = isLeft ? leftSellCancelButton : rightSellCancelButton;

        if (highlight == null) return;

        System.Action<RectTransform> SetHighlightToButton = (targetBtn) =>
        {
            if (targetBtn == null || !targetBtn.gameObject.activeSelf)
            {
                highlight.gameObject.SetActive(false);
                return;
            }

            highlight.gameObject.SetActive(true);

            highlight.SetParent(targetBtn, false);

            highlight.anchoredPosition = Vector2.zero;
            highlight.sizeDelta = targetBtn.rect.size;
            highlight.localScale = Vector3.one;

            highlight.SetAsLastSibling();
        };

        switch (player.currentMenu)
        {
            case MenuManager.MenuState.Main:

                if (mainBtns != null &&
                    player.selectedIndex < mainBtns.Length)
                {
                    SetHighlightToButton(mainBtns[player.selectedIndex]);
                }

                break;

            case MenuManager.MenuState.HealthToggle:
                highlight.gameObject.SetActive(false);
                break;

            case MenuManager.MenuState.Special:
                RectTransform specialBtn = isLeft ? leftSpecialButton : rightSpecialButton;
                if (specialBtn != null) SetHighlightToButton(specialBtn);
                break;

            case MenuManager.MenuState.UnitMenu:

                if (unitBtns != null &&
                    player.selectedIndex < unitBtns.Length)
                {
                    SetHighlightToButton(unitBtns[player.selectedIndex]);
                }

                break;

            case MenuManager.MenuState.TurretMenu:

                if (turretBtns != null &&
                    player.selectedIndex < turretBtns.Length)
                {
                    SetHighlightToButton(turretBtns[player.selectedIndex]);
                }

                break;

            case MenuManager.MenuState.SelectTurretSlot:
                int selectCancelIndex = player.unlockedSlots + 1;
                if (player.selectedIndex == selectCancelIndex) SetHighlightToButton(selectCancelBtn);
                else if (player.selectedIndex <= player.unlockedSlots && selectSlotBtns != null && player.selectedIndex < selectSlotBtns.Length)
                    SetHighlightToButton(selectSlotBtns[player.selectedIndex]);
                break;

            case MenuManager.MenuState.SellTurret:
                int sellCancelIndex = player.unlockedSlots + 1;
                if (player.selectedIndex == sellCancelIndex) SetHighlightToButton(sellCancelBtn);
                else if (player.selectedIndex <= player.unlockedSlots && sellSlotBtns != null && player.selectedIndex < sellSlotBtns.Length)
                    SetHighlightToButton(sellSlotBtns[player.selectedIndex]);
                break;
        }
    }

    public void UpdateSpecialImage(bool isLeft, int eraIndex, GenerationImageData[] leftGen, GenerationImageData[] rightGen)
    {
        GenerationImageData[] generations = isLeft ? leftGen : rightGen;
        Image specialImage = isLeft ? leftSpecialImage : rightSpecialImage;

        if (specialImage == null || generations == null || eraIndex < 0 || eraIndex >= generations.Length) return;

        if (generations[eraIndex].specialSprite != null)
            specialImage.sprite = generations[eraIndex].specialSprite;
    }

    public void UpdateHealthBlink(bool isLeft, PlayerData player)
    {
        Image healthImage = isLeft ? leftHealthImage : rightHealthImage;
        if (healthImage == null) return;

        Color c = healthImage.color;
        if (player.currentMenu == MenuManager.MenuState.HealthToggle)
        {
            c.a = Mathf.Lerp(0.3f, 1f, (Mathf.Sin(Time.time * 6f) + 1f) * 0.5f);
        }
        else
        {
            c.a = 1f;
        }
        healthImage.color = c;
    }

    public void SetDescription(
        bool isLeft,
        string text)
    {
        TextMeshProUGUI target =
            isLeft
            ? leftDescriptionText
            : rightDescriptionText;

        if (target != null)
        {
            target.text = text;
        }
    }
    public void UpdateSpecialCooldownUI(bool isLeft, float currentCooldown, float maxCooldown)
    {
        Image cooldownImg = isLeft ? leftSpecialCooldownImage : rightSpecialCooldownImage;
        if (cooldownImg == null) return;

        if (currentCooldown > 0f)
        {
            cooldownImg.gameObject.SetActive(true);
            cooldownImg.fillAmount = currentCooldown / maxCooldown; 
        }
        else
        {
            cooldownImg.fillAmount = 0f;
            cooldownImg.gameObject.SetActive(false);
        }
    }
}