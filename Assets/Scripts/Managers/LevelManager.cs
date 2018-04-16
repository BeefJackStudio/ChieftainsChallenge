using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// Used to manage Level loading and cutscenes
/// </summary>
public static class LevelManager {

    public static void LoadLevel(string levelName) {
        SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
    }

}