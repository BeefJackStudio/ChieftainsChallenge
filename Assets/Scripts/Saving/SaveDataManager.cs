using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

//Must be a MonoBehaviour in order to use OnApplicationQuit
public class SaveDataManager : MonoBehaviourSingleton<SaveDataManager> {

    public const string FILE_NAME = "Chieftains_Challenge_Save_Data.json";

    public SaveData data;

    private string m_SaveDataPath;

    void OnApplicationQuit() {
        Save();
    }

    private void Awake() {
        if (!Directory.Exists(GetFolderPath())) {
            Directory.CreateDirectory(GetFolderPath());
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.S)) Save();
        if (Input.GetKeyDown(KeyCode.L)) Load();
    }

    public void Save() {
        string jsonData = JsonUtility.ToJson(data);
        File.WriteAllText(GetFilePath(), jsonData);
    }

    public void Load() {
        if (File.Exists(GetFilePath())) {
            data = JsonUtility.FromJson<SaveData>(File.ReadAllText(GetFilePath()));
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

    public void SetCutsceneWatched(CutsceneTypes cutscene) {
        if (data.watchedCutscenes.Contains(cutscene)) return;
        data.watchedCutscenes.Add(cutscene);
    }

    public void SetLevelScore(string levelName, int score) {

    }

    private string GetFolderPath() {
        return Application.persistentDataPath;
    }
}
