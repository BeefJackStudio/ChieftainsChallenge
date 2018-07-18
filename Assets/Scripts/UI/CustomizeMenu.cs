using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CustomizeMenu : MonoBehaviour {

    [Header("Text")]
    public TextMeshPro title;

    [Header("Parents")]
    public GameObject options4Parent;
    public GameObject options12Parent;
    public GameObject arrowsParent;

    [Header("Sections")]
    public CustomizationDatabase data;

    private int m_CurrentPage = 0;
    private int m_MaxPages { get { return (m_CurrentSection == 0 || m_CurrentSection == 1) ?  3 : 0; } }
    private int m_CurrentSection = 0;
    private CustomizeButton[] m_CurrentButtons;
    private CustomizationChoices m_CurrentSectionInstance;

    private CustomizeButton[] m_FourButtonChildren;
    private CustomizeButton[] m_TwelveButtonChildren;

    private CustomizeButton m_LastButton;

    private void Awake() {
        SetDefault();
    }

    private void Start() {
        m_FourButtonChildren = options4Parent.GetComponentsInChildren<CustomizeButton>(true);
        for (int i = 0; i < m_FourButtonChildren.Length; i++) {
            m_FourButtonChildren[i].Initialize(OnCustomizeButtonPress, i);
        }

        m_TwelveButtonChildren = options12Parent.GetComponentsInChildren<CustomizeButton>(true);
        for (int i = 0; i < m_TwelveButtonChildren.Length; i++) {
            m_TwelveButtonChildren[i].Initialize(OnCustomizeButtonPress, i);
        }

        SetCategory(0);
    }

    private void SetDefault() {

        //Just check if it's already loaded via SaveDataManager.cs
        if (CustomizationSelected.woodenMask == null) {

            CustomizationSelected.woodenMask = new CustomizationSelected.SelectionWrapper<CharacterMask>(data.sectionMask[0].options[0].GetComponent<CharacterMask>(), 0);
            CustomizationSelected.hawkMask = new CustomizationSelected.SelectionWrapper<CharacterMask>(data.sectionMask[1].options[0].GetComponent<CharacterMask>(), 0);
            CustomizationSelected.royalMask = new CustomizationSelected.SelectionWrapper<CharacterMask>(data.sectionMask[2].options[0].GetComponent<CharacterMask>(), 0);
            CustomizationSelected.skullMask = new CustomizationSelected.SelectionWrapper<CharacterMask>(data.sectionMask[3].options[0].GetComponent<CharacterMask>(), 0);

            CustomizationSelected.stoneBall = new CustomizationSelected.SelectionWrapper<GameBall>(data.sectionBall[0].options[0].GetComponent<GameBall>(), 0);
            CustomizationSelected.mudBall = new CustomizationSelected.SelectionWrapper<GameBall>(data.sectionBall[1].options[0].GetComponent<GameBall>(), 0);
            CustomizationSelected.beachBall = new CustomizationSelected.SelectionWrapper<GameBall>(data.sectionBall[2].options[0].GetComponent<GameBall>(), 0);
            CustomizationSelected.sunBall = new CustomizationSelected.SelectionWrapper<GameBall>(data.sectionBall[3].options[0].GetComponent<GameBall>(), 0);

            CustomizationSelected.particle = new CustomizationSelected.SelectionWrapper<GameObject>(data.sectionParticle.options[0], 0);
        }

        SaveDataManager.Instance.data.masksUnlocked[0] = true;
        SaveDataManager.Instance.data.masksUnlocked[4] = true;
        SaveDataManager.Instance.data.masksUnlocked[8] = true;
        SaveDataManager.Instance.data.masksUnlocked[12] = true;

        SaveDataManager.Instance.data.ballsUnlocked[0] = true;
        SaveDataManager.Instance.data.ballsUnlocked[4] = true;
        SaveDataManager.Instance.data.ballsUnlocked[8] = true;
        SaveDataManager.Instance.data.ballsUnlocked[12] = true;

        SaveDataManager.Instance.data.particlesUnlocked[0] = true;
        SaveDataManager.Instance.data.skinsUnlocked[0] = true;
    }

    private void OnCustomizeButtonPress(CustomizeButton button, int i) {
        if(m_LastButton != null) {
            m_LastButton.SetSelected(false);
        }

        m_LastButton = button;
        m_LastButton.SetSelected(true);

        switch (m_CurrentSection) {
            //Masks
            case 0:
                CharacterMask mask = m_CurrentSectionInstance.options[i].GetComponent<CharacterMask>();
                CustomizationSelected.SelectionWrapper<CharacterMask> maskWrapper = new CustomizationSelected.SelectionWrapper<CharacterMask>(mask, i);
                switch (m_CurrentPage) {
                    case 0:         //Wooden
                        CustomizationSelected.woodenMask = maskWrapper;
                        break;
                    case 1:         //Hawk
                        CustomizationSelected.hawkMask = maskWrapper;
                        break;
                    case 2:         //Royal
                        CustomizationSelected.royalMask = maskWrapper;
                        break;
                    case 3:         //Skull
                        CustomizationSelected.skullMask = maskWrapper;
                        break;
                }


                break;

            //Balls
            case 1:
                GameBall ball = m_CurrentSectionInstance.options[i].GetComponent<GameBall>();
                CustomizationSelected.SelectionWrapper<GameBall> ballWrapper = new CustomizationSelected.SelectionWrapper<GameBall>(ball, i);
                switch (m_CurrentPage) {
                    case 0:         //Wooden
                        CustomizationSelected.stoneBall = ballWrapper;
                        break;
                    case 1:         //Hawk
                        CustomizationSelected.mudBall = ballWrapper;
                        break;
                    case 2:         //Royal
                        CustomizationSelected.beachBall = ballWrapper;
                        break;
                    case 3:         //Skull
                        CustomizationSelected.sunBall = ballWrapper;
                        break;
                }
                break;

            //Skins
            case 2:
                break;

            //Particles
            case 3:
                CustomizationSelected.particle = new CustomizationSelected.SelectionWrapper<GameObject>(m_CurrentSectionInstance.options[i], i);
                break;
        }
    }

    public void NextPage() {
        int page = m_CurrentPage + 1;
        if (page > m_MaxPages) page = 0;
        SetPage(page);
    }

    public void PreviousPage() {
        int page = m_CurrentPage - 1;
        if (page < 0) page = m_MaxPages;
        SetPage(page);
    }

    private void SetPage(int page) {
        m_CurrentPage = Mathf.Clamp(page, 0, m_MaxPages);

        switch (m_CurrentSection) {
            //Masks
            case 0:
                SetSection(data.sectionMask[m_CurrentPage], CustomizationSelected.GetMaskType(m_CurrentPage).ID);
                break;

            //Balls
            case 1:
                SetSection(data.sectionBall[m_CurrentPage], CustomizationSelected.GetBallType(m_CurrentPage).ID);
                break;

            //Skins
            case 2:
                SetSection(data.sectionSkin, 0);
                break;

            //Particles
            case 3:
                SetSection(data.sectionParticle, CustomizationSelected.particle.ID);
                break;
        }
    }

    private void SetSection(CustomizationChoices section, int currentSelection) {
        bool isValid = section != null;
        m_CurrentSectionInstance = section;

        bool[] array;
        if (section.customizationType == CustomizationTypes.MASK_HAWK ||
            section.customizationType == CustomizationTypes.MASK_ROYAL ||
            section.customizationType == CustomizationTypes.MASK_SKULL ||
            section.customizationType == CustomizationTypes.MASK_WOODEN) array = SaveDataManager.Instance.data.masksUnlocked;
        else if (section.customizationType == CustomizationTypes.BALL_BEACH ||
            section.customizationType == CustomizationTypes.BALL_MUD ||
            section.customizationType == CustomizationTypes.BALL_STONE ||
            section.customizationType == CustomizationTypes.BALL_SUN) array = SaveDataManager.Instance.data.ballsUnlocked;
        else if (section.customizationType == CustomizationTypes.PARTICLES) array = SaveDataManager.Instance.data.particlesUnlocked;
        else array = SaveDataManager.Instance.data.skinsUnlocked;

        title.text = isValid ? section.headerText : "No customization found!";
        for (int i = 0; i < m_CurrentButtons.Length; i++) {
            CustomizeButton button = m_CurrentButtons[i];
            if (isValid) {
                if (i >= section.options.Count) {
                    if (i == section.options.Count) Debug.Log("Too little options for section: " + section.headerText);
                    button.SetLocked(true);
                    button.SetPreview(null);
                } else {
                    if(i == 0) {
                        m_LastButton = button;
                    }

                    bool isSelected = i == currentSelection;

                    if (isSelected) m_LastButton = button;

                    button.SetLocked(!(i == 0 || array[(m_CurrentPage * (array.Length == 16 ? 4 : 0)) + i]));
                    button.SetSelected(isSelected);
                    button.SetPreview(section.options[i]);
                }
            } else {
                button.SetLocked(true);
                button.SetPreview(null);
            }
        }
    }

    public void SetCategory(int category) {
        bool parentMode = category == 0 || category == 1;
        options4Parent.SetActive(parentMode);
        options12Parent.SetActive(!parentMode);
        arrowsParent.SetActive(parentMode);

        m_CurrentButtons = parentMode ? m_FourButtonChildren : m_TwelveButtonChildren;
        m_CurrentSection = category;

        SetPage(0);
    }
}