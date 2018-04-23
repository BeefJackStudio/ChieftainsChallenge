using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class GameLevelCollection : ScriptableObject {

    public List<GameLevelSet> levels = new List<GameLevelSet>();

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
}

[Serializable]
public class GameLevelSet {
    public string scene;
    public int index;
}

public enum GameContinents {
    EUROPE,
    MIDDLE_EAST,
    NORTH_ASIA,
    ASIA,
    OCEANIA,
    AFRICA,
    NORTH_AMERICA,
    SOUTH_AMERICA
}