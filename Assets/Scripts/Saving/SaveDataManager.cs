using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

//Must be a MonoBehaviour in order to use OnApplicationQuit
public class SaveDataManager : MonoBehaviourSingleton<SaveDataManager> {

    public const string FILE_NAME = "Chieftains_Challenge_Save_Data.json";

    public SaveData data;

    private string m_SaveDataPath;
    private Dictionary<string, int> m_LevelScores = new Dictionary<string, int>();

    #region Saving and loading
    void OnApplicationQuit() {
        Save();
    }

    public void Save() {
        if (!Directory.Exists(GetFolderPath())) {
            Directory.CreateDirectory(GetFolderPath());
        }

        data.SerializeScores(m_LevelScores);

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
}
