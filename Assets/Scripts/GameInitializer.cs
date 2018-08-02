using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SaveDataManager))]
[RequireComponent(typeof(LevelManager))]
public class GameInitializer : MonoBehaviour {

    public string cutsceneName;
    public GenericLevelData levelData;

    private SaveDataManager m_SaveDataManager;
    private LoadingScreen m_LoadingScreen;
    private LevelManager m_LevelManager;

    private void Awake() {
        m_SaveDataManager = GetComponent<SaveDataManager>();
        m_LevelManager = GetComponent<LevelManager>();

        Physics2D.gravity *= levelData.gameSpeed;
    }

    private void Start() {
        m_LoadingScreen = LoadingScreen.Instance;

        SetCustomizationDefaults();
        m_SaveDataManager.Load();
        m_LevelManager.Initialize();
        m_LoadingScreen.Initialize();

        LevelManager.Instance.LoadScene(cutsceneName);
    }

    private void SetCustomizationDefaults() {
        CustomizationDatabase data = m_SaveDataManager.customizeData;

        //Just check if it's already loaded via SaveDataManager.cs
        CustomizationSelected.woodenMask = new CustomizationSelected.SelectionWrapper<CharacterMask>(data.sectionMask[0].options[0].GetComponent<CharacterMask>(), 0);
        CustomizationSelected.hawkMask = new CustomizationSelected.SelectionWrapper<CharacterMask>(data.sectionMask[1].options[0].GetComponent<CharacterMask>(), 0);
        CustomizationSelected.royalMask = new CustomizationSelected.SelectionWrapper<CharacterMask>(data.sectionMask[2].options[0].GetComponent<CharacterMask>(), 0);
        CustomizationSelected.skullMask = new CustomizationSelected.SelectionWrapper<CharacterMask>(data.sectionMask[3].options[0].GetComponent<CharacterMask>(), 0);

        CustomizationSelected.stoneBall = new CustomizationSelected.SelectionWrapper<GameBall>(data.sectionBall[0].options[0].GetComponent<GameBall>(), 0);
        CustomizationSelected.mudBall = new CustomizationSelected.SelectionWrapper<GameBall>(data.sectionBall[1].options[0].GetComponent<GameBall>(), 0);
        CustomizationSelected.beachBall = new CustomizationSelected.SelectionWrapper<GameBall>(data.sectionBall[2].options[0].GetComponent<GameBall>(), 0);
        CustomizationSelected.sunBall = new CustomizationSelected.SelectionWrapper<GameBall>(data.sectionBall[3].options[0].GetComponent<GameBall>(), 0);

        CustomizationSelected.particle = new CustomizationSelected.SelectionWrapper<GameObject>(data.sectionParticle.options[0], 0);

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
}
