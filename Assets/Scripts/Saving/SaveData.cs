using System;
using System.Collections.Generic;

[Serializable]
public class SaveData {
    public float volumeMaster = 1;
    public float volumeSFX = 1;
    public float volumeMusic = 1;
    public List<CutsceneTypes> watchedCutscenes = new List<CutsceneTypes>();
    public List<string> completedLevels = new List<string>();

    public Dictionary<string, int> DeserializeScores() {
        Dictionary<string, int> scores = new Dictionary<string, int>();

        foreach(string entry in completedLevels) {
            string[] split = entry.Split('=');
            scores.Add(split[0], int.Parse(split[1]));
        }

        return scores;
    }

    public void SerializeScores(Dictionary<string, int> scores) {
        completedLevels.Clear();

        foreach(KeyValuePair<string, int> entry in scores) {
            completedLevels.Add(entry.Key + "=" + entry.Value);
        }
    }
}
