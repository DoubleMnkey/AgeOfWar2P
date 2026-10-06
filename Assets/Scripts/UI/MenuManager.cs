using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public enum MenuState
    {
        Main, HealthToggle, Special, UnitMenu, TurretMenu, SelectTurretSlot, SellTurret
    }

    [System.Serializable]
    public class AgeVisualData
    {
        public Sprite baseSprite;
        public Sprite towerBottomSprite;
        public Sprite towerPartSprite;
        public Sprite towerTopSprite;
        public Vector3 leftBasePosition, rightBasePosition;
        public Vector3 leftBottomPosition, rightBottomPosition;
        public Vector3 leftPartPosition, rightPartPosition;
        public Vector3 leftTopPosition, rightTopPosition;
    }

    [System.Serializable]
    public class GenerationImageData
    {
        public string eraName;
        public Sprite[] unitSprites;
        public Sprite[] turretSprites;
        public Sprite specialSprite;
    }

    [System.Serializable]
    public class AgeTurretPrefabData
    {
        public GameObject[] turretPrefabs = new GameObject[3];
    }

    [Header("Settings UI Reference")]
    [SerializeField] private GameObject settingsPanel; 
    private bool isSettingsOpen = false; 

    [Header("Sub Managers")]
    public PlayerUIManager uiManager;
    public TurretManager turretManager;
    public SpecialSkillManager specialSkillManager;
    public EvolutionManager evolutionManager;
    public SpawnManager spawnManager;

    [Header("Base Health References (체력 연동용)")]
    public BaseHealth leftBaseHealth;
    public BaseHealth rightBaseHealth;

    [Header("Players Data")]
    public PlayerData leftPlayer = new PlayerData();
    public PlayerData rightPlayer = new PlayerData();

    [Header("Generation Sprites")]
    public GenerationImageData[] leftGenerations;
    public GenerationImageData[] rightGenerations;

    [Header("Left Renderers")]
    public SpriteRenderer leftBaseRenderer, leftBottomRenderer, leftPartRenderer, leftTopRenderer;

    [Header("Right Renderers")]
    public SpriteRenderer rightBaseRenderer, rightBottomRenderer, rightPartRenderer, rightTopRenderer;

    [Header("Age Visuals")]
    public AgeVisualData[] ageVisuals = new AgeVisualData[5];

    private int leftSelectedTurretTypeToBuy = -1;
    private int rightSelectedTurretTypeToBuy = -1;

    public bool showLeftHealthBars = true;
    public bool showRightHealthBars = true;

    void Start()
    {
        if (evolutionManager == null) evolutionManager = GetComponent<EvolutionManager>();
        if (uiManager == null) uiManager = GetComponent<PlayerUIManager>();
        if (turretManager == null) turretManager = GetComponent<TurretManager>();
        if (specialSkillManager == null) specialSkillManager = GetComponent<SpecialSkillManager>();

        leftPlayer.unlockedSlots = 0;
        rightPlayer.unlockedSlots = 0;

        turretManager.ClearTurretPositionsOnStart();

        RefreshAll(true);
        RefreshAll(false);

        EvolvePlayer(true, 0);
        EvolvePlayer(false, 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscapeKey();
            return;
        }

        if (isSettingsOpen) return;

        HandlePlayer1();
        HandlePlayer2();

        uiManager.UpdateHealthBlink(true, leftPlayer);
        uiManager.UpdateHealthBlink(false, rightPlayer);

        if (leftPlayer.currentMenu == MenuState.Special) RefreshDescription(true);
        if (rightPlayer.currentMenu == MenuState.Special) RefreshDescription(false);
    }

    private void HandleEscapeKey()
    {
        if (isSettingsOpen && settingsPanel != null)
        {
            InGameMenuManager inGameMenu = settingsPanel.GetComponentInChildren<InGameMenuManager>();

            if (inGameMenu != null && inGameMenu.IsAnySubPanelActive())
            {
                inGameMenu.CloseAllSubPanels();
                return;
            }
        }

        ToggleSettings();
    }

    public void ToggleSettings()
    {
        isSettingsOpen = !isSettingsOpen;

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(isSettingsOpen);

            if (isSettingsOpen)
            {
                InGameMenuManager inGameMenu = settingsPanel.GetComponentInChildren<InGameMenuManager>();
                if (inGameMenu != null)
                {
                    inGameMenu.OnOpenMenu();
                }
            }
        }
        Time.timeScale = isSettingsOpen ? 0f : 1f;
    }
    public void CloseSettings()
    {
        if (isSettingsOpen)
        {
            ToggleSettings();
        }
    }

    private void RefreshAll(bool isLeft)
    {
        PlayerData player = isLeft ? leftPlayer : rightPlayer;
        uiManager.UpdateSubPanelUI(isLeft, player);
        uiManager.UpdateSlotUIState(isLeft ? uiManager.leftSelectTurretButtons : uiManager.rightSelectTurretButtons, player.unlockedSlots);
        uiManager.UpdateSlotUIState(isLeft ? uiManager.leftSellTurretButtons : uiManager.rightSellTurretButtons, player.unlockedSlots);
        uiManager.UpdatePanels(isLeft, player.currentMenu);
        uiManager.UpdateHighlight(isLeft, player);
        ApplyAgeVisual(isLeft);
        uiManager.UpdateSpecialImage(isLeft, player.age - 1, leftGenerations, rightGenerations);
        RefreshDescription(isLeft);
    }

    void HandlePlayer1()
    {
        if (isSettingsOpen) return; 

        if (Input.GetKeyDown(KeyCode.A)) MoveLeft(true);
        if (Input.GetKeyDown(KeyCode.D)) MoveRight(true);
        if (Input.GetKeyDown(KeyCode.W)) MoveUp(true);
        if (Input.GetKeyDown(KeyCode.S)) MoveDown(true);
        if (Input.GetKeyDown(KeyCode.E)) Confirm(true);
        if (Input.GetKeyDown(KeyCode.Q)) Cancel(true);
    }

    void HandlePlayer2()
    {
        if (isSettingsOpen) return; 

        if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveLeft(false);
        if (Input.GetKeyDown(KeyCode.RightArrow)) MoveRight(false);
        if (Input.GetKeyDown(KeyCode.UpArrow)) MoveUp(false);
        if (Input.GetKeyDown(KeyCode.DownArrow)) MoveDown(false);
        if (Input.GetKeyDown(KeyCode.Slash)) Confirm(false);
        if (Input.GetKeyDown(KeyCode.Period)) Cancel(false);
    }

    void MoveLeft(bool isLeft)
    {
        PlayerData player = isLeft ? leftPlayer : rightPlayer;
        if (player.currentMenu == MenuState.Main) player.selectedIndex = (player.selectedIndex - 1 + 5) % 5;
        else if (player.currentMenu == MenuState.UnitMenu || player.currentMenu == MenuState.TurretMenu) player.selectedIndex = (player.selectedIndex - 1 + 4) % 4;

        uiManager.UpdateHighlight(isLeft, player);
        RefreshDescription(isLeft);
    }

    void MoveRight(bool isLeft)
    {
        PlayerData player = isLeft ? leftPlayer : rightPlayer;
        if (player.currentMenu == MenuState.Main) player.selectedIndex = (player.selectedIndex + 1) % 5;
        else if (player.currentMenu == MenuState.UnitMenu || player.currentMenu == MenuState.TurretMenu) player.selectedIndex = (player.selectedIndex + 1) % 4;

        uiManager.UpdateHighlight(isLeft, player);
        RefreshDescription(isLeft);
    }

    void MoveUp(bool isLeft)
    {
        PlayerData player = isLeft ? leftPlayer : rightPlayer;

        switch (player.currentMenu)
        {
            case MenuState.Main:
                player.currentMenu = MenuState.HealthToggle;
                player.selectedIndex = 0;
                break;
            case MenuState.HealthToggle:
                player.currentMenu = MenuState.Special;
                player.selectedIndex = 0;
                break;
            case MenuState.Special:
                player.currentMenu = MenuState.Main;
                player.selectedIndex = 0;
                break;
            case MenuState.SelectTurretSlot:
            case MenuState.SellTurret:
                int maxIdx = player.unlockedSlots + 1;
                player.selectedIndex = (player.selectedIndex + 1) % (maxIdx + 1);
                break;
        }

        uiManager.UpdatePanels(isLeft, player.currentMenu);
        uiManager.UpdateHighlight(isLeft, player);
        RefreshDescription(isLeft);
    }

    void MoveDown(bool isLeft)
    {
        PlayerData player = isLeft ? leftPlayer : rightPlayer;

        switch (player.currentMenu)
        {
            case MenuState.Main:
                player.currentMenu = MenuState.Special;
                player.selectedIndex = 0;
                break;
            case MenuState.HealthToggle:
                player.currentMenu = MenuState.Main;
                player.selectedIndex = 0;
                break;
            case MenuState.Special:
                player.currentMenu = MenuState.HealthToggle;
                player.selectedIndex = 0;
                break;
            case MenuState.SelectTurretSlot:
            case MenuState.SellTurret:
                int maxIdx = player.unlockedSlots + 1;
                player.selectedIndex = (player.selectedIndex - 1 + (maxIdx + 1)) % (maxIdx + 1);
                break;
        }

        uiManager.UpdatePanels(isLeft, player.currentMenu);
        uiManager.UpdateHighlight(isLeft, player);
        RefreshDescription(isLeft);
    }

    void Confirm(bool isLeft)
    {
        PlayerData player = isLeft ? leftPlayer : rightPlayer;
        switch (player.currentMenu)
        {
            case MenuState.HealthToggle:
                if (isLeft)
                {
                    showLeftHealthBars = !showLeftHealthBars;
                }
                else
                {
                    showRightHealthBars = !showRightHealthBars;
                }
                break;

            case MenuState.Special:
                if (specialSkillManager.CanUseSpecial(isLeft))
                {
                    specialSkillManager.UseSpecial(isLeft, player.age);
                    player.currentMenu = MenuState.Main;
                    player.selectedIndex = 0;
                    uiManager.UpdatePanels(isLeft, player.currentMenu);
                    uiManager.UpdateHighlight(isLeft, player);
                }
                break;

            case MenuState.Main:
                MainMenuSelection(isLeft);
                break;
            case MenuState.UnitMenu:
                BuyUnit(isLeft, player.selectedIndex);
                break;
            case MenuState.TurretMenu:
                SelectTurretToBuy(isLeft, player.selectedIndex);
                break;
            case MenuState.SelectTurretSlot:
                if (player.selectedIndex == player.unlockedSlots + 1) Cancel(isLeft);
                else
                {
                    int selectedType = isLeft ? leftSelectedTurretTypeToBuy : rightSelectedTurretTypeToBuy;
                    if (turretManager.BuildTurretAtSlot(isLeft, player, player.selectedIndex, selectedType, isLeft ? leftGenerations : rightGenerations))
                    {
                        uiManager.UpdateSubPanelUI(isLeft, player);
                        Cancel(isLeft);
                    }
                }
                break;
            case MenuState.SellTurret:
                if (player.selectedIndex == player.unlockedSlots + 1) Cancel(isLeft);
                else
                {
                    turretManager.SellTurretAtSlot(isLeft, player, player.selectedIndex);
                    uiManager.UpdateSubPanelUI(isLeft, player);
                    Cancel(isLeft);
                }
                break;
        }
    }

    void Cancel(bool isLeft)
    {
        PlayerData player = isLeft ? leftPlayer : rightPlayer;
        player.currentMenu = MenuState.Main;
        player.selectedIndex = 0;

        if (isLeft) leftSelectedTurretTypeToBuy = -1;
        else rightSelectedTurretTypeToBuy = -1;

        uiManager.UpdatePanels(isLeft, player.currentMenu);
        uiManager.UpdateHighlight(isLeft, player);
        RefreshDescription(isLeft);
    }

    void MainMenuSelection(bool isLeft)
    {
        PlayerData player = isLeft ? leftPlayer : rightPlayer;
        switch (player.selectedIndex)
        {
            case 0: player.currentMenu = MenuState.UnitMenu; player.selectedIndex = 0; break;
            case 1: player.currentMenu = MenuState.TurretMenu; player.selectedIndex = 0; break;
            case 2: player.currentMenu = MenuState.SellTurret; player.selectedIndex = 0; break;
            case 3: BuySlot(isLeft); break;
            case 4:
                if (evolutionManager != null)
                {
                    BaseHealth targetBaseHealth = isLeft ? leftBaseHealth : rightBaseHealth;

                    bool isEvolved = evolutionManager.Evolve(player, targetBaseHealth);

                    if (isEvolved)
                    {
                        uiManager.UpdateSubPanelUI(isLeft, player);
                        ApplyAgeVisual(isLeft);
                        EvolvePlayer(isLeft, player.age - 1);
                        uiManager.UpdateSpecialImage(isLeft, player.age - 1, leftGenerations, rightGenerations);

                        if (specialSkillManager != null)
                        {
                            specialSkillManager.ResetCooldown(isLeft);
                        }
                    }
                }
                break;
        }
        uiManager.UpdatePanels(isLeft, player.currentMenu);
        uiManager.UpdateHighlight(isLeft, player);
        RefreshDescription(isLeft);
    }

    void BuySlot(bool isLeft)
    {
        PlayerData player = isLeft ? leftPlayer : rightPlayer;
        if (player.unlockedSlots >= 3) return;

        int price = GameDatabase.SlotCost[player.unlockedSlots];
        if (player.gold >= price)
        {
            player.gold -= price;
            player.unlockedSlots++;
            uiManager.UpdateSubPanelUI(isLeft, player);
            ApplyAgeVisual(isLeft);
            uiManager.UpdateSlotUIState(isLeft ? uiManager.leftSelectTurretButtons : uiManager.rightSelectTurretButtons, player.unlockedSlots);
            uiManager.UpdateSlotUIState(isLeft ? uiManager.leftSellTurretButtons : uiManager.rightSellTurretButtons, player.unlockedSlots);
            uiManager.UpdatePanels(isLeft, player.currentMenu);
        }
    }

    void SelectTurretToBuy(bool isLeft, int turretID)
    {
        if (turretID == 3) { Cancel(isLeft); return; }
        PlayerData player = isLeft ? leftPlayer : rightPlayer;
        int ageIndex = player.age - 1;

        if (ageIndex >= 0 && ageIndex < GameDatabase.TurretCost.GetLength(0))
        {
            int cost = GameDatabase.TurretCost[ageIndex, turretID];
            if (player.gold >= cost)
            {
                if (isLeft) leftSelectedTurretTypeToBuy = turretID;
                else rightSelectedTurretTypeToBuy = turretID;

                player.currentMenu = MenuState.SelectTurretSlot;
                player.selectedIndex = 0;
                uiManager.UpdatePanels(isLeft, player.currentMenu);
                uiManager.UpdateHighlight(isLeft, player);
            }
        }
    }

    void BuyUnit(bool isLeft, int unitID)
    {
        if (unitID == 3) { Cancel(isLeft); return; }
        PlayerData player = isLeft ? leftPlayer : rightPlayer;
        int ageIndex = player.age - 1;

        if (ageIndex >= 0 && ageIndex < GameDatabase.UnitCost.GetLength(0))
        {
            int cost = GameDatabase.UnitCost[ageIndex, unitID];
            if (player.gold >= cost)
            {
                if (spawnManager.ProduceUnit(ageIndex, isLeft, unitID))
                {
                    player.gold -= cost;
                    uiManager.UpdateSubPanelUI(isLeft, player);
                }
            }
        }
    }

    void ApplyAgeVisual(bool isLeft)
    {
        PlayerData player = isLeft ? leftPlayer : rightPlayer;
        int ageIndex = Mathf.Clamp(player.age - 1, 0, ageVisuals.Length - 1);
        AgeVisualData data = ageVisuals[ageIndex];

        SpriteRenderer baseRenderer = isLeft ? leftBaseRenderer : rightBaseRenderer;
        SpriteRenderer bottomRenderer = isLeft ? leftBottomRenderer : rightBottomRenderer;
        SpriteRenderer partRenderer = isLeft ? leftPartRenderer : rightPartRenderer;
        SpriteRenderer topRenderer = isLeft ? leftTopRenderer : rightTopRenderer;

        if (baseRenderer == null || bottomRenderer == null || partRenderer == null || topRenderer == null) return;

        baseRenderer.sprite = data.baseSprite;
        bottomRenderer.sprite = data.towerBottomSprite;
        partRenderer.sprite = data.towerPartSprite;
        topRenderer.sprite = data.towerTopSprite;

        bottomRenderer.gameObject.SetActive(player.unlockedSlots >= 1);
        partRenderer.gameObject.SetActive(player.unlockedSlots >= 2);
        topRenderer.gameObject.SetActive(player.unlockedSlots >= 3);

        if (isLeft)
        {
            baseRenderer.transform.position = data.leftBasePosition;
            bottomRenderer.transform.localPosition = data.leftBottomPosition;
            partRenderer.transform.localPosition = data.leftPartPosition;
            topRenderer.transform.localPosition = data.leftTopPosition;
            baseRenderer.flipX = bottomRenderer.flipX = partRenderer.flipX = topRenderer.flipX = false;
        }
        else
        {
            baseRenderer.transform.position = data.rightBasePosition;
            bottomRenderer.transform.localPosition = data.rightBottomPosition;
            partRenderer.transform.localPosition = data.rightPartPosition;
            topRenderer.transform.localPosition = data.rightTopPosition;
            baseRenderer.flipX = bottomRenderer.flipX = partRenderer.flipX = topRenderer.flipX = true;
        }
    }

    public void EvolvePlayer(bool isLeft, int targetEraIndex)
    {
        GenerationImageData[] genData = isLeft ? leftGenerations : rightGenerations;
        RectTransform[] unitBtns = isLeft ? uiManager.leftUnitButtons : uiManager.rightUnitButtons;
        RectTransform[] turretBtns = isLeft ? uiManager.leftTurretButtons : uiManager.rightTurretButtons;

        if (genData == null || targetEraIndex < 0 || targetEraIndex >= genData.Length)
            return;

        GenerationImageData currentEra = genData[targetEraIndex];

        if (unitBtns != null && currentEra.unitSprites != null)
        {
            int count = Mathf.Min(unitBtns.Length, currentEra.unitSprites.Length);
            for (int i = 0; i < count; i++)
            {
                if (unitBtns[i] != null && currentEra.unitSprites[i] != null)
                {
                    Image btnImage = unitBtns[i].GetComponent<Image>();
                    if (btnImage != null) btnImage.sprite = currentEra.unitSprites[i];
                }
            }
        }

        if (turretBtns != null && currentEra.turretSprites != null)
        {
            int count = Mathf.Min(turretBtns.Length, currentEra.turretSprites.Length);
            for (int i = 0; i < count; i++)
            {
                if (turretBtns[i] != null && currentEra.turretSprites[i] != null)
                {
                    Image btnImage = turretBtns[i].GetComponent<Image>();
                    if (btnImage != null) btnImage.sprite = currentEra.turretSprites[i];
                }
            }
        }
    }

    public void AddReward(bool isLeft, int goldReward, int expReward)
    {
        PlayerData player = isLeft ? leftPlayer : rightPlayer;
        player.gold += goldReward;
        player.exp += expReward;
        uiManager.UpdateSubPanelUI(isLeft, player);
    }

    private string GetDescription(bool isLeft)
    {
        PlayerData player = isLeft ? leftPlayer : rightPlayer;
        int ageIndex = player.age - 1;

        switch (player.currentMenu)
        {
            case MenuState.UnitMenu:
                switch (ageIndex)
                {
                    case 0:
                        switch (player.selectedIndex)
                        {
                            case 0: return "15$ - Club man";
                            case 1: return "25$ - Slingshot man";
                            case 2: return "100$ - Dino rider";
                            case 3: return "Cancel";
                        }
                        break;
                    case 1:
                        switch (player.selectedIndex)
                        {
                            case 0: return "50$ - Swordman";
                            case 1: return "75$ - Archer";
                            case 2: return "500$ - Knight";
                            case 3: return "Cancel";
                        }
                        break;
                    case 2:
                        switch (player.selectedIndex)
                        {
                            case 0: return "200$ - Dueler";
                            case 1: return "400$ - Mousuqettere";
                            case 2: return "1000$ - Cannoner";
                            case 3: return "Cancel";
                        }
                        break;
                    case 3:
                        switch (player.selectedIndex)
                        {
                            case 0: return "1500$ - Melee Infantry";
                            case 1: return "2000$ - Infantry";
                            case 2: return "7000$ - Tank";
                            case 3: return "Cancel";
                        }
                        break;
                    case 4:
                        switch (player.selectedIndex)
                        {
                            case 0: return "5000$ - God's blade";
                            case 1: return "6000$ - Blaster";
                            case 2: return "20000$ - War machine";
                            case 3: return "Cancel";
                        }
                        break;
                }
                break;

            case MenuState.TurretMenu:
                switch (ageIndex)
                {
                    case 0:
                        switch (player.selectedIndex)
                        {
                            case 0: return "100$ - Rock Slingshot";
                            case 1: return "200$ - Egg automatic";
                            case 2: return "500$ - Primitive Catapult";
                            case 3: return "Cancel";
                        }
                        break;
                    case 1:
                        switch (player.selectedIndex)
                        {
                            case 0: return "500$ - Catapult";
                            case 1: return "750$ - Fire Catapult";
                            case 2: return "1000$ - Oil";
                            case 3: return "Cancel";
                        }
                        break;
                    case 2:
                        switch (player.selectedIndex)
                        {
                            case 0: return "1500$ - Small Cannon";
                            case 1: return "3000$ - Large Cannon";
                            case 2: return "6000$ - Explosives Cannon";
                            case 3: return "Cancel";
                        }
                        break;
                    case 3:
                        switch (player.selectedIndex)
                        {
                            case 0: return "7000$ - Single Turret";
                            case 1: return "9000$ - Rocket Turret";
                            case 2: return "14000$ - Double Turret";
                            case 3: return "Cancel";
                        }
                        break;
                    case 4:
                        switch (player.selectedIndex)
                        {
                            case 0: return "24000$ - Titanium Shooter";
                            case 1: return "40000$ - Lazer Cannon";
                            case 2: return "100000$ - Ion Ray";
                            case 3: return "Cancel";
                        }
                        break;
                }
                break;

            case MenuState.Main:
                switch (player.selectedIndex)
                {
                    case 0: return "Train unit menu";
                    case 1: return "Build turrets menu";
                    case 2: return "Sell a turret";
                    case 3:
                        if (player.unlockedSlots >= 3) return "Can't build anymore";
                        return $"{GameDatabase.SlotCost[player.unlockedSlots]}$ - Add a turret spot";
                    case 4:
                        if (player.age >= 5) return "You cannot evolve anymore";
                        return $"{GameDatabase.EvolutionExp[player.age]}xp - Evolve to next age";
                }
                break;

            case MenuState.HealthToggle:
                return "Show unit health";

            case MenuState.Special:
                float remainTime = specialSkillManager.GetRemainingCooldown(isLeft);
                string cdText = remainTime > 0f ? $" ({Mathf.CeilToInt(remainTime)}s)" : "";

                switch (ageIndex)
                {
                    case 0: return $"Special skill : Meteor{cdText}";
                    case 1: return $"Special skill : ArrowRain{cdText}";
                    case 2: return $"Special skill : Heal{cdText}";
                    case 3: return $"Special skill : AirStrikeJet{cdText}";
                    case 4: return $"Special skill : LaserBeam{cdText}";
                }
                break;
        }

        return "";
    }

    private void RefreshDescription(bool isLeft)
    {
        uiManager.SetDescription(isLeft, GetDescription(isLeft));
    }
}