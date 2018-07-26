using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;

//Must be a MonoBehaviour in order to use OnApplicationQuit
public class SaveDataManager : MonoBehaviourSingleton<SaveDataManager> {

    public const string FILE_NAME = "Chieftains_Challenge_Save_Data.json";
    public const float HOUR_REGEN_PER_LIFE = 3f;
    public const int LEVELS_PER_ITEM = 5;

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
            m_SaveDataPath = Path.Combine(GetFolderPath(), FILE_NAME);
        }
        return m_SaveDataPath;
    }

    public void UpdateLivesGained() {
        int livesToGain = GetLivesToGain();
        ModifyLifeCount(livesToGain);
    }

    public void ModifyLifeCount(int i) {
        if (i != 0) {
            if (i > 0) {
                int secondsSinceEpoch = GetSecondsSinceEpoch();
                int stepAmount = (int)(60 * 60 * HOUR_REGEN_PER_LIFE);
                while (true) {
                    if (data.lastLivesClaim + stepAmount > secondsSinceEpoch) break;
                    data.lastLivesClaim += stepAmount;
                }
            }

            data.currentLives = Mathf.Clamp(data.currentLives + i, 0, 3);
            LivesIndicator.Instance.SetLivesCount(data.currentLives);
        }
    }

    public TimeSpan GetTimespanUntilNextLife() {
        int secondsSinceEpoch = GetSecondsSinceEpoch();
        int lastClaimSeconds = data.lastLivesClaim;
        int futureClaimSeconds = lastClaimSeconds + (int)((GetLivesToGain() + 1) * 60 * 60 * HOUR_REGEN_PER_LIFE);

        TimeSpan difference = new TimeSpan(0, 0, futureClaimSeconds - secondsSinceEpoch);
        return difference;
    }

    public int GetLivesToGain() {
        TimeSpan timeSpan = new TimeSpan(0, 0, GetSecondsSinceEpoch() - data.lastLivesClaim);
        return Mathf.FloorToInt((int)(timeSpan.TotalMinutes / 60 / HOUR_REGEN_PER_LIFE));
    }

    public int GetPotentialLivesToGain() {
        return 3 - data.currentLives;
    }

    private int GetSecondsSinceEpoch() {
        return (int)GetTimeSinceEpoch().TotalSeconds;
    }
    
    private TimeSpan GetTimeSinceEpoch() {
        return (DateTime.UtcNow - new DateTime(1970, 1, 1));
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
        Debug.Log(Application.persistentDataPath);
        return Application.persistentDataPath;
    }

    public int GetScoreLeftToUnlock() {
        int perfectLevelScores = 0;
        foreach (int score in m_LevelScores.Values) {
            if (score == 3) perfectLevelScores++;
        }

        return LEVELS_PER_ITEM - Mathf.FloorToInt(perfectLevelScores % LEVELS_PER_ITEM);
    }

    public bool UpdateItemClaimCount() {
        int perfectLevelScores = 0;
        foreach (int score in m_LevelScores.Values) {
            if (score == 3) perfectLevelScores++;
        }

        int leftToUnlock = GetScoreLeftToUnlock();
        bool unlock = leftToUnlock == LEVELS_PER_ITEM;
        bool canUseUnlock = unlock && !data.hasUsedUnlock;
        if (leftToUnlock != LEVELS_PER_ITEM) data.hasUsedUnlock = false;

        if (canUseUnlock) {
            data.boxesToOpen++;
        }
        return canUseUnlock; 
    }

    public GameObject UnlockRandomItem() {
        data.hasUsedUnlock = true;

        List<int> maskIndexes = GetLockedIndexes(data.masksUnlocked);
        List<int> ballIndexes = GetLockedIndexes(data.ballsUnlocked);
        List<int> skinIndexes = GetLockedIndexes(data.skinsUnlocked);
        List<int> particleIndexes = GetLockedIndexes(data.particlesUnlocked);

        GameObject obj = null;
        while(obj == null) {
            int category = UnityEngine.Random.Range(0, 5);

            if(category == 0 && maskIndexes.Count != 0) {
                int index = maskIndexes[UnityEngine.Random.Range(0, maskIndexes.Count - 1)];
                obj = customizeData.sectionMask[Mathf.FloorToInt(index / 4)].options[index % 4];
                data.masksUnlocked[index] = true;
            }

            if (category == 1 && ballIndexes.Count != 0) {
                int index = ballIndexes[UnityEngine.Random.Range(0, ballIndexes.Count - 1)];
                obj = customizeData.sectionBall[Mathf.FloorToInt(index / 4)].options[index % 4];
                data.ballsUnlocked[index] = true;
            }

            if (category == 2 && skinIndexes.Count != 0) {
                int index = skinIndexes[UnityEngine.Random.Range(0, skinIndexes.Count - 1)];
                obj = customizeData.sectionSkin.options[index];
                data.skinsUnlocked[index] = true;
            }

            if (category == 3 && particleIndexes.Count != 0) {
                int index = particleIndexes[UnityEngine.Random.Range(0, particleIndexes.Count - 1)];
                obj = customizeData.sectionParticle.options[index];
                data.particlesUnlocked[index] = true;
            }

            if (maskIndexes.Count == 0 &&
                ballIndexes.Count == 0 &&
                skinIndexes.Count == 0 &&
                particleIndexes.Count == 0) break;
        }
        return obj;
    }

    private List<int> GetLockedIndexes(bool[] array) {
        List<int> l = new List<int>();
        for(int i = 0;i < array.Length; i++) {
            if (!array[i]) l.Add(i);
        }
        return l;
    }
}
