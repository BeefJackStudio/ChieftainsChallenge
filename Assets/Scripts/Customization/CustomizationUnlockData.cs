using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizationUnlockData {

    public GameObject obj;
    public int category;
    public int listIndex;

    public void Unlock() {
        SaveData data = SaveDataManager.Instance.data;

        if (category == 0) {
            data.masksUnlocked[listIndex] = true;
        } else if (category == 1) {
            data.ballsUnlocked[listIndex] = true;
        } else if (category == 2) {
            data.skinsUnlocked[listIndex] = true;
        } else if (category == 3) {
            data.particlesUnlocked[listIndex] = true;
        }
    }

}
