using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class GameLevelCollection : ScriptableObject {

    public string displayName;
    public string scenePrefix;
    public List<GameLevelSet> levels = new List<GameLevelSet>();

    private void OnEnable() {
        foreach(GameLevelSet set in levels) {
            set.continent = this;
        }
    }

    public bool IsCompleted() {
        if (levels.Count == 0) return false;
        foreach(GameLevelSet level in levels) {
            int score = SaveDataManager.Instance.GetLevelScore(level.scene);
            if (score == -1) return false;
        }
        return true;
    }

    [ContextMenu("Re-order levels")]
    private void OrderLevels() {
        levels.Sort((l1, l2) => l1.index.CompareTo(l2.index));
    }

    [ContextMenu("Auto name levels")]
    private void NameLevels() {
        int i = 1;
        foreach(GameLevelSet set in levels) {
            set.scene = scenePrefix + i;
            set.index = i;
            i++;
        }
    }
}

[Serializable]
public class GameLevelSet {
    public string scene;
    public int index;

    [HideInInspector]
    public GameLevelCollection continent;

    public GameLevelSet GetNextLevel() {
        int nextIndex = index + 1;
        if (nextIndex < continent.levels.Count) {
            return continent.levels[nextIndex];
        }
        return null;
    }
}