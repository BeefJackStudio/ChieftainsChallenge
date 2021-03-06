﻿using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;

//Must be a MonoBehaviour in order to use OnApplicationQuit
public class SaveDataManager : MonoBehaviourSingleton<SaveDataManager> {

    public const string FILE_NAME = "Chieftains_Challenge_Save_Data.json";
    public const float HOUR_REGEN_PER_LIFE = 3f;
    public const int LEVELS_PER_ITEM = 2;

    public SaveData data;
    public CustomizationDatabase customizeData;

    private string m_SaveDataPath;
    private Dictionary<string, int> m_LevelScores = new Dictionary<string, int>();
    private int m_ClaimedItems = 0;

    #region Saving and loading
    void OnApplicationQuit() {
        Save();
    }

    public void Save() {
        if (!Directory.Exists(GetFolderPath())) {
            Directory.CreateDirectory(GetFolderPath());
        }

        data.SerializeScores(m_LevelScores);
        data.SerializeCustomization();

        string jsonData = JsonUtility.ToJson(data);
        File.WriteAllText(GetFilePath(), jsonData);
    }

    public void Load() {
        if (!Directory.Exists(GetFolderPath())) {
            Directory.CreateDirectory(GetFolderPath());
        }

        if (File.Exists(GetFilePath())) {
            data = JsonUtility.FromJson<SaveData>(File.ReadAllText(GetFilePath()));

            m_LevelScores = data.DeserializeScores();
            data.DeserializeCustomization();

        } else {
            data = new SaveData();
        }
    }

    public string GetFilePath() {
        if (string.IsNullOrEmpty(m_SaveDataPath)) {
            Debug.Log(GetFolderPath());
            m_SaveDataPath = Path.Combine(GetFolderPath(), FILE_NAME);
        }
        return m_SaveDataPath;
    }

#endregion

    public void SetCutsceneWatched(CutsceneTypes cutscene) {
        if (data.watchedCutscenes.Contains(cutscene)) return;
        data.watchedCutscenes.Add(cutscene);
    }

    public void SetLevelScore(string levelName, int score) {
        int existingScore = 0;
        if (m_LevelScores.ContainsKey(levelName)) existingScore = m_LevelScores[levelName];
        if (score > existingScore) m_LevelScores[levelName] = score;
    }

    public int GetLevelScore(string levelName) {
        if (m_LevelScores.ContainsKey(levelName)) return m_LevelScores[levelName];
        return -1;
    }

    private string GetFolderPath() {
        return Application.persistentDataPath;
    }

    public int GetScoreLeftToUnlock() {
        int perfectLevelScores = 0;
        foreach (int score in m_LevelScores.Values) {
            if (score == 3) perfectLevelScores++;
        }

        int result = LEVELS_PER_ITEM - (perfectLevelScores % LEVELS_PER_ITEM);
        return result;
    }

    public bool UpdateItemClaimCount() {
        int leftToUnlock = GetScoreLeftToUnlock();
        bool attemptUnlock = leftToUnlock == LEVELS_PER_ITEM;
        if (attemptUnlock) {
            if (!data.hasUsedUnlock) {
                data.boxesToOpen++;
                data.hasUsedUnlock = true;
                return true;
            }
        } else {
            data.hasUsedUnlock = false;
        }
        return false; 
    }

    public CustomizationUnlockData UnlockRandomItem() {
        CustomizationUnlockData unlockData = null;

        List<int> maskIndexes = GetLockedIndexes(data.masksUnlocked);
        List<int> ballIndexes = GetLockedIndexes(data.ballsUnlocked);
        List<int> skinIndexes = GetLockedIndexes(data.skinsUnlocked);
        List<int> particleIndexes = GetLockedIndexes(data.particlesUnlocked);

        while(unlockData == null) {
            
            //If nothing is left to unlock
            if (maskIndexes.Count == 0 &&
                ballIndexes.Count == 0 &&
                skinIndexes.Count == 0 &&
                particleIndexes.Count == 0) break;

            unlockData = new CustomizationUnlockData();

            int category = UnityEngine.Random.Range(0, 4);
            unlockData.category = category;

            if (category == 0 && maskIndexes.Count != 0) {
                int index = maskIndexes[UnityEngine.Random.Range(0, maskIndexes.Count - 1)];

                unlockData.listIndex = index;
                unlockData.obj = customizeData.sectionMask[Mathf.FloorToInt(index / 4)].options[index % 4];
            }

            if (category == 1 && ballIndexes.Count != 0) {
                int index = ballIndexes[UnityEngine.Random.Range(0, ballIndexes.Count - 1)];

                unlockData.listIndex = index;
                unlockData.obj = customizeData.sectionBall[Mathf.FloorToInt(index / 4)].options[index % 4];
            }

            if (category == 2 && skinIndexes.Count != 0) {
                int index = skinIndexes[UnityEngine.Random.Range(0, skinIndexes.Count - 1)];

                unlockData.listIndex = index;
                unlockData.obj = customizeData.sectionSkin.options[index];
            }

            if (category == 3 && particleIndexes.Count != 0) {
                int index = particleIndexes[UnityEngine.Random.Range(0, particleIndexes.Count - 1)];

                unlockData.listIndex = index;
                unlockData.obj = customizeData.sectionParticle.options[index];
            }
            Debug.Log("Unlock data = " + category + ", " + unlockData.listIndex);
        }
        return unlockData;
    }

    private List<int> GetLockedIndexes(bool[] array) {
        List<int> l = new List<int>();
        for(int i = 0;i < array.Length; i++) {
            if (!array[i]) l.Add(i);
        }
        return l;
    }
}
