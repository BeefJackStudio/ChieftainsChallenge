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
    private List<Action> m_WaitForLoadCalls = new List<Action>();

    public Action OnScenesLoaded = delegate { };
    public GameLevelSet CurrentLevel { get { return m_CurrentLevel; } }

    public void Initialize() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void WaitForScenesLoaded(Action onLoaded) {
        if(m_ScenesToLoad == 0 && m_QueuedScenes.Count == 0) {
            onLoaded();
        }else {
            OnScenesLoaded += onLoaded;
            m_WaitForLoadCalls.Add(onLoaded);
        }
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
        if (m_ScenesToLoad == 0) {
            if (m_QueuedScenes.Count == 0) {
                LoadingScreen.Instance.Hide(() => { });
                OnScenesLoaded();
                foreach(Action call in m_WaitForLoadCalls) {
                    OnScenesLoaded -= call;
                }
            } else {
                LoadScene(m_QueuedScenes[0], LoadSceneMode.Additive);
                m_QueuedScenes.RemoveAt(0);
            }
        }
    }
}