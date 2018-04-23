using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// Used to manage Level loading and cutscenes
/// </summary>
public static class LevelManager {

    private static string GAME_HUD_SCENE = "";

    private static GameLevelSet m_CurrentLevel = null;

    public static GameLevelSet CurrentLevel { get { return m_CurrentLevel; } }

    /// <summary>
    /// Loads a normal scene.
    /// </summary>
    /// <param name="levelName"></param>
    public static void LoadScene(string levelName, LoadSceneMode mode = LoadSceneMode.Single) {
        SceneManager.LoadSceneAsync(levelName, mode);
    }

    /// <summary>
    /// Loads a scene as a level, while also loading the HUD and setting the scene as current level.
    /// </summary>
    /// <param name="levelName"></param>
    public static void LoadLevel(GameLevelSet level) {
        m_CurrentLevel = level;
        LoadScene(level.scene);

        if (!string.IsNullOrEmpty(GAME_HUD_SCENE)) LoadScene(GAME_HUD_SCENE, LoadSceneMode.Additive);
    }

}