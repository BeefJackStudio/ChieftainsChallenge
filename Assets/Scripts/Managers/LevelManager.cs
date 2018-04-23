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

    private static string m_CurrentLevelName = "";

    public static string CurrentLevelName { get { return m_CurrentLevelName; } }

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
    public static void LoadLevel(string levelName) {
        m_CurrentLevelName = levelName;
        LoadScene(levelName);

        if (!string.IsNullOrEmpty(GAME_HUD_SCENE)) LoadScene(GAME_HUD_SCENE, LoadSceneMode.Additive);
    }

}