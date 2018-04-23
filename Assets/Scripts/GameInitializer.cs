using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SaveDataManager))]
public class GameInitializer : MonoBehaviour {

    public string cutsceneName;

    private SaveDataManager m_SaveDataManager;

    private void Awake() {
        m_SaveDataManager = GetComponent<SaveDataManager>();
    }

    private void Start() {
        m_SaveDataManager.Load();

        LevelManager.LoadScene(cutsceneName);
    }
}
