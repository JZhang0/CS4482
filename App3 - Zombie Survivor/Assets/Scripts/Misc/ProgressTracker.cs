using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ProgressTracker : MonoBehaviour
{
    public static ProgressTracker   Instance { get; private set; }
    public bool                     m_paused { get; private set; }

    public float gameDuration { get; private set; }

    [SerializeField] GameObject     m_UICanvas;
    [SerializeField] GameObject     m_PauseMenu;
    [SerializeField] GameObject     m_upgradePanel;
    [SerializeField] GameObject     m_upgradeSelectionPrefab;
    [SerializeField] Sprite         m_MW0Selection;
    [SerializeField] Sprite         m_MW1Selection;
    [SerializeField] Sprite         m_MW2Selection;
    [SerializeField] Sprite         m_RW0Selection;
    [SerializeField] Sprite         m_RW1Selection;
    [SerializeField] Sprite         m_RW2Selection;
    [SerializeField] Sprite         m_healthSelection;

    private UnityEngine.UI.Text     m_GameTimeDisplay;
    private UnityEngine.UI.Text     m_PlayerLevelDisplay;
    private UnityEngine.UI.Text     m_PlayerKillDisplay;
    private UnityEngine.UI.Slider   m_EXPSlider;
    private UnityEngine.UI.Slider   m_healthSlider;
    private Sprite[]                m_meleeWeaponSprites;
    private Sprite[]                m_rangeWeaponSprites;
    private int                     m_deathCount = 0;
    private int                     m_playerLevel = 1;
    private int                     m_playerMeleeWeapon = -1;
    private int                     m_playerRangeWeapon = -1;
    private int                     m_playerMeleeWeaponLevel;
    private int                     m_playerRangeWeaponLevel;
    private readonly int[]          m_levelUpRequirement = { 30, 60, 100, 150, 210, 280, 360, 450, 550, 660, 780, 910, 1050, 1200, 1360, 1530, 1710, 1900, 2100 };
    private float                   m_playerHealthPercentage;
    private float                   m_playerExp = 0;
    private float                   m_timeToGiveStartExp = 5.0f;
    private readonly string[]       m_MWNames = { "Shovel", "Pitchfork", "Sickle"};
    private readonly string[]       m_RWNames = { "Flintlock", "Machine gun", "ShotGun" };
    private readonly string[]       m_MWDescriptions = { "A shovel that rotates around the player", "A Pitchfork that stabs the enemy", "A Sickle that follows the player" };
    private readonly string[]       m_RWDescriptions = { "A Flintlock that always finds the enemy", "A Machine gun that stabs the enemy", "A Shot gun that shoots where you point"};

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        gameDuration = 600.0f;

        m_meleeWeaponSprites = new Sprite[3] { m_MW0Selection, m_MW1Selection, m_MW2Selection };
        m_rangeWeaponSprites = new Sprite[3] { m_RW0Selection, m_RW1Selection, m_RW2Selection };

        m_GameTimeDisplay = m_UICanvas.transform.Find("Text Time").GetComponent< UnityEngine.UI.Text>();
        m_PlayerLevelDisplay = m_UICanvas.transform.Find("Text Level").GetComponent<UnityEngine.UI.Text>();
        m_PlayerKillDisplay = m_UICanvas.transform.Find("Text Kill").GetComponent<UnityEngine.UI.Text>();
        m_EXPSlider = m_UICanvas.transform.Find("Slider Exp").GetComponent<UnityEngine.UI.Slider>();
        m_healthSlider = m_UICanvas.transform.Find("Slider Hp").GetComponent<UnityEngine.UI.Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_paused)
        {
            gameDuration -= Time.deltaTime;
            TimeSpan timeSoFar = TimeSpan.FromSeconds(gameDuration);
            m_GameTimeDisplay.text = timeSoFar.ToString("mm':'ss");
            if (gameDuration <= 0.0f)
            {
                GameManager.Instance.ChangeMenuScene();
            }

            else if (gameDuration >= 589.5f && gameDuration <= 590.5f && m_playerLevel == 1)
            {
                GainXp(30);
            }
        }

        if (m_playerExp >= m_levelUpRequirement[m_playerLevel - 1])
        {
            addUpgradeSelection();
        }

        m_PlayerLevelDisplay.text = "Lv." + m_playerLevel;
        m_PlayerKillDisplay.text = m_deathCount.ToString("D3");
        m_PlayerKillDisplay.text = m_deathCount.ToString("D3");
        m_healthSlider.value = m_playerHealthPercentage;
        m_EXPSlider.value = m_playerExp / m_levelUpRequirement[m_playerLevel-1];

        m_paused = m_upgradePanel.activeSelf || m_PauseMenu.activeSelf;
    }

    public void pauseGame()
    {
        m_PauseMenu.SetActive(!m_PauseMenu.activeSelf);
    }

    public void restartGame()
    {
        GameManager.Instance.StartLevel();
    }

    public void exitMenu()
    {
        GameManager.Instance.ChangeMenuScene(m_deathCount);
    }

    public void enemyDie()
    {
        m_deathCount++;
    }

    public void GainXp(int exp)
    {
        if(m_playerLevel <= 20)
        {
            m_playerExp += exp;
        }
    }

    private void addUpgradeSelection()
    {
        if (!m_upgradePanel.activeSelf)
        {
            if (m_playerMeleeWeapon == -1)
            {
                GameObject selection1 = Instantiate(m_upgradeSelectionPrefab, m_upgradePanel.transform);
                selection1.transform.GetComponent<UpgradeSelectionButton>().initialize(0);
                selection1.transform.Find("SelectionImage").GetComponent<Image>().sprite = m_meleeWeaponSprites[0];
                selection1.transform.Find("SelectionLevel").GetComponent<Text>().text = "Lv." + (m_playerMeleeWeaponLevel + 1);
                selection1.transform.Find("SelectionName").GetComponent<Text>().text = m_MWNames[0];
                selection1.transform.Find("SelectionDescription").GetComponent<Text>().text = m_MWDescriptions[0];

                GameObject selection2 = Instantiate(m_upgradeSelectionPrefab, m_upgradePanel.transform);
                selection2.transform.GetComponent<UpgradeSelectionButton>().initialize(1);
                selection2.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, -5.0f, 0.0f);
                selection2.transform.Find("SelectionImage").GetComponent<Image>().sprite = m_meleeWeaponSprites[1];
                selection2.transform.Find("SelectionLevel").GetComponent<Text>().text = "Lv." + (m_playerMeleeWeaponLevel + 1);
                selection2.transform.Find("SelectionName").GetComponent<Text>().text = m_MWNames[1];
                selection2.transform.Find("SelectionDescription").GetComponent<Text>().text = m_MWDescriptions[1];

                GameObject selection3 = Instantiate(m_upgradeSelectionPrefab, m_upgradePanel.transform);
                selection3.transform.GetComponent<UpgradeSelectionButton>().initialize(2);
                selection3.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, -30.0f, 0.0f);
                selection3.transform.Find("SelectionImage").GetComponent<Image>().sprite = m_meleeWeaponSprites[2];
                selection3.transform.Find("SelectionLevel").GetComponent<Text>().text = "Lv." + (m_playerMeleeWeaponLevel + 1);
                selection3.transform.Find("SelectionName").GetComponent<Text>().text = m_MWNames[2];
                selection3.transform.Find("SelectionDescription").GetComponent<Text>().text = m_MWDescriptions[2];
            }

            else if (m_playerRangeWeapon == -1)
            {
                GameObject selection1 = Instantiate(m_upgradeSelectionPrefab, m_upgradePanel.transform);
                selection1.transform.GetComponent<UpgradeSelectionButton>().initialize(0, false);
                selection1.transform.Find("SelectionImage").GetComponent<Image>().sprite = m_rangeWeaponSprites[0];
                selection1.transform.Find("SelectionLevel").GetComponent<Text>().text = "Lv." + (m_playerRangeWeaponLevel + 1);
                selection1.transform.Find("SelectionName").GetComponent<Text>().text = m_RWNames[0];
                selection1.transform.Find("SelectionDescription").GetComponent<Text>().text = m_RWDescriptions[0];

                GameObject selection2 = Instantiate(m_upgradeSelectionPrefab, m_upgradePanel.transform);
                selection2.transform.GetComponent<UpgradeSelectionButton>().initialize(1, false);
                selection2.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, -5.0f, 0.0f);
                selection2.transform.Find("SelectionImage").GetComponent<Image>().sprite = m_rangeWeaponSprites[1];
                selection2.transform.Find("SelectionLevel").GetComponent<Text>().text = "Lv." + (m_playerRangeWeaponLevel + 1);
                selection2.transform.Find("SelectionName").GetComponent<Text>().text = m_RWNames[1];
                selection2.transform.Find("SelectionDescription").GetComponent<Text>().text = m_RWDescriptions[1];

                GameObject selection3 = Instantiate(m_upgradeSelectionPrefab, m_upgradePanel.transform);
                selection3.transform.GetComponent<UpgradeSelectionButton>().initialize(2, false);
                selection3.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, -30.0f, 0.0f);
                selection3.transform.Find("SelectionImage").GetComponent<Image>().sprite = m_rangeWeaponSprites[2];
                selection3.transform.Find("SelectionLevel").GetComponent<Text>().text = "Lv." + (m_playerRangeWeaponLevel + 1);
                selection3.transform.Find("SelectionName").GetComponent<Text>().text = m_RWNames[2];
                selection3.transform.Find("SelectionDescription").GetComponent<Text>().text = m_RWDescriptions[2];
            }

            else if (m_playerLevel <= 16)
            {
                if (m_playerMeleeWeaponLevel < 8)
                {
                    GameObject selection1 = Instantiate(m_upgradeSelectionPrefab, m_upgradePanel.transform);
                    selection1.transform.GetComponent<UpgradeSelectionButton>().initialize(m_playerMeleeWeapon);
                    selection1.transform.Find("SelectionImage").GetComponent<Image>().sprite = m_meleeWeaponSprites[m_playerMeleeWeapon];
                    selection1.transform.Find("SelectionLevel").GetComponent<Text>().text = "Lv." + (m_playerMeleeWeaponLevel + 1);
                    selection1.transform.Find("SelectionName").GetComponent<Text>().text = m_MWNames[m_playerMeleeWeapon];
                    selection1.transform.Find("SelectionDescription").GetComponent<Text>().text = m_MWDescriptions[m_playerMeleeWeapon];
                }

                if (m_playerRangeWeaponLevel < 8)
                {
                    GameObject selection2 = Instantiate(m_upgradeSelectionPrefab, m_upgradePanel.transform);
                    selection2.transform.GetComponent<UpgradeSelectionButton>().initialize(m_playerRangeWeapon, false);
                    selection2.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, -5.0f, 0.0f);
                    selection2.transform.Find("SelectionImage").GetComponent<Image>().sprite = m_rangeWeaponSprites[m_playerRangeWeapon];
                    selection2.transform.Find("SelectionLevel").GetComponent<Text>().text = "Lv." + (m_playerRangeWeaponLevel + 1);
                    selection2.transform.Find("SelectionName").GetComponent<Text>().text = m_RWNames[m_playerRangeWeapon];
                    selection2.transform.Find("SelectionDescription").GetComponent<Text>().text = m_RWDescriptions[m_playerRangeWeapon];
                }

                GameObject selection3 = Instantiate(m_upgradeSelectionPrefab, m_upgradePanel.transform);
                selection3.transform.GetComponent<UpgradeSelectionButton>().initialize(0, isHealth: true);
                selection3.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, -30.0f, 0.0f);
                selection3.transform.Find("SelectionImage").GetComponent<Image>().sprite = m_healthSelection;
                selection3.transform.Find("SelectionLevel").GetComponent<Text>().text = "Lv.0";
                selection3.transform.Find("SelectionName").GetComponent<Text>().text = "Health Pack";
                selection3.transform.Find("SelectionDescription").GetComponent<Text>().text = "Heal half of your health";
            }

            if(m_playerLevel <= 16)
                m_upgradePanel.SetActive(true);
        }
    }

    public void makeSelection(int weaponNo, bool m_isMeleeWeapon, bool isHealth)
    {
        if (isHealth)
        {
            Player.Instance.addHealth(50.0f);
        }
        else
        {
            if (m_isMeleeWeapon)
            {
                if(m_playerMeleeWeapon != weaponNo)
                {
                    MeleeWeaponController.Instance.changeWeapon(weaponNo);
                    m_playerMeleeWeapon = weaponNo;
                    m_playerMeleeWeaponLevel = 1;
                }
                else
                {
                    MeleeWeaponController.Instance.upgradeWeapon();
                    m_playerMeleeWeaponLevel++;
                }
            }
            else
            {
                if (m_playerRangeWeapon != weaponNo)
                {
                    RangeWeaponController.Instance.changeWeapon(weaponNo);
                    m_playerRangeWeapon = weaponNo;
                    m_playerRangeWeaponLevel = 1;
                }
                else
                {
                    RangeWeaponController.Instance.upgradeWeapon();
                    m_playerRangeWeaponLevel++;
                }
            }
        }

        foreach (Transform selection in m_upgradePanel.transform)
        {
            Destroy(selection.gameObject);
        }

        m_playerExp -= m_levelUpRequirement[m_playerLevel - 1];
        m_playerLevel++;
        m_upgradePanel.SetActive(false);
    }

    public void changeHealth(float healthPercentage)
    {
        m_playerHealthPercentage = healthPercentage;
    }
}
