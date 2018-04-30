using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// Used to manage Level loading and cutscenes
/// </summary>
public class LevelManager : MonoBehaviourSingleton<LevelManager> {

    private static string GAME_HUD_SCENE = "Game_HUD";

    private GameLevelSet m_CurrentLevel = null;
    private int m_ScenesToLoad = 0;
    private List<string> m_QueuedScenes = new List<string>();

    public Action OnScenesLoaded = delegate { };
    public GameLevelSet CurrentLevel { get { return m_CurrentLevel; } }

    public void Initialize() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Loads a normal scene.
    /// </summary>
    /// <param name="levelName"></param>
    public void LoadScene(string levelName, LoadSceneMode mode = LoadSceneMode.Single) {
        m_ScenesToLoad++;
        if (LoadingScreen.Instance.IsShown) {
            SceneManager.LoadSceneAsync(levelName, mode);
        } else {
            LoadingScreen.Instance.Show(() => {
                SceneManager.LoadSceneAsync(levelName, mode);
            });
        }
    }

    /// <summary>
    /// Loads a scene as a level, while also loading the HUD and setting the scene as current level.
    /// </summary>
    /// <param name="levelName"></param>
    public void LoadLevel(GameLevelSet level) {
        m_CurrentLevel = level;
        LoadScene(level.scene);
        m_QueuedScenes.Add(GAME_HUD_SCENE);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        m_ScenesToLoad--;
        if(m_ScenesToLoad == 0) {
            if (m_QueuedScenes.Count == 0) {
                LoadingScreen.Instance.Hide(() => { });
                OnScenesLoaded();
            }else {
                foreach(string s in m_QueuedScenes) {
                    LoadScene(s, LoadSceneMode.Additive);
                }
                m_QueuedScenes.Clear();
            }
        }
    }
}