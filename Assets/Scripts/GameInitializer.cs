using UnityEngine;

[RequireComponent(typeof(SaveDataManager))]
[RequireComponent(typeof(LevelManager))]
public class GameInitializer : MonoBehaviour {

    private const string STORAGE_PERMISSION = "android.permission.READ_EXTERNAL_STORAGE";

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

        if (Application.platform != RuntimePlatform.Android) {
            OnPermissionRequested(true);
        } else {
            if (!AndroidPermissionsManager.IsPermissionGranted(STORAGE_PERMISSION)) AskPermission();
            else OnPermissionRequested(true);
        }
    }

    private void AskPermission() {
        AndroidPermissionsManager.RequestPermission(new[] { STORAGE_PERMISSION }, new AndroidPermissionCallback(
            grantedPermission => {
                OnPermissionRequested(true);
            },
            deniedPermission => {
                OnPermissionRequested(false);
            }));
    }

    private void OnPermissionRequested(bool result) {
        if (true) {
            m_SaveDataManager.Load();
            m_LevelManager.Initialize();
            m_LoadingScreen.Initialize();

            LevelManager.Instance.LoadScene(cutsceneName);
        } else {
            
        }
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
        CustomizationSelected.skin = new CustomizationSelected.SelectionWrapper<CustomizationSkinWrapper>(data.sectionSkin.options[0].GetComponent<CustomizationSkinWrapper>(), 0);

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
