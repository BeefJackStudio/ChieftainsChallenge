using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// Used to manage Level loading and cutscenes
/// </summary>
public class LevelManager : MonoBehaviourSingleton<LevelManager> {

    private static string GAME_HUD_SCENE = "";

    private GameLevelSet m_CurrentLevel = null;
    private int m_ScenesToLoad = 0;

    public GameLevelSet CurrentLevel { get { return m_CurrentLevel; } }

    public void Initialize() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Loads a normal scene.
    /// </summary>
    /// <param name="levelName"></param>
    public void LoadScene(string levelName, LoadSceneMode mode = LoadSceneMode.Single) {
        LoadingScreen.Instance.Show(() => {
            SceneManager.LoadSceneAsync(levelName, mode);
        });
        m_ScenesToLoad++;
    }

    /// <summary>
    /// Loads a scene as a level, while also loading the HUD and setting the scene as current level.
    /// </summary>
    /// <param name="levelName"></param>
    public void LoadLevel(GameLevelSet level) {
        m_CurrentLevel = level;
        LoadScene(level.scene);
        if (!string.IsNullOrEmpty(GAME_HUD_SCENE)) LoadScene(GAME_HUD_SCENE, LoadSceneMode.Additive);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        m_ScenesToLoad--;
        if(m_ScenesToLoad == 0) {
            LoadingScreen.Instance.Hide(() => { });
        }
    }
}