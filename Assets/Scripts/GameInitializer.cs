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

        m_SaveDataManager.Load();
        m_LevelManager.Initialize();
        m_LoadingScreen.Initialize();

        LevelManager.Instance.LoadScene(cutsceneName);
    }
}
