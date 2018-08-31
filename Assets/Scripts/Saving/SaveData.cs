using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData {
    public float volumeMaster = 1;
    public float volumeSFX = 1;
    public float volumeMusic = 1;
    public List<CutsceneTypes> watchedCutscenes = new List<CutsceneTypes>();
    public List<string> completedLevels = new List<string>();
    public int[] customizeMaskOptions = new int[4] { 0, 0, 0, 0 };
    public int[] customizeBallOptions = new int[4] { 0, 0, 0, 0 };
    public int customizeParticleOption = 0;
    public int customizeSkinOption = 0;
    public bool[] masksUnlocked = new bool[16];
    public bool[] ballsUnlocked = new bool[16];
    public bool[] particlesUnlocked = new bool[12];
    public bool[] skinsUnlocked = new bool[12];
    public bool hasUsedUnlock = true;
    public int boxesToOpen = 0;

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

    public void DeserializeCustomization() {
        CustomizationDatabase data = SaveDataManager.Instance.customizeData;

        CustomizationSelected.woodenMask = new CustomizationSelected.SelectionWrapper<CharacterMask>(data.sectionMask[0].options[customizeMaskOptions[0]].GetComponent<CharacterMask>(), customizeMaskOptions[0]);
        CustomizationSelected.hawkMask = new CustomizationSelected.SelectionWrapper<CharacterMask>(data.sectionMask[1].options[customizeMaskOptions[1]].GetComponent<CharacterMask>(), customizeMaskOptions[1]);
        CustomizationSelected.royalMask = new CustomizationSelected.SelectionWrapper<CharacterMask>(data.sectionMask[2].options[customizeMaskOptions[2]].GetComponent<CharacterMask>(), customizeMaskOptions[2]);
        CustomizationSelected.skullMask = new CustomizationSelected.SelectionWrapper<CharacterMask>(data.sectionMask[3].options[customizeMaskOptions[3]].GetComponent<CharacterMask>(), customizeMaskOptions[3]);

        CustomizationSelected.stoneBall = new CustomizationSelected.SelectionWrapper<GameBall>(data.sectionBall[0].options[customizeBallOptions[0]].GetComponent<GameBall>(), customizeBallOptions[0]);
        CustomizationSelected.mudBall = new CustomizationSelected.SelectionWrapper<GameBall>(data.sectionBall[1].options[customizeBallOptions[1]].GetComponent<GameBall>(), customizeBallOptions[1]);
        CustomizationSelected.beachBall = new CustomizationSelected.SelectionWrapper<GameBall>(data.sectionBall[2].options[customizeBallOptions[2]].GetComponent<GameBall>(), customizeBallOptions[2]);
        CustomizationSelected.sunBall = new CustomizationSelected.SelectionWrapper<GameBall>(data.sectionBall[3].options[customizeBallOptions[3]].GetComponent<GameBall>(), customizeBallOptions[3]);

        CustomizationSelected.particle = new CustomizationSelected.SelectionWrapper<GameObject>(data.sectionParticle.options[customizeParticleOption], customizeParticleOption);
    }

    public void SerializeCustomization() {
        customizeMaskOptions[0] = CustomizationSelected.woodenMask.ID;
        customizeMaskOptions[1] = CustomizationSelected.hawkMask.ID;
        customizeMaskOptions[2] = CustomizationSelected.royalMask.ID;
        customizeMaskOptions[3] = CustomizationSelected.skullMask.ID;

        customizeBallOptions[0] = CustomizationSelected.stoneBall.ID;
        customizeBallOptions[1] = CustomizationSelected.mudBall.ID;
        customizeBallOptions[2] = CustomizationSelected.beachBall.ID;
        customizeBallOptions[3] = CustomizationSelected.sunBall.ID;

        customizeParticleOption = CustomizationSelected.particle.ID;
    }
}
