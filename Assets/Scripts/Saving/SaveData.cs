using System;
using System.Collections.Generic;

[Serializable]
public class SaveData {
    public float volumeMaster = 1;
    public float volumeSFX = 1;
    public float volumeMusic = 1;
    public List<CutsceneTypes> watchedCutscenes = new List<CutsceneTypes>();
}
